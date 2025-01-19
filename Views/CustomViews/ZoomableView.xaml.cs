namespace MaCamp.Views.CustomViews
{
    public partial class ZoomableView : ContentView
    {
        private double CurrentScale { get; set; }
        private double StartScale { get; set; }
        private double XOffset { get; set; }
        private double YOffset { get; set; }
        private Point Point { get; }
        private double PreviousX, PreviousY;

        public ZoomableView()
        {
            InitializeComponent();

            CurrentScale = 1.0;
            StartScale = 1.0;
            XOffset = 0.0;
            YOffset = 0.0;
            Point = new Point(0.5f, 0.5f);
        }

        public static BindableProperty MinScaleProperty => BindableProperty.Create(nameof(MinScale), typeof(double), typeof(ZoomableView), 1.0);
        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        public static BindableProperty MaxScaleProperty => BindableProperty.Create(nameof(MaxScale), typeof(double), typeof(ZoomableView), 4.0);
        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        public static BindableProperty DoubleTapScaleFactorProperty => BindableProperty.Create(nameof(DoubleTapScaleFactor), typeof(double), typeof(ZoomableView), 4.0);
        public double DoubleTapScaleFactor
        {
            get => (double)GetValue(DoubleTapScaleFactorProperty);
            set => SetValue(DoubleTapScaleFactorProperty, value);
        }

        public static BindableProperty DoubleTapToZoomProperty => BindableProperty.Create(nameof(DoubleTapToZoom), typeof(bool), typeof(ZoomableView), true);
        public bool DoubleTapToZoom
        {
            get => (bool)GetValue(DoubleTapToZoomProperty);
            set => SetValue(DoubleTapToZoomProperty, value);
        }

        public static BindableProperty IsDoubleTapZoomAnimationEnabledProperty => BindableProperty.Create(nameof(IsDoubleTapZoomAnimationEnabled), typeof(bool), typeof(ZoomableView), true);
        public bool IsDoubleTapZoomAnimationEnabled
        {
            get => (bool)GetValue(IsDoubleTapZoomAnimationEnabledProperty);
            set => SetValue(IsDoubleTapZoomAnimationEnabledProperty, value);
        }

        public static BindableProperty ZoomableProperty => BindableProperty.Create(nameof(Zoomable), typeof(bool), typeof(ZoomableView), true);
        public bool Zoomable
        {
            get => (bool)GetValue(ZoomableProperty);
            set => SetValue(ZoomableProperty, value);
        }

        public static BindableProperty TranslateableProperty => BindableProperty.Create(nameof(Translateable), typeof(bool), typeof(ZoomableView), true);
        public bool Translateable
        {
            get => (bool)GetValue(TranslateableProperty);
            set => SetValue(TranslateableProperty, value);
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (Zoomable)
            {
                switch (e.Status)
                {
                    case GestureStatus.Started:
                        OnPinchStarted();
                        break;
                    case GestureStatus.Running:
                        OnPinchRunning(e);
                        break;
                    case GestureStatus.Completed:
                        OnPinchCompleted();
                        break;
                }
            }
        }

        private void OnPinchStarted()
        {
            StartScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;
        }

        private void OnPinchRunning(PinchGestureUpdatedEventArgs e)
        {
            CurrentScale += (e.Scale - 1) * StartScale;
            CurrentScale = Math.Max(MinScale, CurrentScale);
            CurrentScale = Math.Min(MaxScale, CurrentScale);

            var renderedX = Content.X + XOffset;
            var deltaX = renderedX / Width;
            var deltaWidth = Width / (Content.Width * StartScale);
            var originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

            var renderedY = Content.Y + YOffset;
            var deltaY = renderedY / Height;
            var deltaHeight = Height / (Content.Height * StartScale);
            var originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

            var targetX = XOffset - ((originX * Content.Width) * (CurrentScale - StartScale));
            var targetY = YOffset - ((originY * Content.Height) * (CurrentScale - StartScale));

            if (CurrentScale < 1)
            {
                targetX = (Width - (Content.Width * CurrentScale)) / 2;
                targetY = (Height - (Content.Height * CurrentScale)) / 2;
            }
            else
            {
                targetX = Math.Min(0, Math.Max(targetX, -Content.Width * (CurrentScale - 1)));
                targetY = Math.Min(0, Math.Max(targetY, -Content.Height * (CurrentScale - 1)));
            }

            Content.TranslationX = targetX;
            Content.TranslationY = targetY;

            Content.Scale = CurrentScale;
        }

        private void OnPinchCompleted()
        {
            XOffset = Content.TranslationX;
            YOffset = Content.TranslationY;
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Translateable)
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        OnPanRunning(e);
                        break;
                    case GestureStatus.Started:
                        OnPanStarted(e);
                        break;
                    case GestureStatus.Canceled:
                    case GestureStatus.Completed:
                        OnPanCompleted();
                        break;
                }
            }
        }

        private void OnPanStarted(PanUpdatedEventArgs e)
        {
            PreviousX = e.TotalX;
            PreviousY = e.TotalY;
            XOffset = Content.TranslationX;
            YOffset = Content.TranslationY;
        }

        private void OnPanRunning(PanUpdatedEventArgs e)
        {
            if (Content.Scale != 1)
            {
                var deltaX = e.TotalX - PreviousX;
                var deltaY = e.TotalY - PreviousY;

                Content.TranslationX += deltaX;
                Content.TranslationY += deltaY;

                var maxX = Math.Max(0, (Content.Width * Content.Scale) - Width);
                var maxY = Math.Max(0, (Content.Height * Content.Scale) - Height);

                Content.TranslationX = Math.Clamp(Content.TranslationX, -maxX, 0);
                Content.TranslationY = Math.Clamp(Content.TranslationY, -maxY, 0);

                PreviousX = e.TotalX;
                PreviousY = e.TotalY;
            }
        }

        private void OnPanCompleted()
        {
            XOffset = Content.TranslationX;
            YOffset = Content.TranslationY;
        }

        public async void DoubleTapped(object sender, TappedEventArgs e)
        {
            if (Zoomable && DoubleTapToZoom)
            {
                StartScale = Content.Scale;
                CurrentScale = StartScale > MinScale ? MinScale : DoubleTapScaleFactor;

                CurrentScale = Math.Max(MinScale, Math.Min(CurrentScale, MaxScale));

                Content.AnchorX = 0;
                Content.AnchorY = 0;

                var point = e.GetPosition(Content);

                if (point != null)
                {
                    var touchX = point.Value.X;
                    var touchY = point.Value.Y;

                    var renderedX = Content.X + XOffset;
                    var renderedY = Content.Y + YOffset;

                    var originX = (touchX - renderedX) / (Content.Width * StartScale);
                    var originY = (touchY - renderedY) / (Content.Height * StartScale);

                    var targetX = XOffset - ((originX * Content.Width) * (CurrentScale - StartScale));
                    var targetY = YOffset - ((originY * Content.Height) * (CurrentScale - StartScale));

                    targetX = Math.Min(0, Math.Max(targetX, -Content.Width * (CurrentScale - 1)));
                    targetY = Math.Min(0, Math.Max(targetY, -Content.Height * (CurrentScale - 1)));

                    if (IsDoubleTapZoomAnimationEnabled)
                    {
                        var animationDuration = 250u;

                        await Task.WhenAll(Content.ScaleTo(CurrentScale, animationDuration, Easing.CubicInOut), Content.TranslateTo(targetX, targetY, animationDuration, Easing.CubicInOut));
                    }
                    else
                    {
                        Content.Scale = CurrentScale;
                        Content.TranslationX = targetX;
                        Content.TranslationY = targetY;
                    }

                    XOffset = Content.TranslationX;
                    YOffset = Content.TranslationY;
                }
            }
        }
    }
}
