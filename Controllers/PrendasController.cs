using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaTGU.Servicio;

namespace SistemaTGU.Controllers
{
    public class PrendasController : Controller
    {

        private readonly IConnectionErp _connectionErp;

        public PrendasController(IConnectionErp connectionErp)
        {
            _connectionErp = connectionErp;
        }


        [HttpGet]
        public IActionResult ObtenerPrendasPorEmpresa(int empresaId)
        {
            if (empresaId <= 0)
            {
                return BadRequest("ID de empresa inválido.");
            }

            var prendas = _connectionErp.ObtenerPrendasPorEmpresa(empresaId);

            if (prendas == null || !prendas.Any())
            {
                return NotFound("No se encontraron prendas para esta empresa.");
            }

            return Json(prendas);
        }


        // GET: PrendasController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PrendasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PrendasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrendasController/Create
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

        // GET: PrendasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PrendasController/Edit/5
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

        // GET: PrendasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PrendasController/Delete/5
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
