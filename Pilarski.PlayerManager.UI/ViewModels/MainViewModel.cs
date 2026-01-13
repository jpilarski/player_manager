using Microsoft.Win32;
using Pilarski.PlayerManager.BL;
using Pilarski.PlayerManager.Core;
using Pilarski.PlayerManager.Interfaces;
using Pilarski.PlayerManager.UI.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Pilarski.PlayerManager.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly PlayerManagerBL _bl = null!;
        public ObservableCollection<IPlayer> Players { get; set; } = new ObservableCollection<IPlayer>();
        public ObservableCollection<IClub> Clubs { get; set; } = new ObservableCollection<IClub>();

        private IPlayer? _selectedPlayer;
        public IPlayer? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                OnPropertyChanged(nameof(SelectedPlayer));
                IsAddingOrEditingPlayer = false;
            }
        }

        private IClub? _selectedClub;
        public IClub? SelectedClub
        {
            get => _selectedClub;
            set
            {
                _selectedClub = value;
                OnPropertyChanged(nameof(SelectedClub));
                IsAddingOrEditingClub = false;
            }
        }

        private bool _isAddingOrEditingClub;
        public bool IsAddingOrEditingClub
        {
            get => _isAddingOrEditingClub;
            set
            {
                _isAddingOrEditingClub = value;
                OnPropertyChanged(nameof(IsAddingOrEditingClub));
            }
        }

        private IClub? _clubInEdit;
        public IClub? ClubInEdit
        {
            get => _clubInEdit;
            set
            {
                _clubInEdit = value;
                OnPropertyChanged(nameof(ClubInEdit));
            }
        }

        private bool _isAddingOrEditingPlayer;
        public bool IsAddingOrEditingPlayer
        {
            get => _isAddingOrEditingPlayer;
            set
            {
                _isAddingOrEditingPlayer = value;
                OnPropertyChanged(nameof(IsAddingOrEditingPlayer));
            }
        }

        public ICommand AddPlayerCommand { get; }
        public ICommand AddClubCommand { get; }
        public ICommand DeletePlayerCommand { get; }
        public ICommand DeleteClubCommand { get; }
        public ICommand EditPlayerCommand { get; }
        public ICommand EditClubCommand { get; }
        public ICommand SaveClubCommand { get; }
        public ICommand CancelClubEditCommand { get; }
        public ICommand SelectLogoCommand { get; }

        public Array Countries => Enum.GetValues(typeof(Country));

        public MainViewModel()
        {
            try
            {
                _bl = new PlayerManagerBL();
                AddPlayerCommand = new RelayCommand(AddPlayer);
                AddClubCommand = new RelayCommand(AddClub);
                DeletePlayerCommand = new RelayCommand(DeletePlayer, () => SelectedPlayer != null);
                DeleteClubCommand = new RelayCommand(DeleteClub, () => SelectedClub != null);
                EditPlayerCommand = new RelayCommand(EditPlayer, () => SelectedPlayer != null);
                EditClubCommand = new RelayCommand(EditClub, () => SelectedClub != null);
                
                SaveClubCommand = new RelayCommand(SaveClub, () => ClubInEdit != null);
                CancelClubEditCommand = new RelayCommand(CancelClubEdit);
                SelectLogoCommand = new RelayCommand(SelectLogo, () => ClubInEdit != null);

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
            // Logic to be implemented
        }

        private void AddClub()
        {
            SelectedClub = null;
            ClubInEdit = _bl.CreateClub();
            IsAddingOrEditingClub = true;
        }

        private void DeletePlayer()
        {
            // Logic to be implemented
        }

        private void DeleteClub()
        {
            // Logic to be implemented
        }

        private void EditPlayer()
        {
            // Logic to be implemented
        }

        private void EditClub()
        {
            if (SelectedClub == null) return;
            ClubInEdit = _bl.CreateClub();
            CopyClubValues(SelectedClub, ClubInEdit);
            IsAddingOrEditingClub = true;
        }

        private void SaveClub()
        {
            if (ClubInEdit == null) return;

            // Handle logo copy
            if (!string.IsNullOrEmpty(ClubInEdit.ImagePath) && File.Exists(ClubInEdit.ImagePath))
            {
                string destinationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "PlayerManagerData", "clubs");
                Directory.CreateDirectory(destinationFolder);
                string destinationPath = Path.Combine(destinationFolder, Path.GetFileName(ClubInEdit.ImagePath));
                
                if (ClubInEdit.ImagePath != destinationPath)
                {
                    try
                    {
                        File.Copy(ClubInEdit.ImagePath, destinationPath, true);
                        ClubInEdit.ImagePath = destinationPath;
                    }
                    catch (Exception ex)
                    {
                         MessageBox.Show($"Error copying image: {ex.Message}");
                         // Decide if we should continue saving without the image or stop
                         return;
                    }
                }
            }
            
            try
            {
                if (ClubInEdit.Id == 0) // New club
                {
                    _bl.AddClub(ClubInEdit);
                }
                else // Existing club
                {
                    _bl.UpdateClub(ClubInEdit);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Validation Error: {ex.Message}");
                return;
            }
            
            LoadData();
            IsAddingOrEditingClub = false;
            ClubInEdit = null;
        }

        private void CancelClubEdit()
        {
            IsAddingOrEditingClub = false;
            ClubInEdit = null;
        }

        private void SelectLogo()
        {
            if (ClubInEdit == null) return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ClubInEdit.ImagePath = openFileDialog.FileName;
                OnPropertyChanged(nameof(ClubInEdit)); // Notify view that the property has changed
            }
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
