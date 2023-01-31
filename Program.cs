using Fastenshtein;

namespace ClashCodes
{
    public static class Program
    {
        public static void Main()
        {
            var readRepository = new MaterialReadRepository("Materials.csv");
            var clashWriteRepository = new MaterialWriteRepository("Materials-Clash.csv");
            var nonClashWriteRepository = new MaterialWriteRepository("Materials-Non-Clash.csv");

            var materials = readRepository.List();

            // [x] TODO - Get codes distinctly
            // [X] TODO - For each code get all material ocurrences
            // [X] TODO - Compare records to one another
            // [X] TODO - If one of them is not compatible with the levenshtein one third/33% add to clash group
            // [X] TODO - Otherwise add to non-clash

            var ids = materials
                .Select(m => m.Id)
                .Distinct()
                .ToList();

            var previousPercentage = 0.00M;

            for (int index = 0; index < ids.Count; index++)
            {
                var id = ids[index];

                var narrowedMaterials = materials
                    .Where(m => m.Id == id)
                    .ToList();

                var countById = narrowedMaterials.Count;

                if (countById > 1)
                {
                    var sample = narrowedMaterials.First();

                    var hasClashes = false;

                    foreach (var material in narrowedMaterials.Skip(1))
                    {
                        if
                        (
                            Levenshtein.Distance(sample.Description, material.Description) >
                            Math.Max(sample.Description.Length, material.Description.Length) / 3 * 2
                        )
                        {
                            hasClashes = true;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (hasClashes)
                    {
                        narrowedMaterials.ForEach(clashWriteRepository.Add);
                    }
                    else
                    {
                        narrowedMaterials.ForEach(nonClashWriteRepository.Add);
                    }
                }

                var percentage = decimal.Round(index / (decimal)ids.Count * 100M, 1);

                if (percentage != previousPercentage)
                {
                    previousPercentage = percentage;
                    Console.Clear();
                    Console.WriteLine($"Progress: {percentage}");
                }
            }
        }
    }
}