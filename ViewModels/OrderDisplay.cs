using Marketplace_Group_Project.Models;
using System.Collections.Generic;

namespace Marketplace_Group_Project.ViewModels
{
    /// <summary>
    /// Обёртка над Order для отображения в UI.
    /// Хранит оригинальный заказ + строку с названиями товаров.
    /// </summary>
    public class OrderDisplay
    {
        public Order Order { get; }
        public string ProductsSummary { get; }

        public OrderDisplay(Order order, string productsSummary)
        {
            Order = order;
            ProductsSummary = productsSummary;
        }

        // Прокидываем свойства Order наружу — чтобы XAML-привязки работали
        public int Id => Order.Id;
        public int UserId => Order.UserId;
        public System.DateTime CreatedAt => Order.CreatedAt;
        public StatusEnum Status => Order.Status;
        public decimal TotalPrice => Order.TotalPrice;
    }
}