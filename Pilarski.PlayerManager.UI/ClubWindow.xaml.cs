using System.Windows;

namespace Pilarski.PlayerManager.UI
{
    public partial class ClubWindow : Window
    {
        public ClubWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}