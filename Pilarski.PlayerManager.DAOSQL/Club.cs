using Pilarski.PlayerManager.Core;
using Pilarski.PlayerManager.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Pilarski.PlayerManager.DAOSQL
{
    [Table("Clubs")]
    public class Club : IClub
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Country Country { get; set; }
        public string? ImagePath { get; set; }
        public int FoundingYear { get; set; }
        public int? RivalClubId { get; set; }
        public string? Stadium { get; set; }
        public virtual List<Player> Players { get; set; } = new();
    }
}
