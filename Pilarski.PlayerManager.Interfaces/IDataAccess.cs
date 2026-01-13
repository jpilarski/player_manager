using Pilarski.PlayerManager.Core;

namespace Pilarski.PlayerManager.Interfaces
{
    public interface IDataAccess
    {
        IPlayer CreatePlayer();
        IClub CreateClub();
        List<IPlayer> GetAllPlayers();
        void AddPlayer(IPlayer player);
        void UpdatePlayer(IPlayer player);
        void DeletePlayer(int playerId);
        List<IClub> GetAllClubs();
        void AddClub(IClub club);
        void UpdateClub(IClub club);
        void DeleteClub(int clubId);
    }
}
