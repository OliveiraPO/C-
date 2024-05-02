using Program.Models;
using Microsoft.EntityFrameworkCore;

namespace Program.Data{
    public class AppDbContext : DbContext{
        public DbSet<Pedido> Pedidos{get;set;}
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("DataSource=program.sqlite;Cache=Shared");
    }
}