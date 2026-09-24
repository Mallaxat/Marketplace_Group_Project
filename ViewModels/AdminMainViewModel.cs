using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Marketplace_Group_Project.ViewModels
{
    public class AdminMainViewModel : ViewModelBase
    {
        private readonly MarketplaceService _marketplaceService;
        private readonly AppNavigationService _navigationService;
        private readonly User _currentUser;

        private Product? _selectedProduct;
        private Product? _editingProduct;

        private string _newProductName = string.Empty;
        private string _newProductDescription = string.Empty;
        private decimal _newProductPrice;
        private int _newProductStockQuantity;
        private CategoryEnum _newProductCategory;

        private ProductCharacteristic? _selectedCharacteristic;
        private string _newCharacteristicName = string.Empty;
        private string _newCharacteristicValue = string.Empty;
        private UnitEnum _newCharacteristicUnit = UnitEnum.None;

        private OrderDisplay? _selectedOrder;
        private StatusEnum _selectedStatus;

        public AdminMainViewModel(MarketplaceService marketplaceService,
                                  AppNavigationService navigationService,
                                  User currentUser)
        {
            _marketplaceService = marketplaceService;
            _navigationService = navigationService;
            _currentUser = currentUser;

            Products = new ObservableCollection<Product>();
            Orders = new ObservableCollection<OrderDisplay>();
            Statuses = new ObservableCollection<StatusEnum>();
            Categories = new ObservableCollection<CategoryEnum>();
            Units = new ObservableCollection<UnitEnum>();
            Characteristics = new ObservableCollection<ProductCharacteristic>();

            Statuses.Add(StatusEnum.InProgress);
            Statuses.Add(StatusEnum.Postponed);
            Statuses.Add(StatusEnum.Done);

            foreach (CategoryEnum category in Enum.GetValues(typeof(CategoryEnum)))
                Categories.Add(category);

            foreach (UnitEnum unit in Enum.GetValues(typeof(UnitEnum)))
                Units.Add(unit);

            RefreshProductsCommand = new RelayCommand(_ => LoadProducts());

            DeactivateProductCommand = new RelayCommand(
                _ => DeactivateProduct(),
                _ => SelectedProduct != null && SelectedProduct.SellerId == _currentUser.Id);

            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());

            StartEditProductCommand = new RelayCommand(
                _ => StartEditProduct(),
                _ => SelectedProduct != null && SelectedProduct.SellerId == _currentUser.Id);

            SaveProductCommand = new RelayCommand(_ => SaveProduct(), _ => EditingProduct != null);

            CancelEditCommand = new RelayCommand(_ => CancelEdit());

            AddCharacteristicCommand = new RelayCommand(
                _ => AddCharacteristic(),
                _ => CanAddCharacteristic());

            DeleteCharacteristicCommand = new RelayCommand(
                _ => DeleteCharacteristic(),
                _ => SelectedCharacteristic != null);

            RefreshOrdersCommand = new RelayCommand(_ => LoadOrders());

            ChangeOrderStatusCommand = new RelayCommand(
                _ => ChangeOrderStatus(),
                _ => SelectedOrder != null && SelectedOrder.Status != StatusEnum.Done);

            LogoutCommand = new RelayCommand(_ => Logout());

            LoadProducts();
            LoadOrders();
        }

        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<OrderDisplay> Orders { get; }
        public ObservableCollection<StatusEnum> Statuses { get; }
        public ObservableCollection<CategoryEnum> Categories { get; }
        public ObservableCollection<UnitEnum> Units { get; }
        public ObservableCollection<ProductCharacteristic> Characteristics { get; }

        public string CurrentUserName => _currentUser.Login;

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    LoadCharacteristics();
                    CancelEdit();
                }
            }
        }

        public Product? EditingProduct { get => _editingProduct; set => SetProperty(ref _editingProduct, value); }
        public string NewProductName { get => _newProductName; set => SetProperty(ref _newProductName, value); }
        public string NewProductDescription { get => _newProductDescription; set => SetProperty(ref _newProductDescription, value); }
        public decimal NewProductPrice { get => _newProductPrice; set => SetProperty(ref _newProductPrice, value); }
        public int NewProductStockQuantity { get => _newProductStockQuantity; set => SetProperty(ref _newProductStockQuantity, value); }
        public CategoryEnum NewProductCategory { get => _newProductCategory; set => SetProperty(ref _newProductCategory, value); }

        public ProductCharacteristic? SelectedCharacteristic { get => _selectedCharacteristic; set => SetProperty(ref _selectedCharacteristic, value); }
        public string NewCharacteristicName { get => _newCharacteristicName; set => SetProperty(ref _newCharacteristicName, value); }
        public string NewCharacteristicValue { get => _newCharacteristicValue; set => SetProperty(ref _newCharacteristicValue, value); }
        public UnitEnum NewCharacteristicUnit { get => _newCharacteristicUnit; set => SetProperty(ref _newCharacteristicUnit, value); }

        public OrderDisplay? SelectedOrder { get => _selectedOrder; set => SetProperty(ref _selectedOrder, value); }
        public StatusEnum SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }

        public ICommand RefreshProductsCommand { get; }
        public ICommand DeactivateProductCommand { get; }
		public ICommand ActivateProductCommand { get; }
		public ICommand AddProductCommand { get; }
        public ICommand StartEditProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand AddCharacteristicCommand { get; }
        public ICommand DeleteCharacteristicCommand { get; }
        public ICommand RefreshOrdersCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadProducts()
        {
            Products.Clear();
            var products = _marketplaceService.GetProductsBySellerId(_currentUser.Id);
            foreach (var product in products)
                Products.Add(product);
        }

        private bool CanAddProduct() =>
            !string.IsNullOrWhiteSpace(NewProductName)
            && NewProductPrice > 0
            && NewProductStockQuantity >= 0;

        private void AddProduct()
        {
            var product = new Product
            {
                Name = NewProductName,
                Description = NewProductDescription,
                Price = NewProductPrice,
                StockQuantity = NewProductStockQuantity,
                Category = NewProductCategory,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SellerId = _currentUser.Id
            };

            try
            {
                _marketplaceService.AddProduct(product);
                LoadProducts();
                NewProductName = string.Empty;
                NewProductDescription = string.Empty;
                NewProductPrice = 0;
                NewProductStockQuantity = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartEditProduct()
        {
            if (SelectedProduct == null) return;
            if (SelectedProduct.SellerId != _currentUser.Id) return;

            EditingProduct = new Product
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                Description = SelectedProduct.Description,
                Price = SelectedProduct.Price,
                StockQuantity = SelectedProduct.StockQuantity,
                Category = SelectedProduct.Category,
                IsActive = SelectedProduct.IsActive,
                CreatedAt = SelectedProduct.CreatedAt,
                ImagePath = SelectedProduct.ImagePath,
                SellerId = SelectedProduct.SellerId
            };
        }

        private void SaveProduct()
        {
            //EditingProduct беру из SelectedProduct, т.к. таблица одна
            EditingProduct = SelectedProduct;

			if (EditingProduct == null)
                return;

            try
            {
                _marketplaceService.UpdateProduct(EditingProduct);
                LoadProducts();
                CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit() => EditingProduct = null;

        private void DeactivateProduct()
        {
            if (SelectedProduct == null) return;
            if (SelectedProduct.SellerId != _currentUser.Id) return;

            _marketplaceService.DeactivateProduct(SelectedProduct.Id);
            LoadProducts();
        }

		private void ActivateProduct()
		{
			if (SelectedProduct == null)
				return;

			if (SelectedProduct.SellerId != _currentUser.Id)
				return;

            SelectedProduct.IsActive = true;
			_marketplaceService.UpdateProduct(SelectedProduct);
			LoadProducts();
		}

		private void LoadCharacteristics()
        {
            Characteristics.Clear();
            if (SelectedProduct == null) return;

            var characteristics = _marketplaceService.GetProductCharacteristics(SelectedProduct.Id);
            foreach (var characteristic in characteristics)
                Characteristics.Add(characteristic);
        }

        private bool CanAddCharacteristic() =>
            SelectedProduct != null
            && SelectedProduct.SellerId == _currentUser.Id
            && !string.IsNullOrWhiteSpace(NewCharacteristicName)
            && !string.IsNullOrWhiteSpace(NewCharacteristicValue);

        private void AddCharacteristic()
        {
            if (SelectedProduct == null) return;
            if (SelectedProduct.SellerId != _currentUser.Id) return;

            var characteristic = new ProductCharacteristic
            {
                ProductId = SelectedProduct.Id,
                Name = NewCharacteristicName,
                Value = NewCharacteristicValue,
                Unit = NewCharacteristicUnit
            };

            try
            {
                _marketplaceService.AddCharacteristic(characteristic);
                LoadCharacteristics();
                NewCharacteristicName = string.Empty;
                NewCharacteristicValue = string.Empty;
                NewCharacteristicUnit = UnitEnum.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении характеристики: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCharacteristic()
        {
            if (SelectedCharacteristic == null) return;
            if (SelectedProduct == null || SelectedProduct.SellerId != _currentUser.Id) return;

            _marketplaceService.DeleteCharachteristic(SelectedCharacteristic.Id);
            LoadCharacteristics();
        }

        private void LoadOrders()
        {
            Orders.Clear();
            var orders = _marketplaceService.GetAllOrders();
            foreach (var order in orders)
            {
                string summary = BuildOrderSummary(order.Id);
                Orders.Add(new OrderDisplay(order, summary));
            }
        }

        /// <summary>
        /// Собирает строку со всеми товарами заказа: "Товар 1 ×2, Товар 2, Товар 3".
        /// </summary>
        private string BuildOrderSummary(int orderId)
        {
            var items = _marketplaceService.GetOrderItems(orderId)
                .Where(i => i.Quantity > 0)
                .ToList();

            if (items.Count == 0) return "(нет данных)";

            var names = new List<string>();
            foreach (var item in items)
            {
                var product = _marketplaceService.GetProductById(item.ProductId);
                string name = product?.Name ?? $"Товар #{item.ProductId}";
                if (item.Quantity > 1)
                    name += $" ×{item.Quantity}";
                names.Add(name);
            }

            return string.Join(", ", names);
        }

        private void ChangeOrderStatus()
        {
            if (SelectedOrder == null) return;
            if (SelectedOrder.Status == StatusEnum.Done) return;

            _marketplaceService.ChangeOrderStatus(SelectedOrder.Order, SelectedStatus);
            LoadOrders();
        }

        private void Logout() => _navigationService.Logout();
    }
}