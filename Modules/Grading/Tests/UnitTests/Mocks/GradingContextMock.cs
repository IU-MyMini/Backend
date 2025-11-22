using BuildingBlocks.Domain;

using GradingModule.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Tests.UnitTests.Mocks;

public class GradingContextMock(DbContextOptions options) : GradingContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var e in modelBuilder.Model.GetEntityTypes())
        {
            e.ClrType.GetProperties()
                .Where(
                    p => p.PropertyType == typeof(LangStr)
                         || (p.PropertyType.IsGenericType
                             && p.PropertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                )
                .ToList()
                .ForEach(p => modelBuilder.Entity(e.Name).Property(p.Name).HasConversion<LangStrConverter>());
        }
    }
}