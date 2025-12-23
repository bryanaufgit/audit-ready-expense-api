using AuditReadyExpense.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace AuditReadyExpense.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Expense> Expenses => Set<Expense>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(b =>
        {
            b.ToTable("expenses");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title).IsRequired();
            b.Property(x => x.Amount).IsRequired();
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.CreatedByUserId).IsRequired();

            b.Ignore(x => x.AuditEvents);
            
            // Indexe (Performance / Reporting)
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatedAtUtc);

            // AuditEvents: private field backing collection
            b.OwnsMany<AuditEvent>("_auditEvents", ab =>
            {
                ab.ToTable("expense_audit_events");
                ab.WithOwner().HasForeignKey("ExpenseId");

                ab.Property<Guid>("Id");
                ab.HasKey("Id");

                ab.Property(x => x.FromStatus).IsRequired();
                ab.Property(x => x.ToStatus).IsRequired();
                ab.Property(x => x.ActorUserId).IsRequired();
                ab.Property(x => x.OccurredAtUtc).IsRequired();
                ab.Property(x => x.Reason);

                ab.HasIndex("ExpenseId");
                ab.HasIndex(x => x.OccurredAtUtc);
            });

        });

        base.OnModelCreating(modelBuilder);
    }
}