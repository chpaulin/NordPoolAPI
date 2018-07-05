using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NordPoolAPI.Models
{
    public class Area
    {
	    public Area(string name)
	    {
		    Name = name;
	    }

	    public string Name { get; set; }
	    public IList<SpotPrice> SpotPrices { get; set; } = new List<SpotPrice>();
	    public SpotPrice Min => SpotPrices.OrderBy(s => s.Price).FirstOrDefault();
	    public SpotPrice Max => SpotPrices.OrderByDescending(s => s.Price).FirstOrDefault();
	    public decimal Average => SpotPrices.Average(s => s.Price);
	    public string Currency { get; } = "SEK";
	    public string UnitType { get; } = "kWh";
	}
}
