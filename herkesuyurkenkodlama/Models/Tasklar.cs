using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Tasklar
    {
        public Tasklar()
        {
            Comments = new HashSet<Comment>();
        }

        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public int AssignedUserId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
        public int StatusId { get; set; }

        public virtual User AssignedUser { get; set; } = null!;
        public virtual Mdepartment? Department { get; set; }
        public virtual Project Project { get; set; } = null!;
        public virtual Status Status { get; set; } = null!;
        public virtual Sdepartment? SubDepartment { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
