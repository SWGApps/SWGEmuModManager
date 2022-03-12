using System.Windows;

namespace SWGEmuModManager.Themes
{
    public partial class DarkTheme
    {
        private void CloseWindow_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null) CloseWind(Window.GetWindow((FrameworkElement)e.Source)!);
        }
        private void AutoMinimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null) MaximizeRestore(Window.GetWindow((FrameworkElement)e.Source)!);
        }
        private void Minimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null) MinimizeWind(Window.GetWindow((FrameworkElement)e.Source)!);
        }

        public static void CloseWind(Window window) => window.Close();
        public static void MaximizeRestore(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
        }
        public static void MinimizeWind(Window window) => window.WindowState = WindowState.Minimized;
    }
}