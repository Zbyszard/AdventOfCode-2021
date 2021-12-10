namespace AdventOfCode;

public static class D10
{
    private static string[] GetInput() =>
        File.ReadAllText("./Inputs/D10.txt")
        .Split("\r\n");

    private static readonly Dictionary<char, char> OpenClosePairs =
        new()
        {
            { '(', ')' },
            { '{', '}' },
            { '[', ']' },
            { '<', '>' }
        };

    private static int CorruptedCharToScore(char c) => c switch
    {
        ')' => 3,
        ']' => 57,
        '}' => 1197,
        '>' => 25137,
        _ => throw new ArgumentException("Only ')', ']', '}' and '>' characters are allowed.")
    };

    private static long CompletedCharToScore(char c) => c switch
    {
        ')' => 1,
        ']' => 2,
        '}' => 3,
        '>' => 4,
        _ => throw new ArgumentException("Only ')', ']', '}' and '>' characters are allowed.")
    };

    private static int ScoreFromCorruptedLine(string line)
    {
        Stack<char> stack = new();
        foreach (char c in line)
        {
            if (OpenClosePairs.ContainsKey(c))
                stack.Push(c);
            else if (OpenClosePairs[stack.Peek()] != c)
                return CorruptedCharToScore(c);
            else
                stack.Pop();
        }
        return 0;
    }

    public static int SolveA() => GetInput().Sum(ScoreFromCorruptedLine);

    private static IEnumerable<char> ReconstructEnding(Stack<char> stack) =>
        stack.Select(c => OpenClosePairs[c]);

    private static long ScoreFromIncompleteLine(string line)
    {
        Stack<char> stack = new();
        foreach (char c in line)
        {
            if (OpenClosePairs.ContainsKey(c))
                stack.Push(c);
            else
                stack.Pop();
        }

        return ReconstructEnding(stack)
            .Aggregate(0L, (score, c) => 5 * score + CompletedCharToScore(c));
    }

    public static long SolveB()
    {
        var scores = GetInput()
            .Where(line => ScoreFromCorruptedLine(line) == 0)
            .Select(ScoreFromIncompleteLine)
            .OrderBy(score => score)
            .ToArray();
        return scores[scores.Length / 2];
    }
}
