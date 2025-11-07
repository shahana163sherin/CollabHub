using CollabHub.Domain.Commom;
using CollabHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class User:BaseEntity
    {
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="Enter valid Email")]
        public string Email { get; set; }
        [MinLength(6,ErrorMessage ="Password contain minimum 6 characters")]
        [Required]
        public string Password { get; set; }
        public string? ProfileImg { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginedAt { get; set; }
        public DateTime? LastEmailNotifiedAt { get; set; }
        public string? Qualification { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public ICollection<LoginAudit> LoginAudits { get; set; } = new List<LoginAudit>();
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public ICollection<TaskDefinition> TasksAssignedTo { get; set; }= new List<TaskDefinition>();
        public ICollection<TaskDefinition> TasksAssignedBy { get; set; }= new List<TaskDefinition>();
        public ICollection<FileResource> UploadedFiles { get; set; }= new List<FileResource>();
        public ICollection<ChatMessage> SentMessages { get; set; }= new List<ChatMessage>();
        public ICollection<Notification> NotificationsSent { get; set; }= new List<Notification>();
        public ICollection<Notification> NotificationsReceived { get; set; }= new List<Notification>();
        public ICollection<AiAction> AiActions { get; set; }= new List<AiAction>();
        public ICollection<GitRepository> GitRepositories { get; set; }= new List<GitRepository>();
        public ICollection<GitActivity>GitActivities { get; set; }= new List<GitActivity>();
        public ICollection<Comment> Comments { get; set; }= new List<Comment>();
        public ICollection<FeedBack> FeedBacks { get; set; }= new List<FeedBack>();
        public ICollection<Complaint> ComplaintsFiled { get; set; }= new List<Complaint>();
        public ICollection<Complaint> ComplaintsHandled { get; set; }= new List<Complaint>();
        public ICollection<ReportUser> ReportedUsers { get; set; }= new List<ReportUser>();
        public ICollection<ReportUser> ReportsAgainst { get; set; }= new List<ReportUser>();
        public ICollection<RefreshToken>RefreshTokens { get; set; }= new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();


    }
}
