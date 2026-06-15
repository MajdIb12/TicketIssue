using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TicketIssue.Infrastructure.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=TicketEngineDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}