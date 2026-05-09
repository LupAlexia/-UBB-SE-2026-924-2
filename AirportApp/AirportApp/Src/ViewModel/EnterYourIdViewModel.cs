using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// ViewModel for the EnterYourId page.
/// Handles user identification input and authentication logic.
/// </summary>
///
namespace AirportApp.Src.ViewModel
{
    public sealed partial class EnterYourIdViewModel : ObservableObject
    {
        /// <summary>
        /// The user-provided identification string.
        /// </summary>
        [ObservableProperty]
        private string userIdentification;

        public bool IsTestingMode { get; set; } = false;

        /// <summary>
        /// Attempts to authenticate the user by parsing the identification string
        /// and calling the application's SetUserAsync method.
        /// </summary>
        /// <returns>A tuple of (Success, ParsedId). Success is true if authentication succeeds.</returns>
        public async Task<(bool Success, int ParsedId)> TryAuthenticateAsync()
        {
            if (int.TryParse(UserIdentification, out int parsedId))
            {
                if (IsTestingMode)
                {
                    return (true, parsedId);
                }

                bool result = await ((App)App.Current).SetUserAsync(parsedId);
                return (result, parsedId);
            }
            return (false, 0);
        }
    }
}