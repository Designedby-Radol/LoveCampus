using System;

namespace CampusLove.Domain.Entities
{
    public class Interaction
    {
        public int Id { get; set; }
        public int SourceUserId { get; set; }
        public int TargetUserId { get; set; }
        public string Type { get; set; } // "like" or "dislike"
        public DateTime InteractionDate { get; set; }

        public Interaction()
        {
            InteractionDate = DateTime.Now;
        }

        public Interaction(int sourceUserId, int targetUserId, string type)
        {
            SourceUserId = sourceUserId;
            TargetUserId = targetUserId;
            Type = type.ToLower();
            InteractionDate = DateTime.Now;
        }
    }
}