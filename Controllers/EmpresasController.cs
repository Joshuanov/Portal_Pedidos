using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaTGU.Data;
using SistemaTGU.Entities;
using SistemaTGU.Models;
using SistemaTGU.Servicio;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace SistemaTGU.Controllers
{
    //[Authorize]
    public class EmpresasController(IConnectionErp connectionErp, UserManager<IdentityUser> userManager, ApplicationDbContext context) : Controller
    {
        private readonly IConnectionErp _connectionErp = connectionErp;
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;


        // GET: EmpresasController
        public IActionResult Index(string data = null)
        {
            var pedido = new PedidoViewModel
            {
                Empresa = _connectionErp.ObtenerEmpresas() ?? new List<EmpresaViewModel>(),
                Prendas = new List<PrendasViewModel>() // Inicializa como lista vacía // Evitar valores nulos
            };

            // Si data contiene información (volviendo desde ConfirmarPedido)
            if (!string.IsNullOrEmpty(data))
            {
                // Usa JsonConvert
                var payload = JsonConvert.DeserializeObject<PedidoViewModel>(data);
                pedido.EmpresaId = payload.EmpresaId;
                pedido.CecoId = payload.CecoId;
                pedido.Prendas = payload.Prendas ?? new List<PrendasViewModel>();
            }

            return View(pedido);
        }

        [HttpGet]
        public IActionResult BuscarPorEmpresa(int empresaId)
        {
            if (empresaId <= 0)
            {
                return BadRequest("ID de empresa inválido.");
            }

            var empresa = _connectionErp.ObtenerEmpresas()
                .FirstOrDefault(e => e.Id == empresaId);

            if (empresa == null)
            {
                return NotFound("No se encontró la empresa con el ID proporcionado.");
            }

            return Json(empresa); // Retorna los datos de la empresa en formato JSON
        }



        // GET: EmpresasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmpresasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpresasController/Create
        [HttpPost]
        [Route("Empresas/GuardarPedido")]
      
        
        public async Task<IActionResult> GuardarPedido([FromBody] PedidoViewModel pedidos)
        {
            if (pedidos == null)
            {
                return BadRequest(new { message = "No se enviaron pedidos válidos." });
            }

            // Obtener el usuario autenticado (Solicitante)
            var solicitanteId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(solicitanteId))
            {
                return Unauthorized(new { status = "error", message = "Usuario no autenticado." });
            }

            var DetPedidosSelect = pedidos.Prendas
           .Where(prenda => prenda.CantidadIngresada > 0) // Filtrar prendas con cantidad > 0
           .Select(prenda => new DetPedidos
           {
               CodPrenda = prenda.CodigoUsr,
               Cantidad = prenda.CantidadIngresada,
               //PrecioUnitario = 100,
               //Total = prenda.Cantidad * 100
           })
           .ToHashSet();

            // Crear la cabecera del pedido
            var nuevaCabecera = new CabPedidos
            {
                FechaSolicitud = DateTime.Now,
                IdCeco = pedidos.CecoId,
                CecoEmpresaId = pedidos.EmpresaId,
                EstadoId = 1, // Estado por defecto
                SolicitanteId = solicitanteId,
                DetPedidos = DetPedidosSelect

            };

            try
            {
                // Guardar en la base de datos
                //context.CabPedidos.Add(nuevaCabecera);
                //await context.SaveChangesAsync();
                //return Ok(new { message = "Pedido guardado correctamente." });

                context.CabPedidos.Add(nuevaCabecera);
                await context.SaveChangesAsync();
                return Ok(new { status = "success", message = "El pedido se ha guardado correctamente." });
            }
            catch (Exception ex)
            {
                //Console.Write(ex.ToString());
                //return BadRequest(new { message = "No se enviaron pedidos válidos." });
                return StatusCode(500, new { status = "error", message = "Error interno del servidor.", details = ex.Message });
            }

        }

        [HttpGet]
        [Authorize] // Asegura que solo usuarios autenticados accedan a este método
        public async Task<IActionResult> ListarPedidos()
        {
            // Obtener el usuario autenticado
            var solicitanteId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(solicitanteId))
            {
                return Unauthorized(new { status = "error", message = "Usuario no autenticado." });
            }

            // Recuperar pedidos del usuario autenticado
            var pedidos = await context.CabPedidos
                .Where(p => p.SolicitanteId == solicitanteId)
                .Select(p => new
                {
                    p.Id,
                    p.FechaSolicitud,
                    Empresa = p.CecoEmpresaId,
                    CentroDeCosto = p.IdCeco,
                    Estado = p.EstadoId
                })
                .ToListAsync();

            return Ok(new
            {
                status = "success",
                pedidos
            });
        }



        // GET: EmpresasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmpresasController/Edit/5
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

        // GET: EmpresasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmpresasController/Delete/5
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

        public IActionResult ConfirmarPedido(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return RedirectToAction("Index", "Empresas"); // Redirige si no hay datos
            }

            try
            {
                // Decodifica los datos desde la URL
                var jsonData = Uri.UnescapeDataString(data);
                var pedido = JsonConvert.DeserializeObject<PedidoViewModel>(jsonData); // Convierte a PedidoViewModel

                // Pasa los datos a la vista
                return View(pedido);
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de datos corruptos o problemas
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Empresas");
            }
        }

    }
}
