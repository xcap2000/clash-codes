namespace ClashCodes
{
    public static class Program
    {
        public static void Main()
        {
            var readRepository = new MaterialReadRepository();
            var clashWriteRepository = new MaterialWriteRepository("Materials-Clash.csv");
            var nonClashWriteRepository = new MaterialWriteRepository("Materials-Non-Clash.csv");

            foreach (var material in readRepository.List())
            {
                Console.WriteLine($"ID: {material.Id}, Description: {material.Description}");

                clashWriteRepository.Add(material);
                nonClashWriteRepository.Add(material);
            }
        }
    }
}