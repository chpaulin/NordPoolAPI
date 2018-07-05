using System;
using System.Collections.Generic;
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
		public async Task<LowesCostPeriodForLoadResult> GetLowesCostPeriodForLoad(double energy, double power, string area)
		{
			var outputTime = TimeSpan.FromHours(energy / power); //Time it takes to deliver the energy in hours		

			var areaObj = await _hourlySpotPriceRepository.GetHourlyRatesForArea(area);

			if (outputTime <= TimeSpan.FromHours(1))
				return new LowesCostPeriodForLoadResult(power, outputTime, areaObj.Min);

			return null;
		}
	}
}