using Pilarski.PlayerManager.BL;
using Pilarski.PlayerManager.Core;
using Pilarski.PlayerManager.Interfaces;
using Pilarski.PlayerManager.UI.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Pilarski.PlayerManager.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private PlayerManagerBL _bl = null!;
        public ObservableCollection<IPlayer> Players { get; set; } = new ObservableCollection<IPlayer>();
        public ObservableCollection<IClub> Clubs { get; set; } = new ObservableCollection<IClub>();
        public IPlayer? SelectedPlayer { get; set; }
        public IClub? SelectedClub { get; set; }
        public ICommand AddPlayerCommand { get; set; } = null!;
        public ICommand AddClubCommand { get; set; } = null!;
        public ICommand DeletePlayerCommand { get; set; } = null!;
        public ICommand DeleteClubCommand { get; set; } = null!;
        public ICommand EditPlayerCommand { get; set; } = null!;
        public ICommand EditClubCommand { get; set; } = null!;
        public MainViewModel()
        {
            try
            {
                _bl = new PlayerManagerBL();
                AddPlayerCommand = new RelayCommand(AddPlayer);
                AddClubCommand = new RelayCommand(AddClub);
                DeletePlayerCommand = new RelayCommand(DeletePlayer);
                DeleteClubCommand = new RelayCommand(DeleteClub);
                EditPlayerCommand = new RelayCommand(EditPlayer);
                EditClubCommand = new RelayCommand(EditClub);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical Error loading database: {ex.Message}");
                Application.Current.Shutdown();
            }
        }
        private void LoadData()
        {
            var playersList = _bl.GetPlayers();
            var clubsList = _bl.GetClubs();
            Players.Clear();
            foreach (var p in playersList) Players.Add(p);
            Clubs.Clear();
            foreach (var c in clubsList) Clubs.Add(c);
        }
        private void AddPlayer()
        {
            var newPlayer = _bl.CreatePlayer();
            var editor = new PlayerWindow();
            editor.DataContext = newPlayer;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    _bl.AddPlayer(newPlayer);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding player: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void AddClub()
        {
            // 1. Create empty club via BL
            var newClub = _bl.CreateClub();

            // 2. Open Window
            var editor = new ClubWindow();
            editor.DataContext = newClub;

            if (editor.ShowDialog() == true)
            {
                try
                {
                    // 3. Save via BL
                    _bl.AddClub(newClub);

                    // 4. Reload lists
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding club: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void DeletePlayer()
        {
            if (SelectedPlayer == null) return;
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedPlayer.Name}?",
                                         "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bl.DeletePlayer(SelectedPlayer.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting player: {ex.Message}");
                }
            }
        }
        private void DeleteClub()
        {
            if (SelectedClub == null) return;
            var result = MessageBox.Show($"Are you sure you want to delete {SelectedClub.Name}?\n\nWARNING: All players in this club will become Free Agents!",
                                         "Confirm Club Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bl.DeleteClub(SelectedClub.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting club: {ex.Message}");
                }
            }
        }
        private void EditPlayer()
        {
            if (SelectedPlayer == null) return;

            // 1. Tworzymy "Brudnopis" (pusty obiekt)
            IPlayer draft = _bl.CreatePlayer();

            // 2. Kopiujemy dane z oryginału do brudnopisu
            CopyPlayerValues(SelectedPlayer, draft);

            // 3. Otwieramy okno, ale wiążemy je z BRUDNOPISEM (draft), a nie oryginałem
            var editor = new PlayerWindow();
            editor.DataContext = draft;

            if (editor.ShowDialog() == true)
            {
                try
                {
                    // 4. Jeśli user kliknął SAVE: Przepisujemy zmiany z brudnopisu do oryginału
                    CopyPlayerValues(draft, SelectedPlayer);

                    // 5. Zapisujemy oryginał w bazie
                    _bl.UpdatePlayer(SelectedPlayer);

                    // 6. Odświeżamy widok
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating player: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Jeśli wystąpił błąd zapisu, LoadData przywróci stare dane z bazy
                    LoadData();
                }
            }
            // Jeśli user kliknął CANCEL: Nic nie robimy. Oryginał (SelectedPlayer) nigdy nie został tknięty.
        }

        private void EditClub()
        {
            if (SelectedClub == null) return;

            // 1. Brudnopis
            IClub draft = _bl.CreateClub();
            CopyClubValues(SelectedClub, draft);

            var editor = new ClubWindow();
            editor.DataContext = draft; // Edytujemy kopię

            if (editor.ShowDialog() == true)
            {
                try
                {
                    // Zatwierdzamy zmiany: Kopia -> Oryginał
                    CopyClubValues(draft, SelectedClub);

                    _bl.UpdateClub(SelectedClub);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating club: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadData();
                }
            }
        }
        // Przepisuje dane z źródła (source) do celu (target)
        private void CopyPlayerValues(IPlayer source, IPlayer target)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Nationality = source.Nationality;
            target.ClubId = source.ClubId;
            target.BirthYear = source.BirthYear;
            target.MarketValue = source.MarketValue;
            target.Position = source.Position;
            target.Foot = source.Foot;
            target.SkillMoves = source.SkillMoves;
            target.WeakFoot = source.WeakFoot;
            target.Overall = source.Overall;
            target.ImagePath = source.ImagePath;
        }

        private void CopyClubValues(IClub source, IClub target)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Country = source.Country;
            target.FoundingYear = source.FoundingYear;
            target.RivalClubId = source.RivalClubId;
            target.Stadium = source.Stadium;
            target.ImagePath = source.ImagePath;
        }
    }
}
