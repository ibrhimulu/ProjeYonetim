namespace herkesuyurkenkodlama.Models
{
    public class TasklarViewModel
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public int AssignedUserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int StatusId { get; set; }
        public string? TaskDescription { get; set; }
    }
    public class CreateTasklarModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public int AssignedUserId { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int StatusId { get; set; }
        public string? TaskDescription { get; set; }
    }
    public class EditTasklarModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = null!;
        public int AssignedUserId { get; set; }
        public bool IsActive { get; set; }        
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int StatusId { get; set; }
        public string? TaskDescription { get; set; }
    }

}
