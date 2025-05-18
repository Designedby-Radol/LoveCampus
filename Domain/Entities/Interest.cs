namespace CampusLove.Domain.Entities
{
    public class Interest
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}