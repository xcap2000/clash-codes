using Fastenshtein;

namespace ClashCodes;

public class LevenshteinRange
{
    public LevenshteinRange(int discrepancyPercentage)
    {
        DiscrepancyPercentage = discrepancyPercentage;
    }

    public int DiscrepancyPercentage { get; }

    public bool Falls(string value1, string value2)
    {
        if (DiscrepancyPercentage == 100)
        {
            return Levenshtein.Distance(value1, value2) == Math.Max(value1.Length, value2.Length);
        }
        else
        {
            return Levenshtein.Distance(value1, value2) >= Math.Max(value1.Length, value2.Length) / (100 - DiscrepancyPercentage);
        }
    }
}