using Marketplace_Group_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Marketplace_Group_Project
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Создаем контекст вручную или получаем из DI
			using (var context = new MarketplaceDbContext())
			{
				await context.Database.MigrateAsync(); // Применяет миграции и запускает сидер
			}

		}
	}

}
