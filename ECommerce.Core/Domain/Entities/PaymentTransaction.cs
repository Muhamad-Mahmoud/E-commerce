using ECommerce.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Domain.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string? ProviderTransactionId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}
