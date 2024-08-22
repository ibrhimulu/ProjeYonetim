namespace herkesuyurkenkodlama.Models
{
    public class TeamViewModel
    {
        public int SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; } = null!;
        public int? DepartmentId { get; set; }
        public bool? IsActive { get; set; }
    }
}
