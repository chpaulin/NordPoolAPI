using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NordPoolAPI.Models;
using RestSharp;

namespace NordPoolAPI.Repositories
{
	public class HourlySpotPriceRepository
	{
		private const string ROW_PATTERN = "\\d{2}&nbsp;-&nbsp;\\d{2}";
		private const string API_ADDRESS = "https://www.nordpoolgroup.com/api/";
		private readonly Regex _rowRegex = new Regex(ROW_PATTERN);
		private readonly RestClient _client;

		public HourlySpotPriceRepository()
		{
			_client = new RestClient(API_ADDRESS);
		}

		public async Task<Area> GetHourlyRatesForArea(string area)
		{
			if (string.IsNullOrWhiteSpace(area))
				throw new ArgumentNullException(nameof(area));

			var rates = await GetRates();

			var areaObj = new Area(area);

			foreach (var row in rates)
			{
				foreach (var rowColumn in row.Columns.Where(c => c.Name == area))
				{
					if (decimal.TryParse(rowColumn.Value, out var value))
					{
						value = decimal.Divide(value, 1000);

						areaObj.SpotPrices.Add(new SpotPrice(value, row.StartTime));
					}
				}
			}

			return areaObj;
		}

		public async Task<IEnumerable<Area>> GetHourlyRates()
		{
			var rates = await GetRates();

			var areas = rates.SelectMany(r => r.Columns).Select(c => c.Name).Distinct().Select(a => new Area(a)).ToList();

			foreach (var row in rates)
			{
				foreach (var rowColumn in row.Columns)
				{
					var areaObj = areas.First(a => a.Name == rowColumn.Name);

					if (decimal.TryParse(rowColumn.Value, out var value))
					{
						value = decimal.Divide(value, 1000);

						areaObj.SpotPrices.Add(new SpotPrice(value, row.StartTime));
					}
				}
			}

			return areas;
		}

		private async Task<IOrderedEnumerable<Row>> GetRates()
		{
			var resultToday = await _client.ExecuteTaskAsync<NordPoolResponse>(new RestRequest($"marketdata/page/10?currency=SEK&endDate={DateTime.Today:dd-MM-yyyy}", Method.GET));
			var resultTomorrow = await _client.ExecuteTaskAsync<NordPoolResponse>(new RestRequest($"marketdata/page/10?currency=SEK&endDate={DateTime.Today.AddDays(1):dd-MM-yyyy}", Method.GET));

			var rows = resultToday.Data.data.Rows
				.Concat(resultTomorrow.Data.data.Rows)
				.Where(r => _rowRegex.IsMatch(r.Name) && r.StartTime > DateTime.Now.AddHours(-1))
				.OrderBy(r => r.StartTime);

			return rows;
		}
	}
}