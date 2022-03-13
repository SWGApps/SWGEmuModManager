using System.Diagnostics;
using System.Windows.Controls;

namespace SWGEmuModManager.Views.UserControls;

/// <summary>
/// Interaction logic for ModList.xaml
/// </summary>
public partial class ModList : UserControl
{
    public ModList()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri, 
            UseShellExecute = true
        });

        e.Handled = true;
    }
}
