using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит основную информацию о товаре
	/// </summary>
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public decimal Price { get; set; }

		public int StockQuantity { get; set; }

		public CategoryEnum Category { get; set; }

		public string? ImagePath { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public List<ProductCharacteristic> Characteristics { get; set; }
		public List<Review> Reviews { get; set; }

		[Required]
		public int SellerId { get; set; }
		[ForeignKey("SellerId")]
		public User? Seller { get; set; }
	}

	public enum CategoryEnum
	{
		[Description("Одежда")]
		Clothes,
		[Description("Продукты питания")]
		Food,
		[Description("Косметика")]
		Cosmetic,
		[Description("Канцелярия")]
		OfficeSupplies,
		[Description("Техника")]
		Devices,
		[Description("Стройматериалы")]
		BuildingMaterials,
		[Description("Спортивные товары")]
		Sport,
		[Description("Детские товары")]
		ChildrenGoods,
		[Description("Обувь")]
		Shoes
	}
}
