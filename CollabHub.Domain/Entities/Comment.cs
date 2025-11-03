using CollabHub.Domain.Commom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Domain.Entities
{
    public class Comment:BaseEntity
    {
        public int CommentId { get; set; }
        public int TaskDefinitionId { get; set; }
        public TaskDefinition TaskDefinition { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
      public string CommentText { get; set; }
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }= new List<Comment>();
    }
}
