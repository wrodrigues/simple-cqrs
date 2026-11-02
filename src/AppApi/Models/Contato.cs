using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppApi.Models
{
    public class ContatoViewModel
    {
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
    }

    public class Contato
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
    }

    public class ContatoMapping : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome).IsRequired().HasColumnType("nvarchar(255)");

            builder.Property(x => x.Email).IsRequired(false).HasColumnType("nvarchar(255)");

            builder.Property(x => x.Telefone).IsRequired(false).HasColumnType("nvarchar(20)");

            builder.HasData(SeedData());
        }

        private IEnumerable<Contato> SeedData()
        {
            var data = new List<Contato>();

            //data.Add(new Contato { Id = 1, Nome = "Wellington" });

            // data.Add(new Contato { Id = 2, Nome = "William" });

            return data;
        }
    }

    public class AppDbContext : DbContext
    {
        protected AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("AppConnectionString"));
        }
    }
}