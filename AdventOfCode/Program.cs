foreach (int i in Enumerable.Range(1, 25))
{
    string className = "AdventOfCode.D" + i;
    var type = Type.GetType(className);
    if (type is null)
        break;

    Console.WriteLine(type?.GetMethod("SolveA")!.Invoke(null, null));
    Console.WriteLine(type?.GetMethod("SolveB")!.Invoke(null, null));
}