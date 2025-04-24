using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        [JsonIgnore]
        [ValidateNever]
        public SoOrderModel SoOrder { get; set; }
    }
}
