using CollabHub.Domain.Commom;
using CollabHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Infrastructure.Persistence.Data
{
    public  class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<RegisterAudit> RegisterAudits { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<TaskHead> TaskHeads { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<TeamMember>()
                .HasOne(t => t.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.TeamName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(U=>U.RegisterAudit)
                .WithOne(r=>r.User)
                .HasForeignKey<RegisterAudit>(r=>r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoginAudit>()
                .HasOne(l=>l.User)
                .WithMany(u=>u.LoginAudits)
                .HasForeignKey(l=>l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskHead>()
                .HasOne(th => th.Team)
                .WithMany(t => t.TaskHeads)
                .HasForeignKey(th => th.TeamId)
                .OnDelete(DeleteBehavior.NoAction);



            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var builder = modelBuilder.Entity(entityType.ClrType);

                    builder.HasOne(typeof(User), nameof(BaseEntity.CreatedByUser))
                        .WithMany()
                        .HasForeignKey(nameof(BaseEntity.CreatedBy))
                        .OnDelete(DeleteBehavior.NoAction);

                    builder.HasOne(typeof(User), nameof(BaseEntity.ModifiedByUser))
                        .WithMany()
                        .HasForeignKey(nameof(BaseEntity.ModifiedBy))
                        .OnDelete(DeleteBehavior.NoAction);

                    builder.HasOne(typeof(User), nameof(BaseEntity.DeletedByUser))
                        .WithMany()
                        .HasForeignKey(nameof(BaseEntity.DeletedBy))
                        .OnDelete(DeleteBehavior.NoAction);
                }
            }

        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedOn = DateTime.UtcNow;
                        entry.State = EntityState.Modified;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    
     }
}
