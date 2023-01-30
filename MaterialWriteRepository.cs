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
            var line = $"{material.Id},{material.Description}{Environment.NewLine}";
            File.AppendAllText(filename, line);
        }
    }
}