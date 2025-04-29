namespace MaCamp.CustomControls
{
    public class SmartContentView : ContentView
    {
        private bool HasAppeared { get; set; }

        public event EventHandler? FirstAppeared;

        protected override async void OnParentSet()
        {
            base.OnParentSet();

            if (!HasAppeared && Parent != null && IsVisible)
            {
                await Task.Delay(250);

                HasAppeared = true;
                FirstAppeared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
