using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;
using NordPoolAPI.Models;
using NordPoolAPI.Repositories;
using RestSharp;

namespace NordPoolAPI.Controllers
{
	[Route("spotprices/hourly")]
	[ApiController]
	public class HourlyController : ControllerBase
	{
		private readonly HourlySpotPriceRepository _hourlySpotPriceRepository;

		public HourlyController()
		{
			_hourlySpotPriceRepository = new HourlySpotPriceRepository();
		}

		[HttpGet]
		public async Task<IEnumerable<Area>> Get(string area)
		{
			if (!string.IsNullOrWhiteSpace(area))
			{
				var result = await _hourlySpotPriceRepository.GetHourlyRatesForArea(area);

				return new[] { result };
			}

			var results = await _hourlySpotPriceRepository.GetHourlyRates();
			return results;
		}
	}
}
