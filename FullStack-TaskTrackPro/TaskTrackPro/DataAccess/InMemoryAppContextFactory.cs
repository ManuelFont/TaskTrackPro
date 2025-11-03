using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class InMemoryAppContextFactory
{
    public SqlContext CreateDbContext(string dbName)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlContext>();
        optionsBuilder.UseInMemoryDatabase("TestingDB");

        return new SqlContext(optionsBuilder.Options);
    }
}