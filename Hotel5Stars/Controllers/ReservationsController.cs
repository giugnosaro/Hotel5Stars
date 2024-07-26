using Hotel5Stars.Models;
using Hotel5Stars.Models.Dto;
using Hotel5Stars.Models.ViewModels;
using Hotel5Stars.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Hotel5Stars.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return View(reservations);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _reservationService.GetCustomersAsync();
            ViewBag.Rooms = await _reservationService.GetRoomsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await _reservationService.GetCustomersAsync();
                ViewBag.Rooms = await _reservationService.GetRoomsAsync();
                return View(reservation);
            }

            await _reservationService.AddReservationAsync(reservation);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SearchByFiscalCode(string fiscalCode)
        {
            var reservations = await _reservationService.GetReservationsByFiscalCodeAsync(fiscalCode);
            return View("Index", reservations);
        }

        public async Task<IActionResult> FullBoardCount()
        {
            int count = await _reservationService.GetFullBoardCountAsync();
            ViewBag.FullBoardCount = count;
            return View();
        }

        public IActionResult AddService(int reservationId)
        {
            ViewBag.ReservationId = reservationId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddService(AdditionalService service)
        {
            if (ModelState.IsValid)
            {
                await _reservationService.AddAdditionalServiceAsync(service);
                return RedirectToAction("Details", new { id = service.ReservationId });
            }

            ViewBag.ReservationId = service.ReservationId;
            return View(service);
        }

        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }
    }
}
