using CollabHub.Domain.Commom;
using CollabHub.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileResource = CollabHub.Domain.Entities.FileResource;

namespace CollabHub.Infrastructure.Persistence.Data
{
    public  class ApplicationDbContext:DbContext
    {
        private readonly IConfiguration _config;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IConfiguration config) : base(options)
        {
            _config = config;
        }
        public IDbConnection CreateConnection()=>new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
       
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<TaskHead> TaskHeads { get; set; }
        public DbSet<TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<FileResource> Files { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AiAction> AiActions { get; set; }
        public DbSet<GitRepository> GitRepositories { get; set; }
        public DbSet<GitActivity> GitActivities { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ReportUser> ReportUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TeamMember>()
                .HasOne(t => t.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Team>()
                .HasIndex(t => t.TeamName)
                .IsUnique();

            

            modelBuilder.Entity<LoginAudit>()
                .HasOne(l=>l.User)
                .WithMany(u=>u.LoginAudits)
                .HasForeignKey(l=>l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoginAudit>()
                .Property(l => l.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeamMember>()
                .Property(tm => tm.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TaskHead>()
                .HasOne(th => th.Team)
                .WithMany(t => t.TaskHeads)
                .HasForeignKey(th => th.TeamId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TaskHead>()
                .Property(th => th.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskDefinition>()
                .HasOne(td => td.TaskHead)
                .WithMany(th => th.TaskDefinitions)
                .HasForeignKey(td => td.TaskHeadId)
                .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<TaskDefinition>()
            //    .HasOne(td=>td.AssignedUser)
            //    .WithMany(u=>u.TasksAssignedTo)
            //    .HasForeignKey(td=>td.AssignedUserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskDefinition>()
                .HasOne(td => td.AssignedMember)
                .WithMany(tm => tm.TasksAssignedTo)
                .HasForeignKey(td => td.AssignedMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskDefinition>()
                .HasOne(td=>td.AssignedBy)
                .WithMany(u=>u.TasksAssignedBy)
                .HasForeignKey(td=>td.AssignedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskDefinition>()
                .Property(td => td.Status)
                .HasConversion<string>();

           


            modelBuilder.Entity<FileResource>()
                 .Property(f => f.FileSizeInKB)
                 .HasPrecision(18, 2);
            modelBuilder.Entity<FileResource>()
                .Property(f => f.ContextType)
                .HasConversion<string>();

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Team)
                .WithMany(t => t.ChatMessages)
                .HasForeignKey(cm => cm.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm=>cm.Sender)
                .WithMany(u=>u.SentMessages)
                .HasForeignKey(cm=>cm.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm=>cm.File)
                .WithMany()
                .HasForeignKey(cm=>cm.FileId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);
                
                entity.HasOne(n=>n.User)
                .WithMany(u=>u.NotificationsReceived)
                .HasForeignKey(n=>n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(n => n.Sender)
                .WithMany(u => u.NotificationsSent)
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.NoAction);


                entity.Property(n => n.Type)
                .HasConversion<string>();

                entity.Property(n => n.NotificationEntityType)
                .HasConversion<string>();

                entity.Property(n => n.Message)
                .HasMaxLength(500)
                .IsRequired();
            });

            modelBuilder.Entity<AiAction>()
                .HasOne(a => a.User)
                .WithMany(u => u.AiActions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AiAction>()
                .HasOne(a=>a.TaskDefinition)
                .WithMany(td=>td.AiActions)
                .HasForeignKey(a=>a.TaskDefinitionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AiAction>()
                .Property(a => a.ActivityType)
                .HasConversion<string>();

            modelBuilder.Entity<GitRepository>()
                .HasOne(gr=>gr.User)
                .WithMany(u=>u.GitRepositories)
                .HasForeignKey(gr=>gr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GitRepository>()
                .HasOne(gr=>gr.TaskHead)
                .WithMany(th=>th.GitRepositories)
                .HasForeignKey(gr=>gr.TaskHeadId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GitActivity>()
                .HasOne(ga => ga.User)
                .WithMany(u => u.GitActivities)
                .HasForeignKey(ga => ga.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GitActivity>()
                .HasOne(ga=>ga.Repository)
                .WithMany(gr=>gr.GitActivities)
                .HasForeignKey(ga=>ga.RepositoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GitActivity>()
                 .HasOne(ga => ga.TaskDefinition)
                 .WithMany(td => td.GitActivities)
                 .HasForeignKey(ga => ga.TaskDefinitionId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GitActivity>()
                .Property(ga => ga.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Comment>(entity =>
            {
              
                entity.HasKey(c => c.CommentId);

                
                entity.HasOne(c => c.TaskDefinition)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskDefinitionId)
                    .OnDelete(DeleteBehavior.Cascade); 

               
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict); 

               
                entity.HasOne(c => c.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(c => c.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict); 

                
                entity.Property(c => c.CommentText)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<FeedBack>(entity =>
            {
                entity.HasKey(f => f.FeedbackId);
                entity.HasOne(f => f.User)
                    .WithMany(u => u.FeedBacks)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.Property(f => f.Message)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(f=>f.Status)
                .HasConversion<string>();
            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasOne(c => c.User)
                .WithMany(u => u.ComplaintsFiled)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(c=>c.AssignedAdmin)
                .WithMany(u=>u.ComplaintsHandled)
                .HasForeignKey(c=>c.AssignedAdminId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.Property(c=>c.Priority)
                .HasConversion<string>();

                entity.Property(c => c.Status)
                .HasConversion<string>();
            });

            modelBuilder.Entity<ReportUser>(entity =>
            {
                entity.HasOne(r => r.ReportedUser)
                .WithMany(u => u.ReportedUsers)
                .HasForeignKey(r => r.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r=>r.ReportedAgainstUser)
                .WithMany(u=>u.ReportsAgainst)
                .HasForeignKey(r=>r.ReportedAgainstUserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.Status)
                .HasConversion<string>();
            });

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(p => p.User)
                .WithMany(u => u.PasswordResetTokens)
                .HasForeignKey(p => p.UserId)
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
                        entry.Entity.CreatedOn = DateTime.Now;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedOn = DateTime.Now;
                        entry.State = EntityState.Modified;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    
     }
}
