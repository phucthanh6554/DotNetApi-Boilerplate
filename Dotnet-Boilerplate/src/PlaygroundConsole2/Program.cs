using PlaygroundConsole2;

internal class Program
{
    private static void Main(string[] args)
    {
        var context = new GenerateDbContext(null);
        var wrapper = new RepositoryWrapper(context);
    }
}