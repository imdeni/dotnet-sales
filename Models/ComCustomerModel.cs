namespace TechnicalTest.Models

{
    using System.ComponentModel.DataAnnotations;
    public class ComCustomerModel
    {
        [Key]
        public int ComCustomerId { get; set; }
        public string CustomerName { get; set; }

        public ICollection<SoOrderModel> SoOrders { get; set; }
    }
}
