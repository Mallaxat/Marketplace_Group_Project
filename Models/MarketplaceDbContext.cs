using Marketplace_Group_Project.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	public class MarketplaceDbContext : DbContext
	{

		public DbSet<User> Users { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductCharacteristic> ProductCharacteristics { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=localhost;Database=MarketplaceDB;Trusted_Connection=True;TrustServerCertificate=True;")
				.UseSeeding((context, _) =>
				{
					// Этот код выполнится после Migrate(), если данных ещё нет
					SeedData.Seed(context);
				})
				.UseAsyncSeeding(async (context, _, cancellationToken) =>
				{
					// Этот код выполнится после MigrateAsync(), если данных ещё нет
					await SeedData.SeedAsync(context);
				});
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Login)
				.IsUnique();

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
		}
	}
}
