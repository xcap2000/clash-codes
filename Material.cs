using System.Diagnostics;

namespace ClashCodes;

[DebuggerDisplay("ID = {Id}, Description = {Description}")]
public class Material
{
    public Material(string id, string description)
    {
        Id = id;
        Description = description;
    }

    public string Id { get; }
    public string Description { get; }
    public string DescriptionUpper { get; private set; } = "";
    public int Range { get; set; }

    internal void CalculateUpper()
    {
        DescriptionUpper = Description.ToUpper();
    }
}