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
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsActive { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }

        public virtual Mdepartment Department { get; set; } = null!;
        public virtual Sdepartment SubDepartment { get; set; } = null!;
        public virtual Task Task { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
