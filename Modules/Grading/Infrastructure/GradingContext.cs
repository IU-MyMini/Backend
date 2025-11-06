using BuildingBlocks.Infrastructure.Configuration;

using GradingModule.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Infrastructure;

public class GradingContext(DbContextOptions options) : AppDbContext(options)
{
    public DbSet<User> Users { get; set; }

    // ReSharper disable once RedundantOverriddenMember
    // Ignore in GradingContext, use in others
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(modelBuilder);
}