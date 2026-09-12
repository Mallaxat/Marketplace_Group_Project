using Marketplace_Group_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace_Group_Project.Data
{
	/// <summary>
	/// Класс для заполнения БД тестовыми данными
	/// </summary>
	public static class SeedData
	{
		public static void Seed(DbContext ctx)
		{
			var context = (MarketplaceDbContext)ctx;

			//user-ы на будущее, когда с методом шифрования определимся
			//if(!context.Users.Any())
			//{
			//	await context.Users.AddRangeAsync(
			//		new User
			//		{
			//			Login = "admin",
			//			Email = "test_email@.com",
			//			PasswordHash = "", //закинуть в зависимости от пароля  способа шифрования
			//			Role = RoleEnum.Admin,
			//		},
			//		new User
			//		{
			//			Login = "guest",
			//			Email = "guest_email@.com",
			//			PasswordHash = "", //закинуть в зависимости от пароля  способа шифрования
			//			Role = RoleEnum.User,
			//		}
			//	);
			//}

			//await context.SaveChangesAsync();

			if (!context.Products.Any())
			{
				context.Products.AddRange(
					new Product
					{
						Name = "IPhone 17",
						Description = "Apple IPhone 17 белый",
						Price = 89_999M,
						StockQuantity = 37,
						Category = CategoryEnum.Devices,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Диагональ экрана",
								Value = "6.3",
								Unit = UnitEnum.Inch
							},
							new ProductCharacteristic
							{
								Name = "Объем оперативной памяти",
								Value = "8",
								Unit = UnitEnum.Gigabyte
							}
						}
					},

					new Product
					{
						Name = "Монитор MSI Pro",
						Description = @"Монитор оборудован безрамочным дизайном, что обеспечивает
										реалистичность изображения и концентрацию на выполнении задач",
						Price = 10_999M,
						StockQuantity = 50,
						Category = CategoryEnum.Devices,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Бренд",
								Value = "MSI",
								Unit = UnitEnum.None
							},
							new ProductCharacteristic
							{
								Name = "Диагональ экрана",
								Value = "27",
								Unit = UnitEnum.Inch
							},
							new ProductCharacteristic
							{
								Name = "Макс. разрешение",
								Value = "1920x1080",
								Unit = UnitEnum.None
							}
						}
					},

					new Product
					{
						Name = "Кроссовки мужские Fila Neon",
						Description = @"Кроссовки мужские Fila Neon помогут получить максимум от
										занятий в спортивном зале",
						Price = 7_649M,
						StockQuantity = 40,
						Category = CategoryEnum.Sport,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Размер",
								Value = "42",
								Unit = UnitEnum.None
							},
							new ProductCharacteristic
							{
								Name = "Длина стопы",
								Value = "27.5",
								Unit = UnitEnum.Centimeter
							}
						}
					},

					new Product
					{
						Name = "Набор шариковых ручек Pens",
						Description = "Ручки в полупрозрачном корпусе с колпачком.",
						Price = 540M,
						StockQuantity = 137,
						Category = CategoryEnum.OfficeSupplies,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Вид ручки",
								Value = "Шарикова"
							},
							new ProductCharacteristic
							{
								Name = "Цвет чернил",
								Value = "Синий"
							},
							new ProductCharacteristic
							{
								Name = "Количество ручек",
								Value = "50",
								Unit = UnitEnum.Piece
							},
						}
					}

				);
			}

			context.SaveChanges();
		}

		public static async Task SeedAsync(DbContext ctx)
		{
			var context = (MarketplaceDbContext)ctx;

			//user-ы на будущее, когда с методом шифрования определимся
			//if(!context.Users.Any())
			//{
			//	await context.Users.AddRangeAsync(
			//		new User
			//		{
			//			Login = "admin",
			//			Email = "test_email@.com",
			//			PasswordHash = "", //закинуть в зависимости от пароля  способа шифрования
			//			Role = RoleEnum.Admin,
			//		},
			//		new User
			//		{
			//			Login = "guest",
			//			Email = "guest_email@.com",
			//			PasswordHash = "", //закинуть в зависимости от пароля  способа шифрования
			//			Role = RoleEnum.User,
			//		}
			//	);
			//}

			//await context.SaveChangesAsync();

			if (!context.Products.Any())
			{
				await context.Products.AddRangeAsync(
					new Product
					{
						Name = "IPhone 17",
						Description = "Apple IPhone 17 белый",
						Price = 89_999M,
						StockQuantity = 37,
						Category = CategoryEnum.Devices,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Диагональ экрана",
								Value = "6.3",
								Unit = UnitEnum.Inch
							},
							new ProductCharacteristic
							{
								Name = "Объем оперативной памяти",
								Value = "8",
								Unit = UnitEnum.Gigabyte
							}
						}
					},

					new Product
					{
						Name = "Монитор MSI Pro",
						Description = @"Монитор оборудован безрамочным дизайном, что обеспечивает
										реалистичность изображения и концентрацию на выполнении задач",
						Price = 10_999M,
						StockQuantity = 50,
						Category = CategoryEnum.Devices,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Бренд",
								Value = "MSI",
								Unit = UnitEnum.None
							},
							new ProductCharacteristic
							{
								Name = "Диагональ экрана",
								Value = "27",
								Unit = UnitEnum.Inch
							},
							new ProductCharacteristic
							{
								Name = "Макс. разрешение",
								Value = "1920x1080",
								Unit = UnitEnum.None
							}
						}
					},

					new Product
					{
						Name = "Кроссовки мужские Fila Neon",
						Description = @"Кроссовки мужские Fila Neon помогут получить максимум от
										занятий в спортивном зале",
						Price = 7_649M,
						StockQuantity = 40,
						Category = CategoryEnum.Sport,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Размер",
								Value = "42",
								Unit = UnitEnum.None
							},
							new ProductCharacteristic
							{
								Name = "Длина стопы",
								Value = "27.5",
								Unit = UnitEnum.Centimeter
							}
						}
					},

					new Product
					{
						Name = "Набор шариковых ручек Pens",
						Description = "Ручки в полупрозрачном корпусе с колпачком.",
						Price = 540M,
						StockQuantity = 137,
						Category = CategoryEnum.OfficeSupplies,

						Characteristics = new()
						{
							new ProductCharacteristic
							{
								Name = "Вид ручки",
								Value = "Шарикова"
							},
							new ProductCharacteristic
							{
								Name = "Цвет чернил",
								Value = "Синий"
							},
							new ProductCharacteristic
							{
								Name = "Количество ручек",
								Value = "50",
								Unit = UnitEnum.Piece
							},
						}
					}

				);
			}

			await context.SaveChangesAsync();
		}
	}
}
