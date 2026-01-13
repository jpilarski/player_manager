using System.Windows;

namespace Pilarski.PlayerManager.UI
{
    public partial class PlayerWindow : Window
    {
        public PlayerWindow()
        {
            InitializeComponent();
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}