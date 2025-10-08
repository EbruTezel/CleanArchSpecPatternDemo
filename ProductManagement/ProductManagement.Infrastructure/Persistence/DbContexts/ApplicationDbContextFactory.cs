using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductManagement.Infrastructure.Persistence.DbContexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
       
        optionsBuilder.UseSqlServer("Server=localhost,1435;Database=ProductManagementDB;User Id=sa;Password=SqlPassword123;TrustServerCertificate=true;MultipleActiveResultSets=true");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
    
}