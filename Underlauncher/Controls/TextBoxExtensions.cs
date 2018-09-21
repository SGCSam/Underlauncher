using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Underlauncher
{
    public static class TextBoxExtensions
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(string.Empty));

        public static string GetHeader(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderProperty);
        }

        public static void SetHeader(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderProperty, value);
        }
    }
}
