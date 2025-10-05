using Microsoft.EntityFrameworkCore;
using HotelApi.Models;

namespace HotelApi.Data
{
    public class HotelContext : DbContext
    {
        public HotelContext(DbContextOptions<HotelContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; } = null!;

    }
}
