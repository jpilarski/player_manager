using Pilarski.PlayerManager.Core;
using Pilarski.PlayerManager.Interfaces;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Pilarski.PlayerManager.BL
{
    public class PlayerManagerBL
    {
        private IDataAccess _dataAccess = null!;
        public PlayerManagerBL(string libraryName = "Pilarski.PlayerManager.DAOSQL.dll")
        {
            LoadDataAccessStrategy(libraryName);
        }
        private void LoadDataAccessStrategy(string libraryDllName)
        {
            string? dllPath = FindLibraryPath(libraryDllName);
            if (string.IsNullOrEmpty(dllPath))
            {
                throw new FileNotFoundException($"Library not found: {libraryDllName}. Ensure the DAO project is built.");
            }
            var assembly = Assembly.LoadFrom(dllPath);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IDataAccess).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            if (type == null)
            {
                throw new Exception($"Class implementing IDataAccess not found in library {libraryDllName}.");
            }
            _dataAccess = (IDataAccess)Activator.CreateInstance(type)!;
        }
        private string? FindLibraryPath(string fileName)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(Path.Combine(currentPath, fileName)))
                return Path.Combine(currentPath, fileName);
            DirectoryInfo? dir = new DirectoryInfo(currentPath);
            while (dir != null && dir.Parent != null)
            {
                var files = dir.GetFiles("*.sln");
                if (files.Length > 0)
                {
                    var foundFiles = Directory.GetFiles(dir.FullName, fileName, SearchOption.AllDirectories);
                    return foundFiles.FirstOrDefault(f => f.Contains("bin") && f.Contains("Debug") && f.Contains("net9.0"));
                }
                dir = dir.Parent;
            }
            return null;
        }
        private void ValidatePlayer(IPlayer player)
        {
            if (string.IsNullOrWhiteSpace(player.Name))
                throw new ArgumentException("Player Name is required.");
            if (player.Name.Length < 2)
                throw new ArgumentException("Player Name is too short (min. 2 characters).");
            if (!Regex.IsMatch(player.Name, @"^[a-zA-Z\s\-']+$"))
                throw new ArgumentException("Player Name contains invalid characters. Only letters, spaces, hyphens, and apostrophes are allowed.");
            if (player.BirthYear < 1900 || player.BirthYear > 2025)
                throw new ArgumentException("Birth year must be between 1900 and 2025.");
            if (player.MarketValue < 0 || player.MarketValue > 15_000_000)
                throw new ArgumentException("Market Value must be between 0 and 15,000,000.");
            if (player.Overall < 40 || player.Overall > 99)
                throw new ArgumentException("Overall rating must be between 40 and 99.");
            if (player.SkillMoves < 1 || player.SkillMoves > 5)
                throw new ArgumentException("Skill Moves must be between 1 and 5.");
            if (player.WeakFoot < 1 || player.WeakFoot > 5)
                throw new ArgumentException("Weak Foot must be between 1 and 5.");
            if (!Enum.IsDefined(typeof(Position), player.Position))
                throw new ArgumentException("Invalid Position selected.");
            if (!Enum.IsDefined(typeof(Country), player.Nationality))
                throw new ArgumentException("Invalid Nationality selected.");
            if (!Enum.IsDefined(typeof(PreferredFoot), player.Foot))
                throw new ArgumentException("Invalid Preferred Foot selected.");
            if (player.ClubId.HasValue)
            {
                var clubs = _dataAccess.GetAllClubs();
                if (!clubs.Any(c => c.Id == player.ClubId.Value))
                {
                    throw new ArgumentException("Selected Club does not exist.");
                }
            }
        }
        private void ValidateClub(IClub club)
        {
            if (string.IsNullOrWhiteSpace(club.Name))
                throw new ArgumentException("Club Name is required.");
            if (club.Name.Length < 2)
                throw new ArgumentException("Club Name is too short (min. 2 characters).");
            if (!Regex.IsMatch(club.Name, @"^[a-zA-Z0-9\s\-'/]+$"))
                throw new ArgumentException("Club Name contains invalid characters. Allowed characters: letters, numbers, spaces, hyphens (-), apostrophes ('), and slashes (/).");
            if (!string.IsNullOrWhiteSpace(club.Stadium))
            {
                if (club.Stadium.Length < 2)
                    throw new ArgumentException("Stadium Name is too short.");
                if (!Regex.IsMatch(club.Stadium, @"^[a-zA-Z0-9\s\-'/\.]+$"))
                    throw new ArgumentException("Stadium Name contains invalid characters. Allowed characters: letters, numbers, spaces, hyphens (-), apostrophes ('), slashes (/), and dots (.).");
            }
            if (club.FoundingYear < 1850 || club.FoundingYear > 2025)
                throw new ArgumentException("Founding year must be between 1850 and 2025.");
            if (!Enum.IsDefined(typeof(Country), club.Country))
                throw new ArgumentException("Invalid Country selected.");
            if (club.RivalClubId.HasValue)
            {
                if (club.RivalClubId.Value == club.Id)
                    throw new ArgumentException("A club cannot be its own rival.");
                var clubs = _dataAccess.GetAllClubs();
                if (!clubs.Any(c => c.Id == club.RivalClubId.Value))
                    throw new ArgumentException("Selected Rival Club does not exist.");
            }
        }
        public List<IPlayer> GetPlayers()
        {
            return _dataAccess.GetAllPlayers();
        }
        public void AddPlayer(IPlayer player)
        {
            ValidatePlayer(player);
            _dataAccess.AddPlayer(player);
        }
        public void UpdatePlayer(IPlayer player)
        {
            ValidatePlayer(player);
            _dataAccess.UpdatePlayer(player);
        }
        public void DeletePlayer(int id)
        {
            _dataAccess.DeletePlayer(id);
        }
        public List<IClub> GetClubs()
        {
            return _dataAccess.GetAllClubs();
        }
        public void AddClub(IClub club)
        {
            ValidateClub(club);
            _dataAccess.AddClub(club);
        }
        public void UpdateClub(IClub club)
        {
            ValidateClub(club);
            _dataAccess.UpdateClub(club);
        }
        public void DeleteClub(int clubId)
        {
            var allPlayers = _dataAccess.GetAllPlayers();
            var playersInClub = allPlayers.Where(p => p.ClubId == clubId).ToList();
            foreach (var player in playersInClub)
            {
                player.ClubId = null;
                _dataAccess.UpdatePlayer(player);
            }
            var allClubs = _dataAccess.GetAllClubs();
            var clubsWithRival = allClubs.Where(c => c.RivalClubId == clubId).ToList();
            foreach (var c in clubsWithRival)
            {
                c.RivalClubId = null;
                _dataAccess.UpdateClub(c);
            }
            _dataAccess.DeleteClub(clubId);
        }
        public IPlayer CreatePlayer()
        {
            return _dataAccess.CreatePlayer();
        }
        public IClub CreateClub()
        {
            return _dataAccess.CreateClub();
        }
    }
}
