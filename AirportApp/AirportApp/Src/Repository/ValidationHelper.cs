using System.Linq;
using System.Net.Mail;

namespace AirportApp.Src.Service
{
    public static class ValidationHelper
    {
        private const int MinimumPhoneLength = 10;
        private const int MaximumPhoneLength = 15;

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            return phone.All(char.IsDigit) && phone.Length >= MinimumPhoneLength && phone.Length <= MaximumPhoneLength;
        }
    }
}
