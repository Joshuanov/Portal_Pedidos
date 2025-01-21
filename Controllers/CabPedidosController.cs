using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SistemaTGU.Data;
using SistemaTGU.Entities;
using SistemaTGU.Migrations;
using SistemaTGU.Models;
using SistemaTGU.Servicio;
using System;
using System.Linq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SistemaTGU.Controllers
{
    public class CabPedidosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConnectionErp _connectionErp;
        private readonly UserManager<IdentityUser> _userManager;

        public CabPedidosController(IConnectionErp connectionErp, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _connectionErp = connectionErp;
            _userManager = userManager; ;
        }
        [Authorize]
        public IActionResult Index()
        {            

            // Obtén el usuario actual
            var usuarioActual = _userManager.GetUserName(User); // Obtiene el email del usuario logeado
            var usuarioIdActual = _userManager.GetUserId(User); // Obtiene el ID del usuario logeado


            // Cargar cabeceras y detalles en la vista Index de CAbPedidos

             IQueryable<CabPedidos> cabecerasQuery = _context.CabPedidos
                /*.Where(cab => cab.EmpresaId == empresaId)*/ // Filtra por empresaId
                .Include(cab => cab.Estado); // Incluye la entidad relacionada

            // Aplica el filtro condicional
            if (usuarioActual != "correo@autorizado.cl")
            {
                cabecerasQuery = cabecerasQuery.Where(cab => cab.SolicitanteId == usuarioIdActual);
            }

            var viewModel  = cabecerasQuery
                .Select(cab => new CabeceraDetalleViewModel
                {
                    Id = cab.Id,
                    FechaSolicitud = cab.FechaSolicitud,
                    Solicitante = _context.Usuarios.FirstOrDefault(usr => usr.Id == cab.SolicitanteId).Nombre ?? "Usuario sin nombre",
                    EstadoId = cab.Estado.Descripcion, // Accede directamente a la descripción
                    NombreEmpresa = _connectionErp.ObtenerEmpresaPorId(cab.CecoEmpresaId).Nombre,
                    DenominacionCeco = _connectionErp.ObtenerCecoPorId(int.Parse(cab.IdCeco)).Denominacion ?? "Centro de costos no encontrado",
                    Detalles = _context.DetPedidos
                        .Where(det => det.CabPedidosId == cab.Id)
                        .Select(det => new PedidoDetalleViewModel
                        {
                            PedidoId = det.Id,
                            DescripcionPrenda = _connectionErp.ObtenerPrendasPorID(det.CodPrenda).DescripcionPrenda,                        
                            Cantidad = det.Cantidad,
                            CodPrenda = det.CodPrenda
                            //PrecioUnitario = det.PrecioUnitario,
                            //Total = det.Total
                        })
                        .ToList()
                })
                .ToList();

            // Pasar el correo a la vista
            ViewBag.UsuarioActual = usuarioActual;


            return View(viewModel);
        }

        public IActionResult EditarDetalle(int id)
        {
            var usuarioActual = _userManager.GetUserName(User); // Email del usuario logeado

            if (usuarioActual != "correo@autorizado.cl")
            {
                // Redirige a una página de error o Index
                return RedirectToAction("Index", "CabPedidos");
            }

            // Obtén el pedido por ID
            var cabecera = _context.CabPedidos
                .Include(c => c.Estado)
                .FirstOrDefault(c => c.Id == id);

            if (cabecera == null)
            {
                return NotFound("No se encontró el pedido.");
            }

            // Obtén datos adicionales
            var nombreEmpresa = _connectionErp.ObtenerEmpresaPorId(cabecera.CecoEmpresaId)?.Nombre ?? "Empresa no encontrada";
            var cecoNombre = _connectionErp.ObtenerCecoPorId(int.Parse(cabecera.IdCeco))?.Denominacion ?? "CeCo no encontrado";
            var Solicitante = _context.Usuarios.FirstOrDefault(usr => usr.Id == cabecera.SolicitanteId)?.Nombre ?? "Usuario sin nombre";

            // Recuperar todos los detalles relacionados al pedido con Id = id
            var detalles = _context.DetPedidos
                .Where(d => d.CabPedidosId == id) // Filtra los detalles del pedido
                .Select(det => new PedidoDetalleViewModel
                {
                    PedidoId = det.Id,
                    DescripcionPrenda = _connectionErp.ObtenerPrendasPorID(det.CodPrenda).DescripcionPrenda ?? "Prenda no encontrada",
                    Cantidad = det.Cantidad,
                    CodPrenda = det.CodPrenda
                })
                .ToList();

            if (!detalles.Any())
            {
                return NotFound("No se encontraron detalles para este pedido.");
            }

            // Llena el ViewModel
            var viewModel = new CabeceraDetalleViewModel
            {
                Id = cabecera.Id,
                FechaSolicitud = cabecera.FechaSolicitud,
                EstadoId = cabecera.Estado.Descripcion,
                Solicitante = Solicitante,
                NombreEmpresa = nombreEmpresa,
                DenominacionCeco = cecoNombre,
                Detalles = detalles
            };

            return View(viewModel);
        }


        [HttpPost] // Revisar esta guardando solo el primer elemento
        public IActionResult GuardarDetalles(List<PedidoDetalleViewModel> detalles)
        {
            if (detalles == null || !detalles.Any())
            {
                return BadRequest("No se recibieron detalles para guardar.");
            }

            foreach (var detalle in detalles)
            {
                var detPedido = _context.DetPedidos.FirstOrDefault(d => d.Id == detalle.PedidoId);
                if (detPedido != null)
                {
                    detPedido.Cantidad = detalle.Cantidad;
                }
            }

            _context.SaveChanges();

            return Json(new { success = true, message = "Los cambios se han guardado correctamente." });

        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListarPedidos()
        {
            var solicitanteId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(solicitanteId))
            {
                return Unauthorized();
            }

            var pedidos = await _context.CabPedidos
                .Where(p => p.SolicitanteId == solicitanteId)
                .Select(p => new CabeceraDetalleViewModel
                {
                    Id = p.Id,
                    FechaSolicitud = p.FechaSolicitud,
                    Solicitante = p.SolicitanteId, // Ajusta si necesitas otro campo para nombre                   
                    //EstadoId = p.EstadoId,
                   /* DenominacionCeco = p.Ceco.Denominacion,*/ // Relación con Ceco
                   /* NombreEmpresa = p.CecoEmpresa.Nombre,*/ // Relación con Empresa
                    Detalles = p.DetPedidos.Select(d => new PedidoDetalleViewModel
                    {
                        PedidoId = d.Id,                      
                        DescripcionPrenda = d.CodPrenda, // Ajusta según tu modelo
                        Cantidad = d.Cantidad
                    }).ToList()
                })
                .ToListAsync();

            return View(pedidos);
        }

    }






}
