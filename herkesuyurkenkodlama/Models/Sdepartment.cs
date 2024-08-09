using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Sdepartment
    {
        public Sdepartment()
        {
            Comments = new HashSet<Comment>();
            Projects = new HashSet<Project>();
            Tasks = new HashSet<Task>();
            Users = new HashSet<User>();
        }

        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; } = null!;
        public int DepartmentId { get; set; }
        public bool? IsActive { get; set; }

        public virtual Mdepartment Department { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
