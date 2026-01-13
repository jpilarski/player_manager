using Pilarski.PlayerManager.UI.ViewModels;
using System.Windows;

namespace Pilarski.PlayerManager.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
