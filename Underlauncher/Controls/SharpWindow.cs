using System;
using System.Windows;
using System.Windows.Media;

namespace Underlauncher
{
    public class SharpWindow : Window
    {
        static SharpWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SharpWindow), new FrameworkPropertyMetadata(typeof(SharpWindow)));
        }

        public static readonly DependencyProperty IsMainWindowProperty = DependencyProperty.Register("IsMainWindow", typeof(bool), typeof(SharpWindow));
        public static readonly DependencyProperty IsExitEnabledProperty = DependencyProperty.Register("IsExitEnabled", typeof(bool), typeof(SharpWindow));

        public bool IsMainWindow
        {
            get { return (Boolean)GetValue(IsMainWindowProperty); }
            set { SetValue(IsMainWindowProperty, value); }
        }

        public bool IsExitEnabled
        {
            get { return (Boolean)GetValue(IsExitEnabledProperty); }
            set { SetValue(IsExitEnabledProperty, value); }
        }

        public SharpWindow()
        {
            Style = (Style)FindResource("SharpWindowStyle");
            IsExitEnabled = true;
        }
    }
}
