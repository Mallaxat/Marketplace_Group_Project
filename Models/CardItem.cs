using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит один товар в корзине пользователя
	/// </summary>
	public class CardItem
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int ProductId { get; set; }

		public int Quantity { get; set; }

		public User? User { get; set; }
		public Product? Product { get; set; }
	}
}
