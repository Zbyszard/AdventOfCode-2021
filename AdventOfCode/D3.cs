namespace AdventOfCode;

public static class D3
{
    private static byte[][] GetInput() => File.ReadAllText("./Inputs/D3.txt")
        .Split("\r\n")
        .Select(str => str.Select(c => Convert.ToByte(c == '1')).ToArray())
        .ToArray();

    private static byte[][] Transpose(byte[][] input)
    {
        byte[][] output = new byte[input[0].Length][];
        for (int i = 0; i < output.Length; i++)
            output[i] = new byte[input.Length];

        for (int i = 0; i < input.Length; i++)
            for (int j = 0; j < input[0].Length; j++)
                output[j][i] = input[i][j];
        return output;
    }

    public static int SolveA()
    {
        int epsilon = 0;
        int gamma = 0;
        int mask = 0;
        byte[][] columns = Transpose(GetInput());
        for (int i = 0; i < columns.Length; i++)
            mask |= 1 << i;

        int shift = columns.Length - 1;
        foreach (byte[] col in columns)
        {
            byte colValue = col.GroupBy(i => i)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .First();
            epsilon |= colValue << shift--;
        }
        gamma = ~epsilon & mask; // & with mask to ignore leading 1s

        return epsilon * gamma;
    }

    public static int SolveB()
    {
        byte[][] columns = Transpose(GetInput());
        return OxygenGeneratorRating(columns) * Co2ScrubberRating(columns);
    }

    private static int OxygenGeneratorRating(byte[][] bitColumns) =>
        GetRating(bitColumns, column => column.Count(b => b == 1) >= (column.Length / 2f) ? 1 : 0);

    private static int Co2ScrubberRating(byte[][] bitColumns) =>
        GetRating(bitColumns, column => column.Count(b => b == 1) >= (column.Length / 2f) ? 0 : 1);

    private static int GetRating(byte[][] bitColumns, Func<byte[], int> criterionFunc)
    {
        for (int position = 0; position < bitColumns.Length; position++)
        {
            int criterion = criterionFunc(bitColumns[position]);
            int[] excludedRows = Enumerable
                .Range(0, bitColumns[position].Length)
                .Where(i => bitColumns[position][i] != criterion).ToArray();
            bitColumns = bitColumns
                .Select(col => Enumerable.Range(0, col.Length)
                    .Select(rowNum => excludedRows.Contains(rowNum) ? (byte?)null : col[rowNum])
                    .Where(b => b is not null)
                    .Select(b => b!.Value)
                    .ToArray()
                ).ToArray();
            if (bitColumns[position].Length == 1)
                break;
        }
        int shift = bitColumns.Length - 1;
        int result = 0;
        foreach (var bit in bitColumns.SelectMany(b => b))
            result |= bit << shift--;
        return result;
    }
}
