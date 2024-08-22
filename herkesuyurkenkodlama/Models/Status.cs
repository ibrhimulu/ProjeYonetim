using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Status
    {
        public Status()
        {
            Tasklars = new HashSet<Tasklar>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Tasklar> Tasklars { get; set; }
    }
}
