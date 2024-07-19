using M3u8.Entities;
using Microsoft.EntityFrameworkCore;

namespace M3u8;

public partial class M3u8DbContext : DbContext
{
    public M3u8DbContext()
    {
        
    }

    public M3u8DbContext(DbContextOptions<M3u8DbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Data> Data { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder.UseSqlite($"Data Source={App.DatabasePath}");
        }
    }
}
