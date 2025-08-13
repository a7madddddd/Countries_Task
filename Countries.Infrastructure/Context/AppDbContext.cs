using System;
using System.Collections.Generic;
using Countries.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Countries.Infrastructure.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-513178E;Database=Countries;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC074D0CC6F6");

            entity.HasIndex(e => e.Code, "IX_Countries_Code");

            entity.HasIndex(e => e.IsDeleted, "IX_Countries_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_Countries_Name");

            entity.HasIndex(e => e.Name, "UQ__Countrie__737584F65AE4CC37").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__Countrie__A25C5AA70974E032").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(5);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
