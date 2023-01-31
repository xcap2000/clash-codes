namespace ClashCodes
{
    public class MaterialWriteRepository
    {
        private readonly string filename;

        public MaterialWriteRepository(string filename)
        {
            this.filename = filename;
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            File.AppendAllText(filename, $"Matnr,Desc{Environment.NewLine}");
        }

        public void Add(Material material)
        {
            string description = material.Description.Contains(',')
                ? "\"" + material.Description + "\""
                : material.Description;
            var line = $"{material.Id},{description}{Environment.NewLine}";
            File.AppendAllText(filename, line);
        }
    }
}