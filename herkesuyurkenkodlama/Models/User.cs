using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Projects = new HashSet<Project>();
            Tasks = new HashSet<Task>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? NameSurname { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public bool? IsActive { get; set; }
        public string? ProfileImagePath { get; set; }

        public virtual Mdepartment Department { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual Sdepartment SubDepartment { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
