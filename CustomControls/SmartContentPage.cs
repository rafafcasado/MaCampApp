namespace MaCamp.CustomControls
{
    public class SmartContentPage : ContentPage
    {
        public bool HasAppeared { get; private set; }

        public event EventHandler? FirstAppeared;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!HasAppeared)
            {
                HasAppeared = true;
                FirstAppeared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
