using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит отзыв пользователя о товаре
	/// </summary>
	public class Review
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }
		
		[Required]
		public int ProductId { get; set; }

		[Required]
		[Range(1,5)]
		public int Rating { get; set; }

		public string? Text { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public User? User { get; set; }
		public Product? Product { get; set; }
	}
}
