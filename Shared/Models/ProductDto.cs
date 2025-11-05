using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class ProductDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? ShortDescription { get; set; }
		public string? MainImageUrl { get; set; }
		public IEnumerable<ProductVariantDto>? Variants { get; set; }
	}
}
