namespace AdventOfCode;

public static class D7
{
    private static IEnumerable<int> GetInput() =>
        File.ReadAllText("./Inputs/D7.txt")
        .Split(',')
        .Select(int.Parse);

    private static int Median(IList<int> numbers) => numbers.Count % 2 == 1 ?
            numbers[(numbers.Count - 1) / 2 + 1] :
            (numbers[numbers.Count / 2] + numbers[numbers.Count / 2 + 1]) / 2;

    public static int SolveA()
    {
        int[] input = GetInput().OrderBy(i => i).ToArray();
        int med = Median(input);
        return input.Sum(i => Math.Abs(med - i));
    }

    public static int SolveB()
    {
        int[] input = GetInput().ToArray();
        int avg = (int)input.Average();
        return input.Select(i => Math.Abs(avg - i))
            .Select(n => (2 + n - 1) * n / 2) // sum of arithmetic progression with step = 1
            .Sum();
    }
}
