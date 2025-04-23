using TechnicalTest.Models;

public class PaginationViewModel
{
    public List<SoOrderModel> Orders { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}
