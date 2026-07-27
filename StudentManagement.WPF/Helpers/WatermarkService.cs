using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace StudentManagement.WPF.Helpers
{
    public static class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(WatermarkService), new FrameworkPropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject d) => (string)d.GetValue(WatermarkProperty);
        public static void SetWatermark(DependencyObject d, string value) => d.SetValue(WatermarkProperty, value);

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                control.Loaded -= Control_Loaded;
                control.GotFocus -= Control_FocusChanged;
                control.LostFocus -= Control_FocusChanged;
                
                if (control is TextBox textBox) textBox.TextChanged -= Control_TextChanged;
                if (control is PasswordBox passwordBox) passwordBox.PasswordChanged -= Control_TextChanged;
                if (control is ComboBox comboBox) comboBox.SelectionChanged -= Control_SelectionChanged;

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    control.Loaded += Control_Loaded;
                    control.GotFocus += Control_FocusChanged;
                    control.LostFocus += Control_FocusChanged;
                    if (control is TextBox tb) tb.TextChanged += Control_TextChanged;
                    if (control is PasswordBox pb) pb.PasswordChanged += Control_TextChanged;
                    if (control is ComboBox cb) cb.SelectionChanged += Control_SelectionChanged;
                    
                    if (control.IsLoaded)
                    {
                        UpdateWatermark(control);
                    }
                }
            }
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e) => UpdateWatermark((Control)sender);
        private static void Control_FocusChanged(object sender, RoutedEventArgs e) => UpdateWatermark((Control)sender);
        private static void Control_TextChanged(object sender, RoutedEventArgs e) => UpdateWatermark((Control)sender);
        private static void Control_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateWatermark((Control)sender);

        private static void UpdateWatermark(Control control)
        {
            if (ShouldShowWatermark(control))
            {
                if (control is ComboBox)
                {
                    // Use Adorner for ComboBox because VisualBrush doesn't render over its default template
                    var layer = AdornerLayer.GetAdornerLayer(control);
                    if (layer != null)
                    {
                        var adorners = layer.GetAdorners(control);
                        WatermarkAdorner watermarkAdorner = null;
                        if (adorners != null)
                        {
                            foreach (var adorner in adorners)
                            {
                                if (adorner is WatermarkAdorner wa)
                                {
                                    watermarkAdorner = wa;
                                    break;
                                }
                            }
                        }
                        if (watermarkAdorner == null)
                        {
                            layer.Add(new WatermarkAdorner(control, GetWatermark(control)));
                        }
                    }
                }
                else
                {
                    // Use VisualBrush for TextBox and PasswordBox
                    double paddingLeft = 5;
                    if (control.Padding.Left > 0) paddingLeft = control.Padding.Left + 2;

                    var textBlock = new TextBlock
                    {
                        Text = GetWatermark(control),
                        Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175)), // #9CA3AF
                        FontStyle = FontStyles.Italic,
                        Padding = new Thickness(paddingLeft, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var brush = new VisualBrush
                    {
                        Visual = textBlock,
                        Stretch = Stretch.None,
                        AlignmentX = AlignmentX.Left,
                        AlignmentY = AlignmentY.Center
                    };

                    control.Background = brush;
                }
            }
            else
            {
                if (control is ComboBox)
                {
                    var layer = AdornerLayer.GetAdornerLayer(control);
                    if (layer != null)
                    {
                        var adorners = layer.GetAdorners(control);
                        if (adorners != null)
                        {
                            foreach (var adorner in adorners)
                            {
                                if (adorner is WatermarkAdorner wa)
                                {
                                    layer.Remove(wa);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    control.Background = Brushes.White;
                }
            }
        }

        private static bool ShouldShowWatermark(Control control)
        {
            if (control.IsFocused) return false;

            if (control is TextBox textBox) return string.IsNullOrEmpty(textBox.Text);
            if (control is PasswordBox passwordBox) return string.IsNullOrEmpty(passwordBox.Password);
            if (control is ComboBox comboBox) return comboBox.SelectedItem == null && string.IsNullOrEmpty(comboBox.Text);
            
            return false;
        }
    }

    public class WatermarkAdorner : Adorner
    {
        private readonly TextBlock _textBlock;

        public WatermarkAdorner(UIElement adornedElement, string watermark) : base(adornedElement)
        {
            IsHitTestVisible = false;
            
            double paddingLeft = 5;
            if (adornedElement is Control c && c.Padding.Left > 0)
            {
                paddingLeft = c.Padding.Left;
            }

            _textBlock = new TextBlock
            {
                Text = watermark,
                Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175)), // #9CA3AF
                FontStyle = FontStyles.Italic,
                Padding = new Thickness(paddingLeft + 2, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0)
            };
            AddVisualChild(_textBlock);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _textBlock;

        protected override Size MeasureOverride(Size constraint)
        {
            _textBlock.Measure(constraint);
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
