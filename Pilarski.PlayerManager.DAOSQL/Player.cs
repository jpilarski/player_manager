using System.ComponentModel.DataAnnotations.Schema;
using Pilarski.PlayerManager.Core;
using Pilarski.PlayerManager.Interfaces;

namespace Pilarski.PlayerManager.DAOSQL
{
    [Table("Players")]
    public class Player : IPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int? ClubId { get; set; }
        [ForeignKey("ClubId")]
        public virtual Club? Club { get; set; }
        public Country Nationality { get; set; }
        public int BirthYear { get; set; }
        public int MarketValue { get; set; }
        public Position Position { get; set; }
        public PreferredFoot Foot { get; set; }
        public int SkillMoves { get; set; }
        public int WeakFoot { get; set; }
        public int Overall { get; set; }
    }
}
