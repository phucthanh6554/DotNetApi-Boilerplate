using PlaygroundConsole;

namespace Playground;

public partial class Program
{
    public static void Main(string[] args)
    {
        var context = new GenerateDbContext(null);
        var wrapper = new RepositoryWrapper(context);
        
    }
}

