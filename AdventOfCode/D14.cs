using System.Text;

namespace AdventOfCode;

public static class D14
{
    private static (string Template, Dictionary<string, string> InsertionRules) GetInput()
    {
        string[] input = File.ReadAllText("./Inputs/D14.txt").Split("\r\n");
        string template = input[0];
        var rules = input[2..].ToDictionary(line => line[0..2], line => line[^1..]);
        return (template, rules);
    }

    private static string InsertBetweenPairs(string template, Dictionary<string, string> insertionRules)
    {
        StringBuilder sb = new(template);
        int inserted = 0;
        for (int i = 0; i < template.Length - 1; i++)
        {
            string key = template[i..(i + 2)];
            sb.Replace(key, key[0] + insertionRules[key] + key[1], i + inserted++, 2);
        }
        return sb.ToString();
    }

    private static string SolveA(int steps)
    {
        var (template, rules) = GetInput();
        string result = Enumerable.Range(0, steps)
            .Aggregate(template, (agg, _) => InsertBetweenPairs(agg, rules));
        return result;
    }

    public static int SolveA()
    {
        var result = SolveA(steps: 10);
        var counts = result.GroupBy(c => c).Select(g => g.Count());
        return counts.Max() - counts.Min();
    }

    private static Dictionary<string, Products> 
        ToProductionDictionary(this Dictionary<string, string> insertionRules) =>
            insertionRules
            .ToDictionary(rule => rule.Key,
                          rule => new Products(First: rule.Key[0] + rule.Value, Second: rule.Value + rule.Key[1]));

    private static Dictionary<string, long> ProcessSteps(Dictionary<string, long> coupleCountPairs,
        Dictionary<string, Products> productionDictionary, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Dictionary<string, long> coupleCountPairsCopy = new(coupleCountPairs);
            foreach (var pair in coupleCountPairs)
            {
                var products = productionDictionary[pair.Key];
                coupleCountPairsCopy[pair.Key] -= pair.Value;
                coupleCountPairsCopy[products.First] += pair.Value;
                coupleCountPairsCopy[products.Second] += pair.Value;
            }
            coupleCountPairs = coupleCountPairsCopy;
        }
        return coupleCountPairs;
    }

    public static long SolveB()
    {
        var (template, rules) = GetInput();
        var productions = rules.ToProductionDictionary();
        char firstSymbol = template.First();
        var coupleCountPairs = rules.Keys.ToDictionary(key => key, _ => 0L);

        for (int i = 0; i < template.Length - 1; i++)
            coupleCountPairs[template[i..(i + 2)]]++;

        coupleCountPairs = ProcessSteps(coupleCountPairs, productions, 40);
        Dictionary<char, long> symbolCountPairs =
            coupleCountPairs
            .GroupBy(kvp => kvp.Key[1])
            .ToDictionary(group => group.Key, group => group.Sum(kvp => kvp.Value));
        symbolCountPairs[firstSymbol]++;

        return symbolCountPairs.Max(pair => pair.Value) - symbolCountPairs.Min(pair => pair.Value);
    }

    private record Products(string First, string Second);
}