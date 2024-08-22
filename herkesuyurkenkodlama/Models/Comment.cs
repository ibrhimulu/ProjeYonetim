using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string? CommentText { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }

        public virtual Mdepartment? Department { get; set; }
        public virtual Sdepartment? SubDepartment { get; set; }
        public virtual Tasklar Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
