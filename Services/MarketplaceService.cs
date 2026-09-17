using Marketplace_Group_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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

		public IEnumerable<Product> SearchProducts(string request)
		{
			return context.Products.Where(p => EF.Functions.Like(p.Name, $"%{request}%")).ToList();
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

		public void AddCharacteristic(int productId, ProductCharacteristic characteristic)
		{
			var product = context.Products.Include(p=>p.Characteristics)
				.First(p=> p.Id == productId);
			if (product != null)
			{
				product.Characteristics.Add(characteristic);
				context.SaveChanges();
			}
		}

		public void AddCharacteristic(ProductCharacteristic characteristic)
		{
			var product = context.Products.Include(p => p.Characteristics)
				.First(p => p.Id == characteristic.ProductId);
			if (product != null)
			{
				context.ProductCharacteristics.Add(characteristic);
				context.SaveChanges();
			}
		}

		public void UpdateCharacteristic(ProductCharacteristic characteristic)
		{
			context.ProductCharacteristics.Update(characteristic);
			context.SaveChanges();
		}

		public void DeleteCharachteristic(int characteristicId)
		{
			var characteristic = context.ProductCharacteristics.Find(characteristicId);
			if (characteristic != null)
			{
				context.ProductCharacteristics.Remove(characteristic);
				context.SaveChanges();
			}
		}

		public IEnumerable<ProductCharacteristic> GetProductCharacteristics(int productId)
		{
			try
			{
				return context.ProductCharacteristics
					.Where(ch => ch.ProductId == productId).ToList();
			}
			catch
			{
				return Enumerable.Empty<ProductCharacteristic>();
			}
		}

		#endregion

		#region Методы корзины

		public IEnumerable<CartItem> GetCartItems(int userId)
		{
			try
			{
				return context.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToList(); 
			}
			catch
			{
				return Enumerable.Empty<CartItem>();
			}
		}

		public void AddToCart(CartItem cartItem)
		{
			Product productToAdd = context.Products.First(p => p.Id == cartItem.ProductId);
			if (productToAdd.StockQuantity == 0 | productToAdd.StockQuantity < cartItem.Quantity)
				throw new ArgumentException("Товар отсутствует в таком количестве.");

			context.CartItems.Add(cartItem);
			context.SaveChanges();
		}

		public void ChangeCartItemQauntity(int cartItemId, int newQuantity)
		{
			var cartItem = context.CartItems.Include(c=>c.Product).First(c=> c.Id == cartItemId);
			if (cartItem.Product!.StockQuantity >= newQuantity)
			{
				cartItem.Quantity = newQuantity;
				context.SaveChanges();
			}
			else
				throw new ArgumentException("Товар отсутствует в таком количестве.");
		}

		public void RemoveFromCart(int cartItemId)
		{
			var cartItem = context.CartItems.First(c => c.Id == cartItemId);
			context.CartItems.Remove(cartItem);
			context.SaveChanges();
		}

		public void ClearCart(int userId)
		{
			var cartItems = context.CartItems.Where(c=> c.UserId == userId).ToList();
			context.CartItems.RemoveRange(cartItems);
			context.SaveChanges();
		}

		public decimal GetCartTotal(int userId)
		{
			var cartItems = context.CartItems.Include(c=>c.Product)
				.Where(c => c.UserId == userId).ToList();

			decimal total = cartItems.Sum(c => (c.Product!.Price * c.Quantity));
			return total;
		}

		#endregion


		#region Методы заказов

		public void CreateOrder(Order order)
		{
			var products = context.Products
				.Where(p => order.OrderItems.Select(i => i.ProductId).Contains(p.Id))
				.ToDictionary(p => p.Id);

			var outOfStock = order.OrderItems
				.FirstOrDefault(i => products[i.ProductId].StockQuantity < i.Quantity);

			if (outOfStock != null)
				throw new ArgumentOutOfRangeException("Не удалось оформить заказ. Некторых товаров нет в наличии.");

			foreach (var item in order.OrderItems)
				products[item.ProductId].StockQuantity -= item.Quantity;

			context.Orders.Add(order);
			context.SaveChanges();
		}

		public IEnumerable<Order> GetUserOrders(int userId)
		{
			try
			{
				return context.Orders.Where(o => o.UserId == userId).ToList();
			}
			catch
			{
				return Enumerable.Empty<Order>();
			}
		}

		public IEnumerable<OrderItem> GetOrderItems(int orderId)
		{
			try
			{
				return context.OrderItems.Where(o => o.OrderId == orderId).ToList();
			}
			catch
			{
				return Enumerable.Empty<OrderItem>();
			}
		}

		public bool CheckStock(int productId, int currentQuantity)
		{
			var product = context.Products.First(p => p.Id == productId);

			return product.StockQuantity >= currentQuantity;
		}

		public void ChangeOrderStatus(Order order, StatusEnum status)
		{
			context.Orders.Attach(order);
			order.Status = status;
			context.SaveChanges();
		}

		#endregion


		#region Методы отзывов
		public IEnumerable<Review> GetProductReviews(int productId)
		{
			var product = context.Products.Include(p => p.Reviews)
				.First(p => p.Id == productId);

			return product.Reviews;
		}

		public void AddReview(int productId, int userId, Review review)
		{
			review.UserId = userId;
			review.ProductId = productId;

			context.Reviews.Add(review);
			context.SaveChanges();
		}
		
		public void AddReview(Review review)
		{
			context.Reviews.Add(review);
			context.SaveChanges();
		}

		public bool CanUserReview(int userId, int productId)
		{
			var userOrders = context.Orders.Include(o => o.OrderItems)
				.Where(o => o.UserId == userId && o.Status == StatusEnum.Received).ToList();

			foreach (var order in userOrders)
			{
				foreach(var item in order.OrderItems)
				{
					if(item.ProductId == productId)
					{
						return true;
					}
				}
			}
			return false;
		}

		public double GetAverageRating(int productId)
		{
			var reviews = GetProductReviews(productId);
			return reviews.Average(r => r.Rating);
		}

		#endregion

	}
}
