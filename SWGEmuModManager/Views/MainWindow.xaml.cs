using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using SWGEmuModManager.Util;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) &&
                Keyboard.IsKeyDown(Key.LeftAlt) &&
                Keyboard.IsKeyDown(Key.F1))
            {
                using var dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();
                string directory = "";

                if (result.ToString().Trim() == "Cancel")
                {
                    Close();
                }
                else if (result.ToString().Trim() == "OK")
                {
                    directory = dialog.SelectedPath.Replace("\\", "/");
                }

                ManifestGenerator.GenerateModManifest(directory);
            }
        }
    }
}
