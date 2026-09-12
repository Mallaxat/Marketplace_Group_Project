using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит информацию об оформленном заказе
	/// </summary>
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Required]
		public StatusEnum Status { get; set; }

		public decimal TotalPrice { get; set; }

		public User? User { get; set; }
		public List<OrderItem> OrderItems { get; set; }
	}

	public enum StatusEnum
	{
		[Description("Создан")]
		Created,
		[Description("Выполняется")]
		InProgress,
		[Description("Доставлен")]
		Done,
		[Description("Перенесён")]
		Postponed,
		[Description("Отменён")]
		Canceled
	}
}
