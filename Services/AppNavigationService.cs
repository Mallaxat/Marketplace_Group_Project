using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.ViewModels;
using Marketplace_Group_Project.Views;
using System.Linq;
using System.Windows;

namespace Marketplace_Group_Project.Services
{
    /// <summary>
    /// Отвечает за переходы между окнами приложения
    /// и хранит текущего авторизованного пользователя.
    /// </summary>
    public class AppNavigationService
    {
        private readonly MarketplaceService _marketplaceService;

        public User? CurrentUser { get; private set; }

        public AppNavigationService(MarketplaceService marketplaceService)
        {
            _marketplaceService = marketplaceService;
        }

        /// <summary>
        /// Открывает главное окно покупателя.
        /// </summary>
        public void NavigateToUserMain(User user)
        {
            CurrentUser = user;

            var viewModel = new UserMainViewModel(_marketplaceService, this, user);
            var window = new UserMainWindow
            {
                DataContext = viewModel
            };

            OpenWindow(window);
        }

        /// <summary>
        /// Открывает главное окно администратора.
        /// </summary>
        public void NavigateToAdminMain(User user)
        {
            CurrentUser = user;

            var viewModel = new AdminMainViewModel(_marketplaceService, this, user);
            var window = new AdminMainWindow
            {
                DataContext = viewModel
            };

            OpenWindow(window);
        }

        /// <summary>
        /// Возвращает пользователя на окно входа.
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;

            var loginWindow = new MainWindow();
            OpenWindow(loginWindow);
        }

        /// <summary>
        /// Закрывает все открытые окна, кроме нового, и показывает новое.
        /// </summary>
        private static void OpenWindow(Window newWindow)
        {
            var oldWindows = Application.Current.Windows
                .Cast<Window>()
                .Where(w => w != newWindow)
                .ToList();

            foreach (var window in oldWindows)
            {
                window.Close();
            }

            newWindow.Show();
        }
    }
}