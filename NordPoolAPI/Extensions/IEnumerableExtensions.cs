using System.Linq;

namespace System.Collections.Generic
{
	public static class IEnumerableExtensions
	{
		public static double WeightedAverage<T>(this IEnumerable<T> records, Func<T, double> value, Func<T, double> weight)
		{
			var weightedValueSum = records.Sum(x => value(x) * weight(x));
			var weightSum = records.Sum(weight);

			if (weightSum == 0)
				throw new DivideByZeroException("Your message here");

			return weightedValueSum / weightSum;
		}
	}
}