using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnicalTest.Models;

namespace TechnicalTest.Models
{
    public class SoOrderViewModel
{
    [Required]
    public SoOrderModel SoOrder { get; set; }

    [Required]
    public List<SoItemModel> SoItems { get; set; }

    public List<SelectListItem> Customers { get; set; }
}

}
