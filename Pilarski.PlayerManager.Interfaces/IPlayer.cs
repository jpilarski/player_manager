using Pilarski.PlayerManager.Core;

namespace Pilarski.PlayerManager.Interfaces
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }
        string? ImagePath { get; set; }
        int? ClubId { get; set; }
        Country Nationality { get; set; }
        int BirthYear { get; set; }
        int MarketValue { get; set; }
        Position Position { get; set; }
        PreferredFoot Foot { get; set; }
        int SkillMoves { get; set; }
        int WeakFoot { get; set; }
        int Overall { get; set; }
    }
}
