using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ProductVariantDto
	{
		public Guid Id { get; set; }
		public string SKU { get; set; } = string.Empty;
		public string? Attributes { get; set; }
		public decimal BasePrice { get; set; }
		public int Stock { get; set; }
		public IEnumerable<VariantPriceDto>? Prices { get; set; }
	}
}
