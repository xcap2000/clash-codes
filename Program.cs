using System.Threading.Tasks.Dataflow;

namespace ClashCodes;

public static class Program
{
    private static int materialCount = 0;
    private static volatile int processed = 0;

    public static async Task Main()
    {
        var dataflowBlockOptions = new DataflowBlockOptions
        {
            EnsureOrdered = true,
            BoundedCapacity = 10
        };

        var dataBuffer = new BufferBlock<List<Material>>(dataflowBlockOptions);
        var consumerTask = ConsumeAsync(dataBuffer);

        await Produce(dataBuffer);

        await consumerTask;
    }

    public static async Task Produce(ITargetBlock<List<Material>> target)
    {
        var readRepository = new MaterialReadRepository("Materials.csv");

        var materials = readRepository.List();

        materialCount = materials.Count;

        while (true)
        {
            if (materials.Count == 0)
            {
                break;
            }

            List<Material> narrowedMaterials = GetMaterials(materials);

            if (narrowedMaterials.Count > 1)
            {
                await target.SendAsync(narrowedMaterials);
            }

            materials.RemoveRange(0, narrowedMaterials.Count);
        }

        target.Complete();
    }

    private static List<Material> GetMaterials(List<Material> materials)
    {

        var firstMaterial = materials[0];
        firstMaterial.CalculateUpper();

        var narrowedMaterials = new List<Material>
        {
            firstMaterial
        };

        for (int index = 1; index < materials.Count; index++)
        {
            var currentMaterial = materials[index];

            if (currentMaterial.Id == firstMaterial.Id)
            {
                currentMaterial.CalculateUpper();
                narrowedMaterials.Add(currentMaterial);
            }
            else
            {
                break;
            }
        }

        return narrowedMaterials;
    }

    public static async Task ConsumeAsync(ISourceBlock<List<Material>> source)
    {
        var previousPercentage = 0.00M;

        var materialWriteRepository = new MaterialWriteRepository("Materials-Clash-Groups.csv");

        while (await source.OutputAvailableAsync())
        {
            List<Material> narrowedMaterials = await source.ReceiveAsync();

            var levenshteinRanges = new List<LevenshteinRange>
            {
                new LevenshteinRange(100),
                new LevenshteinRange(80),
                new LevenshteinRange(60),
                new LevenshteinRange(40),
                new LevenshteinRange(20),
                new LevenshteinRange(0),
            };

            var greatestRange = 0;

            for (int index = 1; index < narrowedMaterials.Count; index++)
            {
                foreach (var levenshteinRange in levenshteinRanges)
                {
                    if
                    (
                        levenshteinRange.Falls
                        (
                            narrowedMaterials[0].DescriptionUpper,
                            narrowedMaterials[index].DescriptionUpper
                        )
                    )
                    {
                        if (levenshteinRange.DiscrepancyPercentage > greatestRange)
                        {
                            greatestRange = levenshteinRange.DiscrepancyPercentage;
                        }
                    }
                }
            }

            foreach (var material in narrowedMaterials)
            {
                material.Range = greatestRange;
                materialWriteRepository.Add(material);
            }

            processed += narrowedMaterials.Count;

            var percentage = decimal.Round(processed / (decimal)materialCount * 100M, 1, MidpointRounding.ToZero);

            if (percentage > previousPercentage)
            {
                previousPercentage = percentage;
                Console.Clear();
                Console.WriteLine($"Progress: {percentage}%");
                Console.WriteLine($"{processed:###,###,###} records processed of {materialCount:###,###,###}");
            }
        }
    }
}