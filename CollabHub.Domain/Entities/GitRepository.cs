using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class GitRepository:BaseEntity
    {
        [Key]
        public int RepositoryId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        //public int TaskHeadId { get; set; }
        //public TaskHead TaskHead { get; set; }
        [Required]
        public string RepoName { get; set; }
        [Required]
        public string RepoUrl { get; set; }
        public string DefaultBranch { get; set; }="main";
        public bool IsPrivate { get; set; } = false;
        public string? Description { get; set; }
        public string? LastCommitId { get; set; }

        [MaxLength(500)]
        public string? LastCommitMessage { get; set; }

        public DateTime? LastCommitDate { get; set; }
        public ICollection<GitActivity> GitActivities { get; set; } =new List<GitActivity>();


    }
}
