using Marketplace_Group_Project.Converters;
using Marketplace_Group_Project.Data;
using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Network;
using Marketplace_Group_Project.Services;
using Marketplace_Group_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Marketplace_Group_Project.Views
{
    public partial class AuthWindow : Window
    {
        private AuthViewModel _viewModel;

        public AuthWindow()
        {
            InitializeComponent();

            var context = new MarketplaceDbContext();
            var authService = new AuthenticationService(context);
            var marketplaceService = new MarketplaceService();
            var naviService = new AppNavigationService(marketplaceService);
            var networkService = new NetworkService(
                smtpHost: "smtp.mail.ru",
                smtpPort: 465,
                useSsl: true,
                emailFrom: "sttrebery@mail.ru",
                password: "GbBEEpJAt5NjKKUBokbg"
				);

            _viewModel = new AuthViewModel(authService, naviService, networkService);
            DataContext = _viewModel;
        }

        private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PwdBox.Password;
        }

        private void RepeatPwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.RepeatPassword = RepeatPwdBox.Password;
        }

        /*protected override void OnClosed(EventArgs e)
        {
            networkService.Dispose();
            base.OnClosed(e);
        }*/
    }
}
