namespace AdventOfCode;

public static class D9
{
    private static int CharToInt(char ch) => int.Parse(ch.ToString());

    private static int[][] GetInput() =>
        File.ReadAllText("./Inputs/D9.txt")
        .Split("\r\n")
        .Select(str => str.Select(CharToInt).ToArray())
        .ToArray();

    private static (int i, int j)[] GetNeighbouringPositions(int i, int j, int iLength, int jLength)
    {
        if (i == 0 && j == 0) return new[] { (0, 1), (1, 0) };
        if (i == 0 && j == jLength - 1) return new[] { (1, j), (0, j - 1) };
        if (i == 0 && j < jLength - 1) return new[] { (0, j - 1), (1, j), (0, j + 1) };
        if (i < iLength - 1 && j == 0) return new[] { (i - 1, 0), (i, 1), (i + 1, 0) };
        if (i == iLength - 1 && j == 0) return new[] { (i, 1), (i - 1, 0) };
        if (i == iLength - 1 && j < jLength - 1) return new[] { (i, j - 1), (i - 1, j), (i, j + 1) };
        if (i < iLength - 1 && j == jLength - 1) return new[] { (i - 1, j), (i, j - 1), (i + 1, j) };
        if (i == iLength - 1 && j == jLength - 1) return new[] { (i, j - 1), (i - 1, j) };
        return new[] { (i, j - 1), (i - 1, j), (i, j + 1), (i + 1, j) };
    }

    private static int[][] FilterLowPoints(int[][] heightMap)
    {
        int[][] output = new int[heightMap.Length][];
        for (int i = 0; i < heightMap.Length; i++)
        {
            output[i] = Enumerable.Range(0, heightMap[i].Length)
                .Select(_ => -1).ToArray();
        }

        for (int i = 0; i < heightMap.Length; i++)
            for (int j = 0; j < heightMap[i].Length; j++)
            {
                var neighbours = GetNeighbouringPositions(i, j, heightMap.Length, heightMap[i].Length);
                (int i, int j) lowest = (i, j);
                foreach (var neighbour in neighbours)
                    if (heightMap[neighbour.i][neighbour.j] <= heightMap[i][j])
                        lowest = neighbour;
                if (lowest == (i, j))
                    output[i][j] = heightMap[i][j];
            }
        return output;
    }

    public static int SolveA() => FilterLowPoints(GetInput())
        .SelectMany(ba => ba)
        .Where(i => i != -1)
        .Sum(i => i + 1);

    private static IEnumerable<int> FindBasinSizes(HeightMapInfo heightMap, int[][] lowPoints)
    {
        for(int i = 0; i < heightMap.Rows; i++)
            for(int j = 0; j < heightMap.Columns; j++)
            {
                if (lowPoints[i][j] == -1 || heightMap[i,j].Checked)
                    continue;

                heightMap[i, j].Checked = true;
                var basinPoints = GetHigherPoints(i, j).ToList();
                while (basinPoints.Any(point => !point.Checked))
                {
                    for(int k = 0; k < basinPoints.Count; k++)
                    {
                        if (basinPoints[k].Checked)
                            continue;
                        basinPoints[k].Checked = true;
                        basinPoints.AddRange(GetHigherPoints(basinPoints[k].i, basinPoints[k].j));

                        foreach (var point in basinPoints.Where(p => !p.Selected))
                            point.Selected = true;
                    }
                }
                basinPoints.Add(heightMap[i, j]);
                yield return basinPoints.Count;
            }

        IEnumerable<PointInfo> GetHigherPoints(int i, int j) =>
            GetNeighbouringPositions(i, j, heightMap.Rows, heightMap.Columns)
            .Select(pos => heightMap[pos.i, pos.j])
            .Where(point => point.Value > heightMap[i, j].Value && point.Value < 9 && !point.Selected);
    }

    public static int SolveB()
    {
        var input = GetInput();
        var lowPoints = FilterLowPoints(input);
        var heightMap = new HeightMapInfo(input);
        return FindBasinSizes(heightMap, lowPoints)
            .OrderByDescending(size => size)
            .Take(3)
            .Aggregate(1, (product, i) => product * i);
    }

    private record PointInfo(int Value, int i, int j)
    {
        public bool Checked { get; set; }
        public bool Selected { get; set; }
    };

    private class HeightMapInfo
    {
        private readonly PointInfo[][] _points;
        public PointInfo this[int i, int j] => _points[i][j];
        public int Rows { get; init; }
        public int Columns { get; init; }

        public HeightMapInfo(int[][] heightMap)
        {
            _points = new PointInfo[heightMap.Length][];
            for (int i = 0; i < heightMap.Length; i++)
            {
                _points[i] = new PointInfo[heightMap[i].Length];
                for(int j = 0; j < heightMap[i].Length; j++)
                    _points[i][j] = new PointInfo(heightMap[i][j], i, j);
            }
            Rows = heightMap.Length;
            Columns = heightMap[0].Length;
        }
    }
}
