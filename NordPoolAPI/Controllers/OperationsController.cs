using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NordPoolAPI.Models;
using NordPoolAPI.Repositories;

namespace NordPoolAPI.Controllers
{
	[Route("operations")]
	[ApiController]
	public class OperationsController : ControllerBase
	{
		private readonly HourlySpotPriceRepository _hourlySpotPriceRepository;

		public OperationsController()
		{
			_hourlySpotPriceRepository = new HourlySpotPriceRepository();
		}

		/// <summary>
		/// Returns which period as of now the lowest price is for a specific amount of energy (kWh) at an specified output power (kW)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("GetLowesCostPeriodForLoad")]
		public async Task<LowestCostPeriodForLoadResult> GetLowesCostPeriodForLoad(double energy, double power, string area)
		{
			var result = await GetOptimalPeriodAsync(energy, power, area);
			return result;
		}

		private async Task<LowestCostPeriodForLoadResult> GetOptimalPeriodAsync(double energy, double power, string area)
		{
			var outputTime = TimeSpan.FromHours(energy / power); //Time it takes to deliver the energy in hours		

			var areaObj = await _hourlySpotPriceRepository.GetHourlyRatesForArea(area);

			if (outputTime <= TimeSpan.FromHours(1))
				return new LowestCostPeriodForLoadResult(energy, power, outputTime, areaObj.Min);

			var noSpots = (int) Math.Ceiling(outputTime.TotalHours);

			LowestCostPeriodForLoadResult lowestCostPeriod = null;

			for (var i = 0; i + noSpots < areaObj.SpotPrices.Count; i++)
			{
				var spots = areaObj.SpotPrices.Skip(i).Take(noSpots).ToArray();

				var result = new LowestCostPeriodForLoadResult(energy, power, outputTime, spots);

				if (lowestCostPeriod == null || result.Cost < lowestCostPeriod.Cost)
					lowestCostPeriod = result;
			}

			return lowestCostPeriod;
		}
	}
}