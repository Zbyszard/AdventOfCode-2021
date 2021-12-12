using System.Diagnostics;

IEnumerable<Type> types = Enumerable.Range(1, 25)
    .Select(i => Type.GetType("AdventOfCode.D" + i))
    .Where(t => t is not null)
    .Select(t => t!);

if (Debugger.IsAttached)
    InvokeSolutions(types.Last());
else
    foreach (var type in types)
        InvokeSolutions(type);

static void InvokeSolutions(Type type)
{
    Console.WriteLine(type.GetMethod("SolveA")!.Invoke(null, null));
    Console.WriteLine(type.GetMethod("SolveB")!.Invoke(null, null));
}