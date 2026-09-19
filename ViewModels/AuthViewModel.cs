using Marketplace_Group_Project.Models;
using Marketplace_Group_Project.Network;
using Marketplace_Group_Project.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Marketplace_Group_Project.ViewModels
{
    public class AuthViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;
        private readonly AppNavigationService _appNaviService;
        private readonly NetworkService _networkService;

        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _repeatPassword = string.Empty;
        public string RepeatPassword
        {
            get => _repeatPassword;
            set => SetProperty(ref _repeatPassword, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _confirmationCode = string.Empty;
        public string ConfirmationCode
        {
            get => _confirmationCode;
            set => SetProperty(ref _confirmationCode, value);
        }

        private bool _isRegistrationMode = false;
        public bool IsRegistrationMode
        {
            get => _isRegistrationMode;
            set => SetProperty(ref _isRegistrationMode, value);
        }

        private bool _isConfirmationMode = false;
        public bool IsConfirmationMode
        {
            get => _isConfirmationMode;
            set => SetProperty(ref _isConfirmationMode, value);
        }

        // Скрытое: сгенерированный код
        private string _generatedCode = string.Empty;

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SwitchModeCommand { get; }
        public ICommand ConfirmCodeCommand { get; }

        public AuthViewModel(
            AuthenticationService authService,
            AppNavigationService appNaviService,
            NetworkService networkService)
        {
            _authService = authService;
            _appNaviService = appNaviService;
            _networkService = networkService;

            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync());
            SwitchModeCommand = new RelayCommand(_ => SwitchMode());
            ConfirmCodeCommand = new RelayCommand(async _ => await ConfirmCodeAsync());
        }

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Login))
            {
                ErrorMessage = "Введите логин.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите пароль.";
                return;
            }

            var user = await _authService.Login(Login, Password);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль.";
                return;
            }

            switch (user.Role)
            {
                case RoleEnum.Admin:
                    _appNaviService.NavigateToUserMain(user);
                    _appNaviService.Logout();
                    break;

                case RoleEnum.User:
                    _appNaviService.NavigateToUserMain(user);
                    _appNaviService.Logout();
                    break;

                default:
                    ErrorMessage = "Неизвестная роль пользователя.";
                    break;
            }
        }

        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Login))
            {
                ErrorMessage = "Введите логин.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Введите email.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите пароль.";
                return;
            }

            if (string.IsNullOrWhiteSpace(RepeatPassword))
            {
                ErrorMessage = "Повторите пароль.";
                return;
            }

            if (Password != RepeatPassword)
            {
                ErrorMessage = "Пароли не совпадают.";
                return;
            }

            if (await _authService.IsLoginExists(Login))
            {
                ErrorMessage = "Логин уже занят.";
                return;
            }

            if (await _authService.IsEmailExists(Email))
            {
                ErrorMessage = "Email уже занят.";
                return;
            }

            // 6-значный код
            _generatedCode = new Random().Next(100000, 999999).ToString();

            // Отправляем письмо
            try
            {
                await _networkService.ConnectAsync();
                await _networkService.SendMessageAsync(
                    toEmail: Email,
                    subject: "Код подтверждения регистрации",
                    body: $"Ваш код подтверждения: {_generatedCode}"
                );
                await _networkService.DisconnectAsync();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Не удалось отправить письмо: {ex.Message}";
                return;
            }

            // Переключаем UI на ввод кода
            IsConfirmationMode = true;
            ErrorMessage = "Код отправлен на почту. Введите его для завершения регистрации.";
        }

        private async Task ConfirmCodeAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ConfirmationCode))
            {
                ErrorMessage = "Введите код подтверждения";
                return;
            }

            if (ConfirmationCode != _generatedCode)
            {
                ErrorMessage = "Неверный код";
                return;
            }

            // Код верный — создаём пользователя в БД
            bool success = await _authService.Register(Login, Email, Password, RoleEnum.User);

            if (!success)
            {
                ErrorMessage = "Ошибка при регистрации. Попробуйте снова.";
                return;
            }

            // Успех — сброс и возврат к входу
            _generatedCode = string.Empty;
            ConfirmationCode = string.Empty;
            IsConfirmationMode = false;
            IsRegistrationMode = false;
            Password = string.Empty;
            RepeatPassword = string.Empty;
            ErrorMessage = "Успешная регистрация! Войдите в аккаунт.";
        }

        private void SwitchMode()
        {
            IsRegistrationMode = !IsRegistrationMode;
            IsConfirmationMode = false;
            ErrorMessage = string.Empty;
        }
    }
}
