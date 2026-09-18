using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Marketplace_Group_Project.ViewModels
{
    
    /// ViewModel главного окна покупателя.
    /// Отвечает за отображение товаров, корзины и заказов пользователя.
    
    public class UserMainViewModel : ViewModelBase
    {
        private readonly MarketplaceService _marketplaceService;
        private readonly AppNavigationService _navigationService;
        private readonly User _currentUser;

        private string _searchText = string.Empty;
        private CategoryEnum? _selectedCategory;
        private Product? _selectedProduct;

        public UserMainViewModel(MarketplaceService marketplaceService,
                                 AppNavigationService navigationService,
                                 User currentUser)
        {
            _marketplaceService = marketplaceService;
            _navigationService = navigationService;
            _currentUser = currentUser;

            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            Categories = new ObservableCollection<CategoryEnum>();
            CartItems = new ObservableCollection<CartItem>();
            UserOrders = new ObservableCollection<Order>();

            foreach (CategoryEnum category in Enum.GetValues(typeof(CategoryEnum)))
            {
                Categories.Add(category);
            }

            SearchCommand = new RelayCommand(_ => ApplyFilter());
            FilterByCategoryCommand = new RelayCommand(_ => ApplyFilter());
            ResetFilterCommand = new RelayCommand(_ => ResetFilter());
            AddToCartCommand = new RelayCommand(_ => AddToCart(), _ => SelectedProduct != null);
            RemoveFromCartCommand = new RelayCommand(item => RemoveFromCart(item as CartItem));
            IncreaseQuantityCommand = new RelayCommand(item => ChangeQuantity(item as CartItem, 1));
            DecreaseQuantityCommand = new RelayCommand(item => ChangeQuantity(item as CartItem, -1));
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => CartItems.Any());
            RefreshOrdersCommand = new RelayCommand(_ => LoadOrders());
            LogoutCommand = new RelayCommand(_ => Logout());

            LoadProducts();
            LoadCart();
            LoadOrders();
        }

        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Product> FilteredProducts { get; }
        public ObservableCollection<CategoryEnum> Categories { get; }
        public ObservableCollection<CartItem> CartItems { get; }
        public ObservableCollection<Order> UserOrders { get; }

        public string CurrentUserName => _currentUser.Login;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        public CategoryEnum? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    ApplyFilter();
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public decimal CartTotal => _marketplaceService.GetCartTotal(_currentUser.Id);

        public bool HasOrders => UserOrders.Any();

        public ICommand SearchCommand { get; }
        public ICommand FilterByCategoryCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand RefreshOrdersCommand { get; }
        public ICommand LogoutCommand { get; }

        private void LoadProducts()
        {
            Products.Clear();
            var products = _marketplaceService.GetProducts().Where(p => p.IsActive);
            foreach (var product in products)
            {
                Products.Add(product);
            }
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var query = Products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedCategory.HasValue)
            {
                query = query.Where(p => p.Category == SelectedCategory.Value);
            }

            FilteredProducts.Clear();
            foreach (var product in query)
            {
                FilteredProducts.Add(product);
            }
        }

        private void ResetFilter()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
        }

        private void LoadCart()
        {
            CartItems.Clear();
            var items = _marketplaceService.GetCartItems(_currentUser.Id);
            foreach (var item in items)
            {
                CartItems.Add(item);
            }
            OnPropertyChanged(nameof(CartTotal));
        }

        private void AddToCart()
        {
            if (SelectedProduct == null)
                return;

            var cartItem = new CartItem
            {
                UserId = _currentUser.Id,
                ProductId = SelectedProduct.Id,
                Quantity = 1
            };

            _marketplaceService.AddToCart(cartItem);
            LoadCart();
        }

        private void RemoveFromCart(CartItem? item)
        {
            if (item == null)
                return;

            _marketplaceService.RemoveFromCart(item.Id);
            LoadCart();
        }

        private void ChangeQuantity(CartItem? item, int delta)
        {
            if (item == null)
                return;

            int newQuantity = item.Quantity + delta;
            if (newQuantity < 1)
                return;

            try
            {
                _marketplaceService.ChangeCartItemQauntity(item.Id, newQuantity);
                LoadCart();
            }
            catch (ArgumentException)
            {
                // Превышено доступное количество товара на складе.
            }
        }

        private void Checkout()
        {
            var order = new Order
            {
                UserId = _currentUser.Id,
                CreatedAt = DateTime.UtcNow,
                Status = StatusEnum.Created,
                TotalPrice = CartTotal,
                OrderItems = CartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.Price ?? 0
                }).ToList()
            };

            try
            {
                _marketplaceService.CreateOrder(order);
                _marketplaceService.ClearCart(_currentUser.Id);
                LoadCart();
                LoadOrders();
            }
            catch (ArgumentOutOfRangeException)
            {
                // Недостаточно товара на складе.
            }
        }

        private void LoadOrders()
        {
            UserOrders.Clear();
            var orders = _marketplaceService.GetUserOrders(_currentUser.Id);
            foreach (var order in orders)
            {
                UserOrders.Add(order);
            }
            OnPropertyChanged(nameof(HasOrders));
        }

        private void Logout()
        {
            _navigationService.Logout();
        }
    }
}