using System;
using System.Linq;

namespace NordPoolAPI.Models
{
	public class LowesCostPeriodForLoadResult
	{
		private readonly double _power;
		private readonly TimeSpan _time;
		private readonly SpotPrice[] _spotPrices;
		private readonly double _fraction;
		private readonly double _hours;

		public LowesCostPeriodForLoadResult(double power, TimeSpan time, params SpotPrice[] spotPrices)
		{
			_power = power;
			_time = time;
			_spotPrices = spotPrices;

			_hours = _time.TotalHours / 1;
			_fraction = _time.TotalHours % 1;
		}

		public decimal Cost => GetCost();

		public string Currency { get; } = "SEK";

		private decimal GetCost()
		{
			decimal cost = 0;

			for (var i = 0; i < _hours; i++)
				cost += (decimal)_power * _spotPrices[i].Price;

			if(_fraction > 0)
				cost += (decimal)(_power * _fraction) * _spotPrices.Last().Price;

			return decimal.Round(cost, 2);
		}

		public DateTime Start => _spotPrices.First().Time;
		public DateTime Stop => _spotPrices.Last().Time.AddHours(_fraction);

	}
}