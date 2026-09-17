using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Marketplace_Group_Project.ViewModels
{
    /// <summary>
    /// ViewModel главного окна администратора.
    /// Отвечает за управление товарами и заказами.
    /// </summary>
    public class AdminMainViewModel : ViewModelBase
    {
        private readonly MarketplaceService _marketplaceService;
        private readonly AppNavigationService _navigationService;
        private readonly User _currentUser;

        private Product? _selectedProduct;
        private Order? _selectedOrder;
        private StatusEnum _selectedStatus;

        public AdminMainViewModel(MarketplaceService marketplaceService,
                                  AppNavigationService navigationService,
                                  User currentUser)
        {
            _marketplaceService = marketplaceService;
            _navigationService = navigationService;
            _currentUser = currentUser;

            Products = new ObservableCollection<Product>();
            Orders = new ObservableCollection<Order>();
            Statuses = new ObservableCollection<StatusEnum>();

            foreach (StatusEnum status in Enum.GetValues(typeof(StatusEnum)))
            {
                Statuses.Add(status);
            }

            RefreshProductsCommand = new RelayCommand(_ => LoadProducts());
            DeactivateProductCommand = new RelayCommand(_ => DeactivateProduct(), _ => SelectedProduct != null);
            RefreshOrdersCommand = new RelayCommand(_ => LoadOrders());
            ChangeOrderStatusCommand = new RelayCommand(_ => ChangeOrderStatus(), _ => SelectedOrder != null);
            LogoutCommand = new RelayCommand(_ => Logout());

            LoadProducts();
            LoadOrders();
        }

        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Order> Orders { get; }
        public ObservableCollection<StatusEnum> Statuses { get; }

        public string CurrentUserName => _currentUser.Login;

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public StatusEnum SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ICommand RefreshProductsCommand { get; }
        public ICommand DeactivateProductCommand { get; }
        public ICommand RefreshOrdersCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadProducts()
        {
            Products.Clear();
            var products = _marketplaceService.GetProducts();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private void DeactivateProduct()
        {
            if (SelectedProduct == null)
                return;

            _marketplaceService.DeactivateProduct(SelectedProduct.Id);
            LoadProducts();
        }

        private void LoadOrders()
        {
            Orders.Clear();
            var orders = _marketplaceService.GetUserOrders(_currentUser.Id);
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
        }

        private void ChangeOrderStatus()
        {
            if (SelectedOrder == null)
                return;

            _marketplaceService.ChangeOrderStatus(SelectedOrder, SelectedStatus);
            LoadOrders();
        }

        private void Logout()
        {
            _navigationService.Logout();
        }
    }
}