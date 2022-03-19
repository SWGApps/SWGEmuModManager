using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SWGEmuModManager.Views.UserControls;

/// <summary>
/// Interaction logic for TitleBarLeftCorner.xaml
/// </summary>
public partial class TitleBarLeftCorner : UserControl
{
    public TitleBarLeftCorner()
    {
        InitializeComponent();
    }

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Application.Current.MainWindow!.DragMove();
        }
    }
}
