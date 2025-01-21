using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTGU.Data;
using SistemaTGU.Entities;
using SistemaTGU.Models;
using SistemaTGU.Servicio;
using System;


namespace SistemaTGU.Controllers
{

    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<SolicitudesController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConnectionErp _connectionErp;

        public SolicitudesController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<SolicitudesController> logger,
        IHttpContextAccessor httpContextAccessor, 
            IConnectionErp connectionErp)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _connectionErp = connectionErp ?? throw new ArgumentNullException(nameof(connectionErp));
        }

        //public IActionResult Detalles(int cabPedidosId)
        //{
        //    var detalles = _connectionErp.ObtenerDetallesPedido(cabPedidosId);

        //    if (detalles == null || detalles.Count == 0)
        //    {
        //        TempData["Mensaje"] = "No hay detalles para este pedido.";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View(detalles); // Enviar ViewModel a la vista
        //}



        // GET: SolicitudesController
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.CabPedidos
                .Include(s => s.Estado)
                .Include(s => s.Solicitante)
                .ToListAsync();

            // Convertimos las entidades a ViewModels




            var solicitudVar = solicitudes.Select(s => new SolicitudViewModel

            {
                Id = s.Id,
                FechaSolicitud = s.FechaSolicitud,
                EstadoId = s.EstadoId,
                Estado = s.Estado,
                SolicitanteId = s.SolicitanteId,
                Solicitante = s.Solicitante,


            }).ToList();

            return View(solicitudVar);
        }

        // Obtiene las solicitudes
        public async Task<IActionResult>ObtenerSolicitudes()
        {
        //    var solicitudes = await _context.CabPedidos
        //   .Include(s => s.Estado)
        //.Include(s => s.Solicitante)
        //.Include(s => s.cabPedidos.DetPedidos)
        //.Select(s => new SolicitudViewModel
        //{
            //Id = s.Id,
            //Solicitud = s.Solicitud,
            //FechaSolicitud = s.FechaSolicitud,
            //EstadoId = s.EstadoId,
            //Estado = s.Estado,
            //SolicitanteId = s.SolicitanteId,
            //Solicitante = s.Solicitante,
            //DetPedidos = s.cabPedidos.DetPedidos,
        //    Prendas = s.Prendas.Select(p => new PrendasViewModel
        //    {
        //        CodigoUsr = p.CodigoUsr,
        //        CantidadIngresada = p.CantidadIngresada
        //    }).ToList()
        //})
        //.ToListAsync();

            return View(/*solicitudes*/);
        }

        // GET: SolicitudesController/Create
        public async Task<IActionResult> CrearSolicitud()
        {
            string usuario = User.Identity.Name;


            //var usuarioActual = await _context.Usuarios.FirstOrDefaultAsync(u => u.UserName == usuario);
            var usuarioActual = await _userManager.FindByNameAsync(usuario);

            if (usuarioActual == null)
            {

                return RedirectToAction("Error", "Home");
            }

            var model = new SolicitudViewModel
            {

            };



            return View(model);
        }

        // POST: SolicitudesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSolicitud(SolicitudViewModel solicitudModel)
        {
            if (ModelState.IsValid)
            {
                // Crear la nueva solicitud
                var nuevaSolicitud = new CabPedidos
                {
                    FechaSolicitud = solicitudModel.FechaSolicitud,
                    EstadoId = solicitudModel.EstadoId,
                    SolicitanteId = solicitudModel.SolicitanteId
                };
                // Guardar la solicitud en la base de datos
                _context.CabPedidos.Add(nuevaSolicitud);
                await _context.SaveChangesAsync();

                // Redirigir a la vista de lista (Index)
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, devolver a la vista de creación
            return View(solicitudModel);
        }


        // GET: SolicitudesController/Edit/5
        public async Task<IActionResult> Editar(int id)
        {
                    var solicitud = await _context.CabPedidos
            .Include(s => s.Estado)
            .Include(s => s.Solicitante)
            .Include(s => s.DetPedidos) // Incluir la colección DetPedidos
            .FirstOrDefaultAsync(s => s.Id == id);


            if (solicitud == null)
            {
                return NotFound();
            }

            var solicitudVar2 = new SolicitudViewModel
            {
                Id = solicitud.Id,
                FechaSolicitud = solicitud.FechaSolicitud,
                EstadoId = solicitud.EstadoId,
                Estado = solicitud.Estado,
                SolicitanteId = solicitud.SolicitanteId,
                Solicitante = solicitud.Solicitante,
                DetPedidos = solicitud.DetPedidos != null ? new HashSet<DetPedidos>(solicitud.DetPedidos) : new HashSet<DetPedidos>(),
                Estados = await _context.Estados.ToListAsync()
            };

            return View("EditarSolicitud", solicitudVar2);
        }


        // POST: SolicitudesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, SolicitudViewModel solicitud)
        {
            if (id != solicitud.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var solicitudExistente = await _context.CabPedidos.FirstOrDefaultAsync(s => s.Id == id);

                if (solicitudExistente != null)
                {
                    solicitudExistente.FechaSolicitud = solicitud.FechaSolicitud;
                    solicitudExistente.EstadoId = solicitud.EstadoId;
                    solicitudExistente.SolicitanteId = solicitud.SolicitanteId;


                    _context.Update(solicitudExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)); // Redirigir a la vista de lista
                }
            }


            solicitud.Estados = await _context.Estados.ToListAsync();


            return View("EditarSolicitud", solicitud);
        }

        // GET: SolicitudesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }


        // POST: SolicitudesController/Delete/5
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