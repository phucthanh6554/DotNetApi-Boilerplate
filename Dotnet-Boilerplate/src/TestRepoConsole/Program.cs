using Microsoft.EntityFrameworkCore;
using Repository;
using TestRepoConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        var option = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("demo-db")
            .Options;
        var context = new GenerateDbContext(option);
        var repoWrapper = new RepositoryWrapper(context);
    }
}