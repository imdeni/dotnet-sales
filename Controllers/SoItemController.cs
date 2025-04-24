using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Models;

namespace TechnicalTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SoItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var item = _context.SoItems.Find(id);
            if (item == null)
            {
                return NotFound(new { message = "Item not found" });
            }

            _context.SoItems.Remove(item);
            _context.SaveChanges();

            return Ok(new { message = "Item deleted successfully" });
        }

    [HttpPut("UpdateItem/{SoItemId}")]
    public IActionResult UpdateItem(int SoItemId, [FromBody] SoItemModel itemData)
    {
        var item = _context.SoItems.FirstOrDefault(i => i.SoItemId == SoItemId);

        if (item == null)
        {
            return NotFound(new { success = false, message = "Item not found." });
        }

        item.SoOrderId = itemData.SoOrderId;
        item.ItemName = itemData.ItemName;
        item.Quantity = itemData.Quantity;
        item.Price = itemData.Price;

        try
        {
            _context.Update(item);
            _context.SaveChanges();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, message = ex.Message });
        }
    }
    }
}
