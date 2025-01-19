using Microsoft.Maui.Controls.Maps;

namespace MaCamp.CustomControls
{
    public class StylishPin : Pin
    {
        private static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(StylishPin));
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        private static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(object), typeof(StylishPin));
        public object Data
        {
            get => (object)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}