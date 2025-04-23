using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Models
{
    public class ComCustomerModel
    {
        [Key]
        public int ComCustomerId { get; set; }

        public string CustomerName { get; set; }

        public ICollection<SoOrderModel> SoOrders { get; set; } = new List<SoOrderModel>();
    }
}
