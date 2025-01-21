using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTGU.Entities;
using SistemaTGU.Models;

namespace SistemaTGU.Data
{
    public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext(options)
    {
        public DbSet<CabPedidos> CabPedidos { get; set; }
        public DbSet<DetPedidos> DetPedidos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }


    }
}
