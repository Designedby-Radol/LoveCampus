namespace CampusLove.Domain.Entities
{
    public class Gender
    {
        public int Id { get; set; }
        public required string Description { get; set; }

        public Gender()
        {
            Description = string.Empty;
        }

        public override string ToString()
        {
            return Description ?? string.Empty;
        }
    }
}