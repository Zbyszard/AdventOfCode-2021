namespace AdventOfCode;

public static class D1
{
    private static int[] GetInput() => 
        File.ReadAllText("./Inputs/D1.txt").Split("\r\n").Select(i => int.Parse(i)).ToArray();

    private static int GetLargerSum(int[] input)
    {
        int first = input.First();
        
        int output = input[1..].Aggregate((prev: first, larger: 0), (acc, i) =>
        {
            if (acc.prev < i) 
                acc.larger++;
            acc.prev = i;
            return acc;
        }, acc => acc.larger);
        return output;
    }

    public static int SolveA()
    {
        int[] input = GetInput();
        return GetLargerSum(input);
    }

    public static int SolveB()
    {
        int[] input = GetInput();
        int[] threeMeasurements = input.Select<int, int?>((measurement, index) =>
        {
            if (index + 3 > input.Length)
                return null;
            return input[index..(index + 3)].Sum();
        }).Where(i => i is not null)
        .Select(i => i!.Value)
        .ToArray();

        return GetLargerSum(threeMeasurements);
    }
}
