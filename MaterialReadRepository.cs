namespace ClashCodes
{
    public class MaterialReadRepository
    {
        public List<Material> List()
        {
            var materials = new List<Material>();

            var lines = File.ReadAllLines("Materials.csv");

            for (int index = 1; index < lines.Length; index++)
            {
                var line = lines[index];
                var id = line[..line.IndexOf(',')];
                var description = line.Substring(line.IndexOf(',') + 1, line.Length - (id.Length + 1)).Trim('"');
                var material = new Material(id, description);
                materials.Add(material);
            }

            return materials
                .OrderBy(m => m.Id)
                .ThenBy(m => m.Description)
                .ToList();
        }
    }
}