using System.Windows;
using System.Windows.Media;

namespace Underlauncher
{
    class Button
    {
        public static readonly DependencyProperty PointsProperty;

        public static PointCollection GetPoints(DependencyObject obj)
        {
            return (PointCollection)obj.GetValue(PointsProperty);
        }

        public static void SetPoints(DependencyObject obj, PointCollection value)
        {
            obj.SetValue(PointsProperty, value);
        }
        static Button()
        {
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
            PointsProperty = DependencyProperty.RegisterAttached("Points", typeof(PointCollection), typeof(Button), metadata);
        }

    }
}
