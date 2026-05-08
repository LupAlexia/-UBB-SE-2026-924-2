using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.Src.Service.Interfaces;

namespace AirportApp.Src.ViewModel
{
    public class AuthViewModel : ViewModelBase
    {
        private readonly IAuthService authService;
        private readonly INavigationService navigationService;

        private string emailText = string.Empty;
        private string passwordText = string.Empty;
        private string usernameText = string.Empty;
        private string phoneText = string.Empty;
        private string errorMessage = string.Empty;
        private string successMessage = string.Empty;
        private bool isLoginMode = true;
        private bool isAuthenticated;
        private Customer? authenticatedUser;

        private string titleText = "Flight Security Access";
        private string subtitleText = "To protect your flight details and personal data, please complete this quick security verification.";
        private string actionButtonLabel = "Sign In";
        private string togglePromptLabel = "Don't have an account?";
        private string toggleButtonLabel = "Create one";
        private bool isRegisterFieldsVisible = false;

        public AuthViewModel(IAuthService authService, INavigationService navigationService)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            ActionCommand = new RelayCommand(async parameter => await ExecuteActionAsync(), parameter => IsFormValid);
            ToggleModeCommand = new RelayCommand(parameter => ToggleMode());
        }

        public string EmailText
        {
            get => emailText;
            set
            {
                emailText = value;
                OnPropertyChanged();
                RaiseActionCanExecuteChanged();
            }
        }

        public string PasswordText
        {
            get => passwordText;
            set
            {
                passwordText = value;
                OnPropertyChanged();
                RaiseActionCanExecuteChanged();
            }
        }

        public string UsernameText
        {
            get => usernameText;
            set
            {
                usernameText = value;
                OnPropertyChanged();
                RaiseActionCanExecuteChanged();
            }
        }

        public string PhoneText
        {
            get => phoneText;
            set
            {
                phoneText = value;
                OnPropertyChanged();
                RaiseActionCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public string SuccessMessage
        {
            get => successMessage;
            set
            {
                successMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoginMode
        {
            get => isLoginMode;
            set
            {
                isLoginMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsAuthenticated
        {
            get => isAuthenticated;
            set
            {
                isAuthenticated = value;
                OnPropertyChanged();
            }
        }

        public Customer? AuthenticatedUser
        {
            get => authenticatedUser;
            set
            {
                authenticatedUser = value;
                OnPropertyChanged();
            }
        }

        public string TitleText
        {
            get => titleText;
            set
            {
                titleText = value;
                OnPropertyChanged();
            }
        }

        public string SubtitleText
        {
            get => subtitleText;
            set
            {
                subtitleText = value;
                OnPropertyChanged();
            }
        }

        public string ActionButtonLabel
        {
            get => actionButtonLabel;
            set
            {
                actionButtonLabel = value;
                OnPropertyChanged();
            }
        }

        public string TogglePromptLabel
        {
            get => togglePromptLabel;
            set
            {
                togglePromptLabel = value;
                OnPropertyChanged();
            }
        }

        public string ToggleButtonLabel
        {
            get => toggleButtonLabel;
            set
            {
                toggleButtonLabel = value;
                OnPropertyChanged();
            }
        }

        public bool IsRegisterFieldsVisible
        {
            get => isRegisterFieldsVisible;
            set
            {
                isRegisterFieldsVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsFormValid
        {
            get
            {
                if (IsLoginMode)
                {
                    return !string.IsNullOrWhiteSpace(EmailText) &&
                           !string.IsNullOrWhiteSpace(PasswordText);
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(EmailText) &&
                           !string.IsNullOrWhiteSpace(UsernameText) &&
                           !string.IsNullOrWhiteSpace(PhoneText) &&
                           !string.IsNullOrWhiteSpace(PasswordText);
                }
            }
        }

        public ICommand ActionCommand { get; }
        public ICommand ToggleModeCommand { get; }

        private async Task ExecuteActionAsync()
        {
            if (IsLoginMode)
            {
                await LoginAsync();

                if (IsAuthenticated)
                {
                    UserSession.CurrentUser = AuthenticatedUser;

                    if (UserSession.PendingBookingParameters != null)
                    {
                        var pendingParameters = UserSession.PendingBookingParameters;
                        UserSession.PendingBookingParameters = null;
                        navigationService.NavigateTo(typeof(View.BookingPage), pendingParameters);
                    }
                    else
                    {
                        navigationService.NavigateTo(typeof(View.FlightSearchPage));
                    }
                }
            }
            else
            {
                await RegisterAsync();

                if (string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    SetLoginMode();
                }
            }
        }

        private void ToggleMode()
        {
            if (IsLoginMode)
            {
                SetRegisterMode();
            }
            else
            {
                SetLoginMode();
            }

            ClearMessages();
            RaiseActionCanExecuteChanged();
        }

        private void SetLoginMode()
        {
            IsLoginMode = true;
            TitleText = "Welcome to WizzErr";
            SubtitleText = "Please sign in to manage your tickets";
            ActionButtonLabel = "Sign In";
            TogglePromptLabel = "Don't have an account?";
            ToggleButtonLabel = "Create one";
            IsRegisterFieldsVisible = false;
        }

        private void SetRegisterMode()
        {
            IsLoginMode = false;
            TitleText = "Create a WizzErr Account";
            SubtitleText = "Fill in the details to register";
            ActionButtonLabel = "Register";
            TogglePromptLabel = "Already have an account?";
            ToggleButtonLabel = "Sign in";
            IsRegisterFieldsVisible = true;
        }

        private async Task LoginAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                Customer user = await authService.LoginAsync(EmailText, PasswordText);

                AuthenticatedUser = user;
                IsAuthenticated = true;
                SuccessMessage = "Login successful.";
            }
            catch (Exception ex)
            {
                IsAuthenticated = false;
                AuthenticatedUser = null;
                ErrorMessage = ex.Message;
            }
        }

        private async Task RegisterAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                await authService.RegisterAsync(EmailText, PhoneText, UsernameText, PasswordText);

                SuccessMessage = "Registration successful. You can now sign in.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private void RaiseActionCanExecuteChanged()
        {
            OnPropertyChanged(nameof(IsFormValid));
            (ActionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}
