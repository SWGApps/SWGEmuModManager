using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Views
{
    /// <summary>
    /// Interaction logic for ExamineWindow.xaml
    /// </summary>
    public partial class ExamineWindow : Window
    {
        public ExamineWindow(MainWindowViewModelResponses.ModsDisplay modDisplay)
        {
            InitializeComponent();

            DataContext = new ExamineWindowViewModel(modDisplay);
        }
    }
}
