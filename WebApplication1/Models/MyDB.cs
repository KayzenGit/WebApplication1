using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class MyDB:DbContext
    {

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public MyDB() {
           
        }
        public MyDB(DbContextOptions<MyDB> options):base(options) {  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //-------- IEntityTypeConfiguration For Cleaner Fluent API Mappings --------------
            new EmployeeEntityTypeConfiguration().Configure(modelBuilder.Entity<Employee>());
            new SalaryEntityTypeConfiguration().Configure(modelBuilder.Entity<Salary>());
            //--------------------------------------------------------------------------------
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies(false);
            
            //string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0] + @"DataBase\";
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
           
            string cnnStr = configuration.GetConnectionString("DefaultConnection").Replace("[DataDirectory]", Directory.GetCurrentDirectory());
            optionsBuilder.UseSqlServer(cnnStr);

            Environment.SetEnvironmentVariable("DB_CNN_MSSQL", cnnStr);
        }

    }

    public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> modelBuilder)
        {
            //modelBuilder
            //    .HasMany(e => e.Salaries)
            //    .WithOne(e => e.Employee)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .IsRequired();

            modelBuilder.Property(e => e.EmployeeID)
                            .HasColumnName("EmployeeID")
                            .HasColumnType("int")
                            .UseIdentityColumn(seed: 1, increment: 1)
                            .IsRequired();
            modelBuilder.Property(e=>e.EmployeeFirstName)
                            .HasColumnName("EmployeeFirstName")
                            .HasColumnType("nvarchar (100)")
                            .HasMaxLength(100)
                            .IsRequired();
            modelBuilder.Property(e => e.EmployeeLastName)
                            .HasColumnName("EmployeeLastName")
                            .HasColumnType("nvarchar (150)")
                            .HasMaxLength(150);

        }
    }
    public class SalaryEntityTypeConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> modelBuilder)
        {
            modelBuilder
                .HasOne(e => e.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(e => e.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            modelBuilder.Property(e => e.SalaryID)
                            .HasColumnName("SalaryID")
                            .HasColumnType("int")
                            .UseIdentityColumn(seed: 1, increment: 1)
                            .IsRequired();
            modelBuilder.Property(e => e.EmployeeID)
                           .HasColumnName("EmployeeID")
                           .HasColumnType("int")
                           .IsRequired();
            modelBuilder.Property(e => e.MonthNumber)
                           .HasColumnName("MonthNumber")
                           .HasColumnType("int")
                           .HasDefaultValue(0)
                           .IsRequired();
            modelBuilder.Property(e => e.SalaryAmount)
                           .HasColumnName("SalaryAmount")
                           .HasColumnType("bigint")
                           .HasDefaultValue(0)
                           .IsRequired();
        }
    }

}

