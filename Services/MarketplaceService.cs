using Marketplace_Group_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Marketplace_Group_Project.Services
{
	public class MarketplaceService : IDisposable
	{

		#region Dispose
		private bool disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			context.Dispose();
			disposed = true;
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~MarketplaceService()
		{
			Dispose(false);
		}

		#endregion

		private readonly MarketplaceDbContext context = new MarketplaceDbContext();

		#region Методы товаров
		public IEnumerable<Product> GetProducts()
		{
			try
			{
				return context.Products.ToList();
			}
			catch
			{
				return Enumerable.Empty<Product>();
			}
		}

		public Product? GetProductById(int productId)
		{
			try
			{
				return context.Products.FirstOrDefault(p => p.Id == productId);
			}
			catch
			{
				return null;
			}
		}

		public IEnumerable<Product> SearchProducts()
		{
			throw new NotImplementedException(); //todo
		}

		public void AddProduct(Product product)
		{
			context.Products.Add(product);
			context.SaveChanges();
		}

		public void UpdateProduct(Product product)
		{
			context.Products.Update(product);
			context.SaveChanges();
		}

		public void DeactivateProduct(int productId)
		{
			var product = context.Products.Find(productId);
			if(product != null)
			{   
				product.IsActive = false;
				context.SaveChanges();
			}
		}

		#endregion

		#region Методы характеристик


		#endregion

		#region Методы корзины


		#endregion


		#region Методы заказов


		#endregion


		#region Методы отзывов


		#endregion

	}
}
