using System.ComponentModel.DataAnnotations;

namespace herkesuyurkenkodlama.Models
{
    public class UserViewModel
    {

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? NameSurname { get; set; }
        public int RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
        public bool? IsActive { get; set; }
    }
}
