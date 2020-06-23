using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MSensis.Models
{
    public class MSensisContext : IdentityDbContext<User>
    {
        public MSensisContext()
        {
        }

        public MSensisContext(DbContextOptions<MSensisContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Pdf> Pdfs { get; set; }
        public DbSet<Invoice_Product> Invoice_Products { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
                 optionsBuilder
                    //Log parameter values
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Pdf>().HasKey(uc => new { uc.Id, uc.InvoiceId});

            builder.Entity<Pdf>()
            .HasOne(c => c.User)
            .WithMany()
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Invoice_Product>().HasKey(sc => new { sc.Id, sc.InvoiceId });

            builder.Entity<Invoice_Product>()
            .HasOne(l => l.Invoice)
            .WithMany(t => t.Invoice_Products)
            .HasForeignKey(sc => sc.InvoiceId);


        }
    }
}
