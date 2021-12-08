namespace AdventOfCode;

public static class D8
{
    private static IEnumerable<PatternsOutputsPair> GetInput() =>
        File.ReadAllText("./Inputs/D8.txt")
        .Split("\r\n")
        .Select(line =>
        {
            string[] parts = line.Split(" | ");
            string patterns = parts[0];
            string output = parts[1];
            return new PatternsOutputsPair(patterns.Split(' '), output.Split(' '));
        });

    private static readonly Dictionary<int, int> LengthNumberPairs = new()
    {
        { 2, 1 },
        { 3, 7 },
        { 4, 4 },
        { 7, 8 }
    };

    public static int SolveA()
    {
        IEnumerable<string[]> outputs = GetInput().Select(patternOutput => patternOutput.Outputs);
        return outputs
            .Sum(fourDigits => fourDigits.Count(digitString => LengthNumberPairs.ContainsKey(digitString.Length)));
    }

    private static int InferNumber(Dictionary<HashSet<char>, int> knownMappings, HashSet<char> pattern) =>
        (knownMappings.First(m => m.Value == 4).Key.Intersect(pattern).Count(),
        knownMappings.First(m => m.Value == 1).Key.Intersect(pattern).Count(),
        knownMappings.First(m => m.Value == 7).Key.Intersect(pattern).Count(),
        knownMappings.First(m => m.Value == 8).Key.Intersect(pattern).Count()) switch
        {
            (2, _, _, _) => 2, // only 2 has two common segments with 4
            (4, _, _, _) => 9, // only 9 has 4 common segments with 4
            (_, 2, 3, 6) => 0, // only 0 has 2 common segments with 1, 3 with 7 and 6 with 8
            (_, 2, 3, 5) => 3, // etc.
            (_, 1, 2 ,5) => 5,
            (_, 1, 2, 6) => 6,
            _ => throw new ArgumentException("Invalid argument", nameof(knownMappings))
        };

    private static int SolveLine(PatternsOutputsPair pair)
    {
        Dictionary<HashSet<char>, int> mappings = pair.Patterns
            .Where(pattern => LengthNumberPairs.ContainsKey(pattern.Length))
            .ToDictionary(pair => pair.ToHashSet(), pair => LengthNumberPairs[pair.Length]);

        IEnumerable<HashSet<char>> unknownNumbers = pair.Patterns
            .Where(pattern => !LengthNumberPairs.ContainsKey(pattern.Length))
            .Select(p => p.ToHashSet());

        foreach(var pattern in unknownNumbers)
            mappings.Add(pattern, InferNumber(mappings, pattern));

        Dictionary<string, int> stringToNum = mappings
            .ToDictionary(mapping => string.Concat(mapping.Key.OrderBy(c => c)), mapping => mapping.Value);
        var outputs = pair.Outputs.Select(s => string.Concat(s.OrderBy(c => c)));

        return int.Parse(string.Concat(outputs
            .Select(o => stringToNum[o].ToString())));
    }

    public static int SolveB() => GetInput().Select(SolveLine).Sum();

    private record struct PatternsOutputsPair(string[] Patterns, string[] Outputs);
}
