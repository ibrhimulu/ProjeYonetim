namespace herkesuyurkenkodlama.Models
{
    public class HeadshipViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmanName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
    public class CreateHeadshipModel
    {
        public int DepartmentId { get; set; }
        public string DepartmanName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
    public class EditHeadshipModel
    {
        public int DepartmentId { get; set; }
        public string DepartmanName { get; set; } = null!;
        public bool IsActive { get; set; }
    }

}
