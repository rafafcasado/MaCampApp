namespace MaCamp.CustomControls
{
    public class SmartContentView : ContentView
    {
        public bool HasAppeared { get; private set; }

        public event EventHandler? FirstAppeared;

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (!HasAppeared && Parent != null && IsVisible)
            {
                HasAppeared = true;
                FirstAppeared?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
