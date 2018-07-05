using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

		// GET api/values
		[HttpGet]
		public async Task<IEnumerable<Area>> Get(string area)
		{
			var result = await _hourlySpotPriceRepository.GetHourlyRates(area);
			return result;
		}
	}
}
