using System.Collections.ObjectModel;

namespace AdventOfCode;

public static class D12
{
    private static IEnumerable<KeyValuePair<string, string>> GetInput() =>
        File.ReadAllText("./Inputs/D12.txt")
        .Split("\r\n")
        .Select(s =>
        {
            var arr = s.Split('-');
            return new KeyValuePair<string, string>(arr[0], arr[1]);
        });

    public static IEnumerable<T> DistinctKeysAndValues<T>(this IEnumerable<KeyValuePair<T, T>> pairs) =>
        pairs.Select(p => p.Key)
        .Union(pairs.Select(p => p.Value));

    private static Cave ToCave(string name) => new(name);

    private static IEnumerable<Cave> ToCaves(IEnumerable<string> names) => names.Select(ToCave);

    private static void CreateCaveSystem(IEnumerable<KeyValuePair<Cave, Cave>> connections)
    {
        foreach (var (cave1, cave2) in connections)
        {
            if (cave2.Name != "start")
                cave1.ConnectedTo.Add(cave2);
            if (cave1.Name != "start" && cave2.Name != "end")
                cave2.ConnectedTo.Add(cave1);
        }
    }

    private static HashSet<Cave> GetCaveSystem()
    {
        var input = GetInput();
        var names = input.DistinctKeysAndValues();
        var caves = ToCaves(names).ToList();
        var connections = input
            .Select(p => new KeyValuePair<Cave, Cave>(caves.Single(c => c.Name == p.Key),
                                                      caves.Single(c => c.Name == p.Value)));
        CreateCaveSystem(connections);
        return caves.Where(c => c.ConnectedTo.Any() || c.Name == "end").ToHashSet();
    }

    private static void FillPaths(List<List<Cave>> paths, List<Cave> currentPath,
        Cave currentCave, Dictionary<Cave, int> visitedLittleCaves, int maxVisitsForSingleLittleCave)
    {
        List<Cave> path = new(currentPath) { currentCave };
        foreach (var cave in currentCave.ConnectedTo)
        {
            if (cave.Name == "end")
            {
                paths.Add(path.Append(cave).ToList());
                continue;
            }
            if (!cave.IsBig && visitedLittleCaves.ContainsKey(cave) 
                && visitedLittleCaves.ContainsValue(maxVisitsForSingleLittleCave))
                continue;
            var newVisited = visitedLittleCaves;
            if (!cave.IsBig)
            {
                newVisited = new(visitedLittleCaves);
                if (newVisited.ContainsKey(cave))
                    newVisited[cave]++;
                else
                    newVisited.Add(cave, 1);
            }
            FillPaths(paths, path, cave, newVisited, maxVisitsForSingleLittleCave);
        }
    }

    private static void Print(List<List<Cave>> paths)
    {
        foreach (string s in paths.Select(path => string.Join(',', path.Select(c => c.Name))).OrderBy(s => s))
            Console.WriteLine(s);
    }

    private static int Solve(int maxVisitsForSingleLittleCave)
    {
        var system = GetCaveSystem();
        List<List<Cave>> paths = new();
        FillPaths(paths, currentPath: new(), system.Single(c => c.Name == "start"), visitedLittleCaves: new(),
            maxVisitsForSingleLittleCave);
        return paths.Count;
    }

    public static int SolveA() => Solve(1);

    public static int SolveB() => Solve(2);

    private class Cave
    {
        public string Name { get; init; }
        public bool IsBig => Name.Any(c => c is >= 'A' and <= 'Z') || Name == "start" || Name == "end";
        public ICollection<Cave> ConnectedTo { get; }

        public Cave(string name)
        {
            Name = name;
            ConnectedTo = new HashSet<Cave>();
        }
    }
}