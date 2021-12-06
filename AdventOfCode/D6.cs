namespace AdventOfCode;

public static class D6
{
    private static IEnumerable<int> GetInput() =>
        File.ReadAllText("./Inputs/D6.txt")
        .Split(',')
        .Select(int.Parse);

    private static void AdvanceTimeA(List<int> nums, int days)
    {
        while (days-- > 0)
        {
            int numsToAdd = 0;
            for(int i = 0; i < nums.Count; i++)
            {
                if (nums[i] > 0)
                    nums[i]--;
                else
                {
                    nums[i] = 6;
                    numsToAdd++;
                }
            }
            while (numsToAdd-- > 0)
                nums.Add(8);
        }
    }

    public static int SolveA()
    {
        var input = GetInput().ToList();
        AdvanceTimeA(input, 80);
        return input.Count;
    }

    private static long AdvanceTimeB(List<int> inputNums, int days)
    {
        //number, count
        Dictionary<int, long> numbers = Enumerable.Range(0, 9).ToDictionary(i => i, _ => 0L);
        foreach (int num in inputNums)
            numbers[num]++;

        while(days-- > 0)
        {
            var newNumbers = numbers
                .Where(pair => pair.Key > 0)
                .ToDictionary(pair => pair.Key - 1, pair => pair.Value);
            newNumbers.Add(8, numbers[0]);
            newNumbers[6] += numbers[0];
            numbers = newNumbers;
        }
        return numbers.Sum(pair => pair.Value);
    }

    public static long SolveB()
    {
        var input = GetInput().ToList();
        return AdvanceTimeB(input, 256);
    }
}
