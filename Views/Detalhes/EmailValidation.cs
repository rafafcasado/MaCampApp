using System.Text.RegularExpressions;
using MaCamp.Utils;

namespace MaCamp.Views.Detalhes
{
    public class EmailValidation : Behavior<Entry>
    {
        private Color? TextColor { get; set; }

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
            base.OnAttachedTo(bindable);

            TextColor = bindable.TextColor;

            bindable.TextChanged += HandleTextChanged;
        }

        private void HandleTextChanged(object? sender, TextChangedEventArgs e)
        {
            var isValid = ValidarEmail(e.NewTextValue);

            if (sender is Entry entry && TextColor != null)
            {
                entry.TextColor = isValid ? TextColor : Colors.Red;
            }
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;

            base.OnDetachingFrom(bindable);
        }
    }
}