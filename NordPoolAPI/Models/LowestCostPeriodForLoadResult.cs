using System;
using System.Linq;

namespace NordPoolAPI.Models
{
	public class LowestCostPeriodForLoadResult
	{
		private readonly double _power;
		private readonly TimeSpan _time;
		private readonly SpotPrice[] _spotPrices;
		private readonly double _fraction;
		private readonly double _hours;

		public LowestCostPeriodForLoadResult(double energy, double power, TimeSpan time, params SpotPrice[] spotPrices)
		{
			EnergyUnits = energy;
			_power = power;
			_time = time;
			_spotPrices = spotPrices;

			_hours = _time.TotalHours / 1;
			_fraction = _time.TotalHours % 1;
		}

		public decimal Cost => GetCost();

		public double EnergyUnits { get; }

		public decimal AveragePricePerUnit => _spotPrices.Average(s => s.Price);

		public string UnitType { get; } = "kWh";

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
		public DateTime Stop => _spotPrices.Last().Time.AddHours(_fraction == 0 ? 1 : _fraction);

	}
}