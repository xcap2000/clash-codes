namespace ClashCodes
{
    public class Material
    {
        public Material(string id, string description)
        {
            Id = id;
            Description = description;
        }

        public string Id { get; }
        public string Description { get; }
    }
}