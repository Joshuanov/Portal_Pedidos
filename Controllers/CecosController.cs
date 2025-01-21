using Microsoft.AspNetCore.Mvc;
using SistemaTGU.Servicio;

namespace SistemaTGU.Controllers
{
    public class CecosController : Controller
    {
        private readonly IConnectionErp _connectionErp;

        public CecosController(IConnectionErp connectionErp)
        {
            _connectionErp = connectionErp;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Retorna la vista principal vacía o con datos iniciales, si corresponde
            return View();
        }

        //[HttpGet]
        //public IActionResult index()
        //{
        //    // Llama al servicio para obtener los datos
        //    var cecosEmpresa = _connectionErp.ObtenerCecosPorEmpresa();

        //    return View(cecosEmpresa); // Pasa los datos a la vista
        //}


        [HttpGet]
        public IActionResult ObtenerCecosPorEmpresa(int empresaId)
        {
            if (empresaId <= 0)
            {
                return BadRequest("ID de empresa inválido.");
            }
                        // Llama al servicio para obtener los CECOs asociados
            var cecos = _connectionErp.ObtenerCecosPorEmpresa(empresaId);

            if (cecos == null || !cecos.Any())
            {
                return NotFound("No se encontraron CECOs para esta empresa.");
            }

            // Retorna los CECOs en formato JSON
            return Json(cecos);
        }

        //[HttpGet]
        //public IActionResult ObtenerPrendasPorCeco(int cecoId)
        //{
        //    if (cecoId <= 0)
        //    {
        //        return BadRequest("ID de CECO inválido.");
        //    }

        //    // Obtener las prendas asociadas al CECO
        //    var prendas = _connectionErp.ObtenerPrendasPorEmpresa(cecoId);

        //    if (prendas == null || !prendas.Any())
        //    {
        //        return NotFound("No se encontraron prendas para este CECO.");
        //    }

        //    return Json(prendas);
        //}

    }
}



    

