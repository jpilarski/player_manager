using Pilarski.PlayerManager.Interfaces;

namespace Pilarski.PlayerManager.DAOSQL
{
    public class SQLDataAccess : IDataAccess
    {
        private PlayerManagerContext _context;
        public IPlayer CreatePlayer()
        {
            return new Player();
        }
        public IClub CreateClub()
        {
            return new Club();
        }
        public SQLDataAccess()
        {
            _context = new PlayerManagerContext();
            _context.Database.EnsureCreated();
        }
        public List<IPlayer> GetAllPlayers()
        {
            return _context.Players.ToList<IPlayer>();
        }
        public void AddPlayer(IPlayer player)
        {
            if (player is Player p)
            {
                _context.Players.Add(p);
                _context.SaveChanges();
            }
        }
        public void UpdatePlayer(IPlayer player)
        {
            if (player is Player p)
            {
                _context.Players.Update(p);
                _context.SaveChanges();
            }
        }
        public void DeletePlayer(int playerId)
        {
            var player = _context.Players.FirstOrDefault(x => x.Id == playerId);
            if (player != null)
            {
                _context.Players.Remove(player);
                _context.SaveChanges();
            }
        }
        public List<IClub> GetAllClubs()
        {
            return _context.Clubs.ToList<IClub>();
        }
        public void AddClub(IClub club)
        {
            if (club is Club c)
            {
                _context.Clubs.Add(c);
                _context.SaveChanges();
            }
        }
        public void UpdateClub(IClub club)
        {
            if (club is Club c)
            {
                _context.Clubs.Update(c);
                _context.SaveChanges();
            }
        }
        public void DeleteClub(int clubId)
        {
            var club = _context.Clubs.FirstOrDefault(x => x.Id == clubId);
            if (club != null)
            {
                _context.Clubs.Remove(club);
                _context.SaveChanges();
            }
        }
    }
}
