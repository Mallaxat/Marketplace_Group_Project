using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace_Group_Project.Models
{
	/// <summary>
	/// Хранит характеристику конкретного товара
	/// </summary>
	public class ProductCharacteristic
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int ProductId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Value { get; set; }
		public UnitEnum? Unit { get; set; } = UnitEnum.None;
		public bool IsCommon { get; set; }

		public Product? Product { get; set; }
	}
	public enum UnitEnum
	{
		[Description("")]
		None,
		[Description("шт.")]
		Piece,
		[Description("рулон")]
		Roll,

		// Длина 
		[Description("мм")]
		Millimeter,
		[Description("см")]
		Centimeter,
		[Description("м")]
		Meter,
		[Description("дюйм")]
		Inch,

		// Масса
		[Description("мг")]
		Milligram,
		[Description("г")]
		Gram,
		[Description("кг")]
		Kilogram,
		[Description("т")]
		Ton,

		// Объём
		[Description("мл")]
		Milliliter,
		[Description("л")]
		Liter,
		[Description("м³")]
		CubicMeter,

		// Площадь
		[Description("см²")]
		SquareCentimeter,
		[Description("м²")]
		SquareMeter,

		[Description("МБ")]
		Megabyte,
		[Description("ГБ")]
		Gigabyte,
		[Description("ТБ")]
		Terrabyte,

		// Мощность / электрика
		[Description("Вт")]
		Watt,
		[Description("кВт")]
		Kilowatt,
		[Description("В")]
		Volt,
		[Description("А")]
		Ampere,
		[Description("А·ч")]
		AmpereHour,
		[Description("мА·ч")]
		MilliampereHour,

		// Производительность / расход
		[Description("л/мин")]
		LiterPerMinute,
		[Description("м³/ч")]
		CubicMeterPerHour,
		[Description("Вт·ч")]
		WattHour,
		[Description("кВт·ч")]
		KilowattHour,

		// Прочее
		[Description("%")]
		Percent,
		[Description("°C")]
		Celsius,
		[Description("об/мин")]
		Rpm,
		[Description("дБ")]
		Decibel
	}

}
