using MaCamp.Utils;

namespace MaCamp.CustomControls
{
    public class SmartContentPage : ContentPage
    {
        private bool HasAppeared { get; set; }

        public event EventHandler? FirstAppeared;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!HasAppeared)
            {
                await Task.Delay(AppConstants.Delay);

                HasAppeared = true;
                FirstAppeared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
