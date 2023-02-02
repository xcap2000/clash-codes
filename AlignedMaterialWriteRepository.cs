namespace ClashCodes
{
    public class AlignedMaterialWriteRepository
    {
        private readonly string filename;

        public AlignedMaterialWriteRepository(string filename)
        {
            this.filename = filename;
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            File.AppendAllText(filename, $"Matnr,Desc,Matnr,Desc{Environment.NewLine}");
        }

        public void Add(List<Material> materials)
        {
            if (materials.Count != 1)
            {
                for (int index = 1; index < materials.Count; index++)
                {
                    if (index == 1)
                    {
                        var firstMaterial = materials[0];
                        var secondMaterial = materials[index];
                        var firstDescription = FormatDescription(firstMaterial);
                        var secondDescription = FormatDescription(secondMaterial);

                        var line = $"{firstMaterial.Id},{firstDescription},{secondMaterial.Id},{secondDescription}{Environment.NewLine}";
                        File.AppendAllText(filename, line);
                    }
                    else
                    {
                        var currentMaterial = materials[index];
                        var currentDescription = FormatDescription(currentMaterial);

                        var line = $",,{currentMaterial.Id},{currentDescription}{Environment.NewLine}";
                        File.AppendAllText(filename, line);
                    }
                }
            }
            else
            {
                var currentMaterial = materials[0];
                var currentDescription = FormatDescription(currentMaterial);

                var line = $"{currentMaterial.Id},{currentDescription},,{Environment.NewLine}";
                File.AppendAllText(filename, line);
            }
        }

        private static string FormatDescription(Material material)
        {
            return material.Description.Contains(',')
                ? "\"" + material.Description + "\""
                : material.Description;
        }
    }
}