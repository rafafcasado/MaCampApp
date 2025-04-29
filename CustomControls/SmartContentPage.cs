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
                await Task.Delay(250);

                HasAppeared = true;
                FirstAppeared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
