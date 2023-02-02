using System.Threading.Tasks.Dataflow;
using Fastenshtein;

namespace ClashCodes
{
    public static class Program
    {
        private static int idsCount = 0;

        public static async Task Main()
        {
            var dataflowBlockOptions = new DataflowBlockOptions
            {
                EnsureOrdered = true,
                BoundedCapacity = 5
            };

            var dataBuffer = new BufferBlock<List<Material>>(dataflowBlockOptions);
            var consumerTask = ConsumeAsync(dataBuffer);

            Produce(dataBuffer);

            await consumerTask;

            // var readRepository = new MaterialReadRepository("Materials.csv");
            // var writeRepository = new AlignedMaterialWriteRepository("Materials-New-Layout.csv");

            // var materials = readRepository.List();

            // var ids = materials
            //     .Select(m => m.Id)
            //     .Distinct()
            //     .ToList();

            // foreach (var id in ids)
            // {
            //     var narrowedMaterials = materials
            //         .Where(m => m.Id == id)
            //         .ToList();

            //     writeRepository.Add(narrowedMaterials);
            // }

            // await Task.Delay(0);
        }

        public static void Produce(ITargetBlock<List<Material>> target)
        {
            var readRepository = new MaterialReadRepository("Materials.csv");

            var materials = readRepository.List();

            var ids = materials
                .Select(m => m.Id)
                .Distinct()
                .ToList();

            idsCount = ids.Count;

            for (int index = 0; index < ids.Count; index++)
            {
                var id = ids[index];

                var narrowedMaterials = materials
                    .Where(m => m.Id == id)
                    .ToList();

                target.Post(narrowedMaterials);
            }

            target.Complete();
        }

        public static async Task ConsumeAsync(ISourceBlock<List<Material>> source)
        {
            var index = 0;

            var previousPercentage = 0.00M;

            var clashWriteRepository = new MaterialWriteRepository("Materials-Clash.csv");
            var nonClashWriteRepository = new MaterialWriteRepository("Materials-Non-Clash.csv");

            while (await source.OutputAvailableAsync())
            {
                List<Material> narrowedMaterials = await source.ReceiveAsync();

                var countById = narrowedMaterials.Count;

                if (countById > 1)
                {
                    var sample = narrowedMaterials.First();

                    var hasClashes = false;

                    foreach (var material in narrowedMaterials.Skip(1))
                    {
                        // 99%
                        // if
                        // (
                        //     Levenshtein.Distance(sample.Description, material.Description) >
                        //     Math.Max(sample.Description.Length, material.Description.Length) / 10 * 9
                        // )
                        // 100%
                        if
                        (
                            Levenshtein.Distance(sample.Description, material.Description) ==
                            Math.Max(sample.Description.Length, material.Description.Length)
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

                var percentage = decimal.Round(++index / (decimal)idsCount * 100M, 1);

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