using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Mdepartment
    {
        public Mdepartment()
        {
            Comments = new HashSet<Comment>();
            Projects = new HashSet<Project>();
            Sdepartments = new HashSet<Sdepartment>();
            Tasklars = new HashSet<Tasklar>();
            Users = new HashSet<User>();
        }

        public int DepartmentId { get; set; }
        public string DepartmanName { get; set; } = null!;
        public bool? IsActive { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Sdepartment> Sdepartments { get; set; }
        public virtual ICollection<Tasklar> Tasklars { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
