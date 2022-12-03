namespace Playground;

public partial class Program
{
    public static void Main(string[] args)
    {
        GenerateDbContext a = new GenerateDbContext(null);
        a.Customer.ToList();
    }
}