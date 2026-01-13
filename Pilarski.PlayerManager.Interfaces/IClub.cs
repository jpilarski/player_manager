using Pilarski.PlayerManager.Core;

namespace Pilarski.PlayerManager.Interfaces
{
    public interface IClub
    {
        int Id { get; set; }
        string Name { get; set; }
        Country Country { get; set; }
        string? ImagePath { get; set; }
        int FoundingYear { get; set; }
        int? RivalClubId { get; set; }
        string? Stadium { get; set; }
    }
}
