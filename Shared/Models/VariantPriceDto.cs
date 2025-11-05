using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class VariantPriceDto
	{
		public Guid Id { get; set; }
		public string Currency { get; set; } = "VND";
		public decimal Price { get; set; }
		public DateTime EffectiveFrom { get; set; }
	}
}

