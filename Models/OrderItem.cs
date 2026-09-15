using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит отдельный товар внутри заказа
	/// </summary>
	public class OrderItem
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }

		[Required]
		public int ProductId { get; set; }

		public int Quantity { get; set; }

		public decimal UnitPrice { get; set; }

		public Order? Order { get; set; }
		public Product? Product { get; set; }
	}
}
