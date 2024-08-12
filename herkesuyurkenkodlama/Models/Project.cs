﻿using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Project
    {
        public Project()
        {
            Tasks = new HashSet<Task>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        public int OwnerUserId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }

        public virtual Mdepartment Department { get; set; } = null!;
        public virtual User OwnerUser { get; set; } = null!;
        public virtual Sdepartment SubDepartment { get; set; } = null!;
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
