using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Services;
using System;
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

        // ---- Товары ----
        private Product? _selectedProduct;
        private Product? _editingProduct;

        // ---- Форма нового товара ----
        private string _newProductName = string.Empty;
        private string _newProductDescription = string.Empty;
        private decimal _newProductPrice;
        private int _newProductStockQuantity;
        private CategoryEnum _newProductCategory;

        // ---- Характеристики ----
        private ProductCharacteristic? _selectedCharacteristic;
        private string _newCharacteristicName = string.Empty;
        private string _newCharacteristicValue = string.Empty;
        private UnitEnum _newCharacteristicUnit = UnitEnum.None;

        // ---- Заказы ----
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
            Categories = new ObservableCollection<CategoryEnum>();
            Units = new ObservableCollection<UnitEnum>();
            Characteristics = new ObservableCollection<ProductCharacteristic>();

            foreach (StatusEnum status in Enum.GetValues(typeof(StatusEnum)))
                Statuses.Add(status);

            foreach (CategoryEnum category in Enum.GetValues(typeof(CategoryEnum)))
                Categories.Add(category);

            foreach (UnitEnum unit in Enum.GetValues(typeof(UnitEnum)))
                Units.Add(unit);

            RefreshProductsCommand = new RelayCommand(_ => LoadProducts());
            DeactivateProductCommand = new RelayCommand(_ => DeactivateProduct(), _ => SelectedProduct != null);
            AddProductCommand = new RelayCommand(_ => AddProduct(), _ => CanAddProduct());
            StartEditProductCommand = new RelayCommand(_ => StartEditProduct(), _ => SelectedProduct != null);
            SaveProductCommand = new RelayCommand(_ => SaveProduct(), _ => EditingProduct != null);
            CancelEditCommand = new RelayCommand(_ => CancelEdit());
            AddCharacteristicCommand = new RelayCommand(_ => AddCharacteristic(), _ => CanAddCharacteristic());
            DeleteCharacteristicCommand = new RelayCommand(_ => DeleteCharacteristic(), _ => SelectedCharacteristic != null);
            RefreshOrdersCommand = new RelayCommand(_ => LoadOrders());
            ChangeOrderStatusCommand = new RelayCommand(_ => ChangeOrderStatus(), _ => SelectedOrder != null);
            LogoutCommand = new RelayCommand(_ => Logout());

            LoadProducts();
            LoadOrders();
        }

        // ==================== Коллекции ====================

        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Order> Orders { get; }
        public ObservableCollection<StatusEnum> Statuses { get; }
        public ObservableCollection<CategoryEnum> Categories { get; }
        public ObservableCollection<UnitEnum> Units { get; }
        public ObservableCollection<ProductCharacteristic> Characteristics { get; }

        public string CurrentUserName => _currentUser.Login;

        // ==================== Товары ====================

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

        public Product? EditingProduct
        {
            get => _editingProduct;
            set => SetProperty(ref _editingProduct, value);
        }

        public string NewProductName
        {
            get => _newProductName;
            set => SetProperty(ref _newProductName, value);
        }

        public string NewProductDescription
        {
            get => _newProductDescription;
            set => SetProperty(ref _newProductDescription, value);
        }

        public decimal NewProductPrice
        {
            get => _newProductPrice;
            set => SetProperty(ref _newProductPrice, value);
        }

        public int NewProductStockQuantity
        {
            get => _newProductStockQuantity;
            set => SetProperty(ref _newProductStockQuantity, value);
        }

        public CategoryEnum NewProductCategory
        {
            get => _newProductCategory;
            set => SetProperty(ref _newProductCategory, value);
        }

        // ==================== Характеристики ====================

        public ProductCharacteristic? SelectedCharacteristic
        {
            get => _selectedCharacteristic;
            set => SetProperty(ref _selectedCharacteristic, value);
        }

        public string NewCharacteristicName
        {
            get => _newCharacteristicName;
            set => SetProperty(ref _newCharacteristicName, value);
        }

        public string NewCharacteristicValue
        {
            get => _newCharacteristicValue;
            set => SetProperty(ref _newCharacteristicValue, value);
        }

        public UnitEnum NewCharacteristicUnit
        {
            get => _newCharacteristicUnit;
            set => SetProperty(ref _newCharacteristicUnit, value);
        }

        // ==================== Заказы ====================

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

        // ==================== Команды ====================

        public ICommand RefreshProductsCommand { get; }
        public ICommand DeactivateProductCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand StartEditProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand AddCharacteristicCommand { get; }
        public ICommand DeleteCharacteristicCommand { get; }
        public ICommand RefreshOrdersCommand { get; }
        public ICommand ChangeOrderStatusCommand { get; }
        public ICommand LogoutCommand { get; }

        // ==================== Логика: товары ====================

        private void LoadProducts()
        {
            Products.Clear();
            var products = _marketplaceService.GetProducts();
            foreach (var product in products)
                Products.Add(product);
        }

        private bool CanAddProduct()
        {
            return !string.IsNullOrWhiteSpace(NewProductName)
                && NewProductPrice > 0
                && NewProductStockQuantity >= 0;
        }

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
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _marketplaceService.AddProduct(product);
                LoadProducts();

                // Очистить форму
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
            if (SelectedProduct == null)
                return;

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
                ImagePath = SelectedProduct.ImagePath
            };
        }

        private void SaveProduct()
        {
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

        private void CancelEdit()
        {
            EditingProduct = null;
        }

        private void DeactivateProduct()
        {
            if (SelectedProduct == null)
                return;

            _marketplaceService.DeactivateProduct(SelectedProduct.Id);
            LoadProducts();
        }

        // ==================== Логика: характеристики ====================

        private void LoadCharacteristics()
        {
            Characteristics.Clear();
            if (SelectedProduct == null)
                return;

            var characteristics = _marketplaceService.GetProductCharacteristics(SelectedProduct.Id);
            foreach (var characteristic in characteristics)
                Characteristics.Add(characteristic);
        }

        private bool CanAddCharacteristic()
        {
            return SelectedProduct != null
                && !string.IsNullOrWhiteSpace(NewCharacteristicName)
                && !string.IsNullOrWhiteSpace(NewCharacteristicValue);
        }

        private void AddCharacteristic()
        {
            if (SelectedProduct == null)
                return;

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
            if (SelectedCharacteristic == null)
                return;

            _marketplaceService.DeleteCharachteristic(SelectedCharacteristic.Id);
            LoadCharacteristics();
        }

        // ==================== Логика: заказы ====================

        private void LoadOrders()
        {
            Orders.Clear();

            // TODO: заменить на GetAllOrders(), когда метод появится в MarketplaceService.
            // Сейчас админ видит только свои заказы.
            var orders = _marketplaceService.GetUserOrders(_currentUser.Id);
            foreach (var order in orders)
                Orders.Add(order);
        }

        private void ChangeOrderStatus()
        {
            if (SelectedOrder == null)
                return;

            _marketplaceService.ChangeOrderStatus(SelectedOrder, SelectedStatus);
            LoadOrders();
        }

        // ==================== Выход ====================

        private void Logout()
        {
            _navigationService.Logout();
        }
    }
}