using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Models
{
    public class SoOrderModel
    {
        [Key]
        public long SoOrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int ComCustomerId { get; set; }
        public string Address { get; set; }

        public ComCustomerModel ComCustomer { get; set; }
        public ICollection<SoItemModel> SoItems { get; set; }
    }
}
