using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SWGEmuModManager.Views.UserControls;

/// <summary>
/// Interaction logic for TitleBarRightCorner.xaml
/// </summary>
public partial class TitleBarRightCorner : UserControl
{
    public TitleBarRightCorner()
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
