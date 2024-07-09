using MagicOnion;
using MagicOnion.Server;
using MagicOnionShared;

namespace MagicOnionServer;

public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
{
    public async UnaryResult<int> SumAsync(int x, int y)
    {
        Console.WriteLine($"Received:{x}, {y}");
        return x + y;
    }
}