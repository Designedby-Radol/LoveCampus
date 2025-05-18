using System;

namespace CampusLove.Domain.Entities
{
    public class Career
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}