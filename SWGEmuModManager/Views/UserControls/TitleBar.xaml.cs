using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SWGEmuModManager.Views.UserControls;

/// <summary>
/// Interaction logic for TitleBar.xaml
/// </summary>
public partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }

    private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Application.Current.MainWindow!.DragMove();
        }
    }
}
