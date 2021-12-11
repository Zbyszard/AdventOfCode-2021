namespace AdventOfCode;

public static class D11
{
    private static int ToInt(char c) => int.Parse(c.ToString());

    private static int[][] GetInput() =>
        File.ReadAllText("./Inputs/D11.txt")
        .Split("\r\n")
        .Select(s => s.Select(ToInt).ToArray())
        .ToArray();

    private static int[] GetNeighbouringIndexes(int i, int dimLength) =>
        (i == 0, i == dimLength - 1) switch
        {
            (true, _) => new[] { i, i + 1 },
            (_, true) => new[] { i - 1, i },
            _ => new[] { i - 1, i, i + 1 }
        };

    private static Octopus[,] ToOctopusMatrix(int[][] input)
    {
        var octopuses = new Octopus[input.Length, input[0].Length];

        for (int i = 0; i < input.Length; i++)
            for (int j = 0; j < input[i].Length; j++)
                octopuses[i, j] = new() { EnergyLevel = input[i][j] };

        for (int i = 0; i < input.Length; i++)
            for (int j = 0; j < input[i].Length; j++)
            {
                int[] availableIs = GetNeighbouringIndexes(i, input.Length);
                int[] availableJs = GetNeighbouringIndexes(j, input[i].Length);
                foreach (int ii in availableIs)
                    foreach (int jj in availableJs)
                    {
                        if (ii == i && jj == j)
                            continue;
                        octopuses[i, j].Neighbours.Add(octopuses[ii, jj]);
                    }
            }
        return octopuses;
    }

    private static void IluminateNeighbours(Octopus oct)
    {
        foreach (var neighbour in oct.Neighbours.Where(n => n.EnergyLevel > 0))
        {
            neighbour.EnergyLevel = ++neighbour.EnergyLevel % 10;
            if (neighbour.EnergyLevel == 0)
                IluminateNeighbours(neighbour);
        }
    }

    private static int Step(Octopus[,] octopuses)
    {
        foreach (var oct in octopuses)
            oct.EnergyLevel = ++oct.EnergyLevel % 10;

        foreach (var oct in octopuses.Cast<Octopus>().Where(o => o.EnergyLevel == 0).ToList())
            IluminateNeighbours(oct);

        return octopuses.Cast<Octopus>().Count(o => o.EnergyLevel == 0);
    }

    public static int SolveA()
    {
        var octopuses = ToOctopusMatrix(GetInput());
        int flashes = 0;

        for (int i = 0; i < 100; i++)
            flashes += Step(octopuses);

        return flashes;
    }

    public static long SolveB()
    {
        var octopuses = ToOctopusMatrix(GetInput());
        int step = 0;

        while (true)
        {
            step++;
            Step(octopuses);
            if (octopuses.Cast<Octopus>().Count(o => o.EnergyLevel == 0) == octopuses.Length)
                break;
        }

        return step;
    }

    private class Octopus
    {
        public ICollection<Octopus> Neighbours { get; set; }
        public int EnergyLevel { get; set; }
        public Octopus()
        {
            Neighbours = new List<Octopus>();
        }
    }
}
