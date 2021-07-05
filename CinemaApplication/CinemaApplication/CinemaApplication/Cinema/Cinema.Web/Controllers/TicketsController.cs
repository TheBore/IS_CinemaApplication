using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Domain.DTO;
using Cinema.Domain.Enum;
using Cinema.Service.Interface;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Web.Controllers
{
    public class TicketsController : Controller
    {

        private readonly ITicketService _ticketService;
        private readonly UserManager<CinemaUser> userManager;

        public TicketsController(ITicketService ticketService, UserManager<CinemaUser> userManager)
        {
            _ticketService = ticketService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync(DateTime dateTicket)
        {
            ViewData["genres"] = new SelectList(Enum.GetValues(typeof(Genre)));

            DateTime dt3 = new DateTime(0001, 1, 1);

            if (dateTicket != null && dateTicket != dt3)
            {
                var allTicketsFiltered = _ticketService.FilterTickets(dateTicket);
                return View(allTicketsFiltered);
            }

            var allTickets = _ticketService.GetAllTickets();
            return View(allTickets);
        }

        public IActionResult Create()
        {
            ViewData["genres"] = new SelectList(Enum.GetValues(typeof(Genre)));
            return View();
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,movieTitle,MovieGenre,Price,Date")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._ticketService.CreateNewTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            ViewData["genres"] = new SelectList(Enum.GetValues(typeof(Genre)));

            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,movieTitle,MovieGenre,Price,Date")] Ticket ticket)
        {
            //Presentation for some content - so we write them in the controller
            if (id != ticket.Id)
            {
                return NotFound();
            }
            //not found - message - view 

            if (ModelState.IsValid) //where to redirect
            {
                try
                { 
                    this._ticketService.UpdateExistingTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForTicket(id) != null;
        }

        public IActionResult AddToShoppingCart(Guid? id)
        {
            var model = this._ticketService.GetShoppingCartInfo(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToShoppingCart([Bind("TicketId", "Quantity")] AddToShoppingCartDto item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var result = this._ticketService.AddToShoppingCart(item, userId);
          
            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(item);
        }

        [HttpGet]
        public async Task<FileContentResult> ExportAllTickets(string? genre)
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Movie Title";
                worksheet.Cell(1, 3).Value = "Movie Category";
                worksheet.Cell(1, 4).Value = "Price";

                var user = await userManager.GetUserAsync(HttpContext.User);
                var result = new List<Ticket>();
                if (genre == null || genre.Length == 0 || genre.Equals("NoCategory"))
                {
                    result= _ticketService.GetAllTickets(); 
                }
                else
                {
                    result = _ticketService.GetAllTickets().Where(t => t.MovieGenre.ToString().Equals(genre)).ToList();
                }
                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.movieTitle;
                    worksheet.Cell(i + 1, 3).Value = item.MovieGenre;
                    worksheet.Cell(i + 1, 4).Value = item.Price.ToString();

                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }
        }

    }
}
