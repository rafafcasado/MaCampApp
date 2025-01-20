using System.Text.RegularExpressions;
using MaCamp.Utils;

namespace MaCamp.Views.Detalhes
{
    public class EmailValidation : Behavior<Entry>
    {
        public static bool ValidarEmail(string? email)
        {
            if (email == null)
            {
                return false;
            }

            var isValid = Regex.IsMatch(email, AppConstants.EmailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

            return isValid;
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;

            base.OnAttachedTo(bindable);
        }

        private void HandleTextChanged(object? sender, TextChangedEventArgs e)
        {
            var isValid = ValidarEmail(e.NewTextValue);

            if (sender is Entry entry)
            {
                entry.TextColor = isValid ? Colors.Transparent : Colors.Red;
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;

            base.OnDetachingFrom(bindable);
        }
    }
}