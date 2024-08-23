namespace herkesuyurkenkodlama.Models
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        public int? OwnerUserId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
    }
    public class CreateProjectModel
    {
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        public int? OwnerUserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
    }
    public class EditProjectModel
    {
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        public int? OwnerUserId { get; set; }
        public bool IsActive { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
    }
}
