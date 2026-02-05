using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StajerManager.Models
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<StajerModel> Stajers { get; set; }
        public DbSet<DepartmanModel> Departmans { get; set; }
        public DbSet<UniversiteModel> Universiteler { get; set; }
        public DbSet<BolumModel> Bolumler { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("server=DESKTOP-KST05H4\\SQLEXPRESS;database=StajerManager;integrated security=true;TrustServerCertificate=true;");
        //    }
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<StajerModel>()
                .HasOne(s => s.Departman)
                .WithMany(d => d.Stajers)
                .HasForeignKey(s => s.DepartmanID)
                .OnDelete(DeleteBehavior.Cascade) 
                .IsRequired();

            modelBuilder.Entity<DepartmanModel>()
                .HasIndex(d => d.DepartmanAdi)                   
                .IsUnique()                                    
                .HasDatabaseName("IX_Departmans_DepartmanAdi");

            modelBuilder.Entity<StajerModel>()
                .HasIndex(s => s.Email)                       
                .IsUnique()                                      
                .HasDatabaseName("IX_Stajers_Email");

            // Üniversite - Stajer ilişkisi (1:N)
            modelBuilder.Entity<StajerModel>()
                .HasOne(s => s.Universite)
                .WithMany(u => u.Stajers)
                .HasForeignKey(s => s.UniversiteID)
                .OnDelete(DeleteBehavior.NoAction);

            // Bölüm - Stajer ilişkisi (1:N)
            modelBuilder.Entity<StajerModel>()
                .HasOne(s => s.Bolum)
                .WithMany(b => b.Stajers)
                .HasForeignKey(s => s.BolumID)
                .OnDelete(DeleteBehavior.NoAction);

            // Üniversite adı unique index
            modelBuilder.Entity<UniversiteModel>()
                .HasIndex(u => u.UniversiteAdi)
                .IsUnique()
                .HasDatabaseName("IX_Universiteler_UniversiteAdi");

            // Üniversite - Bölüm ilişkisi (1:N) - YENİ EKLENECEK
            modelBuilder.Entity<BolumModel>()
                .HasOne(b => b.Universite)
                .WithMany(u => u.Bolumler)
                .HasForeignKey(b => b.UniversiteID)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();


            // Bölüm adı unique index - Üniversite bazında (aynı üniversitede aynı bölüm adı olamaz)
            modelBuilder.Entity<BolumModel>()
                .HasIndex(b => new { b.BolumAdi, b.UniversiteID })
                .IsUnique()
                .HasDatabaseName("IX_Bolumler_BolumAdi_UniversiteID");
        }
    }
}
    