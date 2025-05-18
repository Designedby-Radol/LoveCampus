using System;

namespace CampusLove.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchDate { get; set; }
        
        // Navigation properties
        public User User1 { get; set; }
        public User User2 { get; set; }

        public Match()
        {
            MatchDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Match with {User2?.FormattedName ?? "Unknown"} on {MatchDate:d}";
        }

        public User GetMatchedUser(int currentUserId)
        {
            return User1Id == currentUserId ? User2 : User1;
        }
    }
}