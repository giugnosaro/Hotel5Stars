using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hotel5Stars.Models.Dto;
using Hotel5Stars.Services;

namespace Hotel5Stars.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return View(customers); // Passa la lista dei clienti alla vista
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            // Genera il codice fiscale
            string generatedFiscalCode = FiscalCodeGenerator.Generate(
                customer.LastName,
                customer.FirstName,
                DateTime.Now, // Sostituisci con la data di nascita reale se disponibile
                "M", // Sostituisci con il sesso reale se disponibile
                "A001" // Codice della città, sostituisci con il codice reale
            );

            customer.FiscalCode = generatedFiscalCode;

            await _customerService.AddCustomerAsync(customer);

            return RedirectToAction("Index");
        }
    }
}
