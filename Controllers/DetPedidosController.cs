using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SistemaTGU.Data;
using SistemaTGU.Models;

namespace SistemaTGU.Controllers
{
    public class DetPedidosController : Controller
    {

        private readonly ApplicationDbContext _context;

        public DetPedidosController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            // Convertir los datos de la entidad al ViewModel
            var viewModel = _context.DetPedidos
                .Select(dp => new PedidoDetalleViewModel
                {
                    PedidoId = dp.Id,
                    CodPrenda = dp.CodPrenda,
                    Cantidad = dp.Cantidad,
                    //PrecioUnitario = dp.PrecioUnitario,
                    //Total = dp.Total
                })
                .ToList();
            return View(viewModel);
        }


        // GET: DetPedidosController

        // GET: DetPedidosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DetPedidosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetPedidosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DetPedidosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DetPedidosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DetPedidosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DetPedidosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
