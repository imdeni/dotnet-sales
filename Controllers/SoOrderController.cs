using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

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

}
