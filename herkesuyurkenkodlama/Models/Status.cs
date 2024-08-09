using System;
using System.Collections.Generic;

namespace herkesuyurkenkodlama.Models
{
    public partial class Status
    {
        public Status()
        {
            Tasks = new HashSet<Task>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
