using System.ComponentModel.DataAnnotations;

namespace TechnicalTest.Models
{
    public class SoItemModel
    {
        [Key]
        public long SoItemId { get; set; }
        public long SoOrderId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public SoOrderModel SoOrder { get; set; }
    }
}
