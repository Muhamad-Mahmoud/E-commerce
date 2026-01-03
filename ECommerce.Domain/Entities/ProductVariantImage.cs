using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductVariantImage
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        public string ImageUrl { get; set; }

        public bool IsPrimary { get; set; } = false;
    }
}
