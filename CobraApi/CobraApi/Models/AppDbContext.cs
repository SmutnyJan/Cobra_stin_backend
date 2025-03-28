using Microsoft.EntityFrameworkCore;

namespace CobraApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<FavoriteTicker> FavoriteTickers { get; set; } = null!;
        public DbSet<StockData> StockDatas { get; set; } = null!;
    }
}