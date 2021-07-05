using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Domain.Enum;
using Cinema.Service.Interface;
using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly UserManager<CinemaUser> userManager;
        public OrderController(IOrderService orderService, UserManager<CinemaUser> userManager)
        {
            this.orderService = orderService;
            this.userManager = userManager;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            ViewData["genres"] = new SelectList(Enum.GetValues(typeof(Genre)));

            var user = await userManager.GetUserAsync(HttpContext.User);
            var isAdmin = userManager.IsInRoleAsync(user, "Administrator").Result;
            var orders = orderService.getAllOrders(user);
            if (!isAdmin)
            {
                orders = orders.Where(o => o.UserId == user.Id).ToList();
            }
            return View(orders);
        }

        
        [HttpGet]
        public IActionResult Details(Guid? id)
        {
            var result = orderService.getOrderDetails(id);
            return View(result);
        }
        [HttpGet]
        public async Task<FileContentResult> ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Costumer Email";

                var user = await userManager.GetUserAsync(HttpContext.User);

                var result = orderService.getAllOrders(user);

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.Email;

                    for (int p = 0; p < item.TicketInOrders.Count(); p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Ticket-" + (p + 1);
                        worksheet.Cell(i + 1, p + 3).Value = item.TicketInOrders.ElementAt(p).OrderedTicket.movieTitle;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }
        }

        public FileContentResult CreateInvoice(Guid id)
        {
            var result = orderService.getOrderDetails(id);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);


            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.User.UserName);

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in result.TicketInOrders)
            {
                totalPrice += item.Quantity * item.OrderedTicket.Price;
                sb.AppendLine(item.OrderedTicket.movieTitle + " with quantity of: " + item.Quantity + " and price of: " + item.OrderedTicket.Price + "$");
            }


            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "$");


            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}