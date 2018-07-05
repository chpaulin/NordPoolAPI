using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NordPoolAPI.Models
{
    public class SpotPrice
    {
	    public SpotPrice(decimal price, DateTime time)
	    {
		    Price = price;
		    Time = time;
	    }

	    public decimal Price { get; set; }
	    public DateTime Time { get; set; }
    }
}
