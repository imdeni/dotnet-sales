using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Mvc;


public class SoOrderController : Controller
{
    private readonly AppDbContext _context;

    public SoOrderController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string keyword, DateTime? orderDate, int page = 1)
    {
        const int pageSize = 10;

        var query = _context.SoOrders
            .Include(o => o.ComCustomer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(o =>
                o.OrderNo.Contains(keyword) ||
                o.ComCustomer.CustomerName.Contains(keyword));
        }

        if (orderDate.HasValue)
        {
            query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
        }

        var totalItems = await query.CountAsync();
        var orders = await query
            .OrderBy(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new PaginationViewModel
        {
            Orders = orders,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            TotalItems = totalItems
        };

        return View(model);
    }

    public IActionResult Create()
    {
        var customers = _context.ComCustomers
            .Select(c => new SelectListItem
            {
                Value = c.ComCustomerId.ToString(),
                Text = c.CustomerName
            }).ToList();

        var model = new SoOrderViewModel
        {
            SoOrder = new SoOrderModel(),
            SoItems = new List<SoItemModel>(),
            Customers = customers
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SoOrderViewModel viewModel)
    {

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            var existingOrder = await _context.SoOrders
            .FirstOrDefaultAsync(o => o.OrderNo == viewModel.SoOrder.OrderNo);

            if (existingOrder != null)
            {
                return Json(new { success = false, message = "Order sudah ada, Ubah nama order agar tidak duplikat." });
            }

            var order = new SoOrderModel
            {
                OrderNo = viewModel.SoOrder.OrderNo,
                OrderDate = viewModel.SoOrder.OrderDate,
                ComCustomerId = viewModel.SoOrder.ComCustomerId,
                Address = viewModel.SoOrder.Address
            };

            _context.SoOrders.Add(order);
            await _context.SaveChangesAsync();

            if (viewModel.SoItems != null && viewModel.SoItems.Any())
            {
                foreach (var item in viewModel.SoItems)
                {
                    var soItem = new SoItemModel
                    {
                        SoOrderId = order.SoOrderId,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _context.SoItems.Add(soItem);
                }
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            return Json(new { success = true, message = "Sales order berhasil di buat." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Json(new { success = false, message = "An error occurred while creating the sales order." });
        }
    }

    [HttpPost]
    public IActionResult ExportToExcel()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("SalesOrders");

        worksheet.Cell(1, 1).Value = "No";
        worksheet.Cell(1, 2).Value = "Sales Order";
        worksheet.Cell(1, 3).Value = "Order Date";
        worksheet.Cell(1, 4).Value = "Customer";

        var orders = _context.SoOrders
            .Include(o => o.ComCustomer)
            .OrderBy(o => o.OrderDate)
            .ToList();

        for (int i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            worksheet.Cell(i + 2, 1).Value = i + 1;
            worksheet.Cell(i + 2, 2).Value = order.OrderNo;
            worksheet.Cell(i + 2, 3).Value = order.OrderDate.ToString("d/M/yyyy");
            worksheet.Cell(i + 2, 4).Value = order.ComCustomer.CustomerName;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "SalesOrders.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.SoOrders
            .Include(o => o.SoItems)
            .FirstOrDefaultAsync(o => o.SoOrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (order.SoItems != null && order.SoItems.Any())
            {
                _context.SoItems.RemoveRange(order.SoItems);
            }

            _context.SoOrders.Remove(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["SuccessMessage"] = "Sales order berhasil di hapus.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["ErrorMessage"] = "Gagal hapus sales order.";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Edit(long id)
    {
        var soOrder = _context.SoOrders
            .Include(s => s.ComCustomer)
            .Include(s => s.SoItems)
            .FirstOrDefault(s => s.SoOrderId == id);

        if (soOrder == null)
        {
            return NotFound();
        }

        var viewModel = new SoOrderViewModel
        {
            SoOrder = soOrder,
            Customers = GetCustomers()
        };

        return View(viewModel);
    }

    private List<SelectListItem> GetCustomers()
    {
        return _context.ComCustomers
            .Select(c => new SelectListItem
            {
                Value = c.ComCustomerId.ToString(),
                Text = c.CustomerName
            }).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] SoOrderModel model)
    {

        var existingOrder = await _context.SoOrders.FindAsync(model.SoOrderId);
        if (existingOrder == null)
        {
            return NotFound(new { success = false, message = "Order not found" });
        }

        existingOrder.OrderNo = model.OrderNo;
        existingOrder.OrderDate = model.OrderDate;
        existingOrder.ComCustomerId = model.ComCustomerId;
        existingOrder.Address = model.Address;

        _context.SoOrders.Update(existingOrder);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Order updated successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> SaveItem([FromBody] SoItemModel item)
    {

        try
        {
            var order = await _context.SoOrders
                .FirstOrDefaultAsync(o => o.SoOrderId == item.SoOrderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Sales order not found." });
            }

            var soItem = new SoItemModel
            {
                SoOrderId = item.SoOrderId,
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                Price = item.Price
            };

            _context.SoItems.Add(soItem);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Item saved successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred while saving the item." });
        }
    }
}
