using SWGEmuModManager.Models;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private async void Window_Initialized(object sender, System.EventArgs e)
        {
            await ConfigFile.GenerateNewConfig();
        }
    }
}
