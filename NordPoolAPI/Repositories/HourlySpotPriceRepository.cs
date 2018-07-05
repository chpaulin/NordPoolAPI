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
		private readonly Regex _rowRegex = new Regex(ROW_PATTERN);
		private readonly RestClient _client;

		public HourlySpotPriceRepository()
		{
			_client = new RestClient("https://www.nordpoolgroup.com/api/");
		}

		public async Task<IEnumerable<Area>> GetHourlyRates(string area)
		{
			var resultToday = await _client.ExecuteTaskAsync<NordPoolResponse>(new RestRequest($"marketdata/page/10?endDate={DateTime.Today:dd-MM-yyyy}", Method.GET));
			var resultTomorrow = await _client.ExecuteTaskAsync<NordPoolResponse>(new RestRequest($"marketdata/page/10?endDate={DateTime.Today.AddDays(1):dd-MM-yyyy}", Method.GET));

			var rows = resultToday.Data.data.Rows
				.Concat(resultTomorrow.Data.data.Rows)
				.Where(r => _rowRegex.IsMatch(r.Name) && r.StartTime > DateTime.Now.AddHours(-1))
				.OrderBy(r => r.StartTime);

			IList<Area> areas;

			if (!string.IsNullOrWhiteSpace(area))
			{
				var areaObj = new Area(area);

				areas = new List<Area> { areaObj };

				foreach (var row in rows)
				{
					foreach (var rowColumn in row.Columns.Where(c => c.Name == area))
					{
						if (decimal.TryParse(rowColumn.Value, out var value))
						{
							areaObj.SpotPrices.Add(new SpotPrice(value, row.StartTime));
						}
					}
				}

				return areas;
			}

			areas = rows.SelectMany(r => r.Columns).Select(c => c.Name).Distinct().Select(a => new Area(a)).ToList();

			foreach (var row in rows)
			{
				foreach (var rowColumn in row.Columns)
				{
					var areaObj = areas.First(a => a.Name == rowColumn.Name);

					areaObj.SpotPrices.Add(new SpotPrice(decimal.Parse(rowColumn.Value), row.StartTime));
				}
			}

			return areas;
		}
	}
}