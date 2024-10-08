﻿using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string Rolename { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
