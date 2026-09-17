using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит данные пользователя и его роль
	/// </summary>
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Login { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string PasswordHash { get; set; }

		[Required]
		public RoleEnum Role { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public List<Order> Orders { get; set; }

	}

	public enum RoleEnum
	{
		[Description("Администратор")]
		Admin,
		[Description("Пользователь")]
		User
	}
}
