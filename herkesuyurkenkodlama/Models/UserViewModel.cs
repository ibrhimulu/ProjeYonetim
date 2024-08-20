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
        public string? ProfileImagePath { get; set; }
    }

    public class CreateUserModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(30, ErrorMessage = "Karakter yetmezliği.Tekrar deneyin.")]
        [RegularExpression(@"^[a-zA-ZğüşöçıĞÜŞÖÇİ]+$", ErrorMessage = "Kullanıcı adı yalnızca harflerden oluşmalıdır.")]
        public string Username { get; set; }
        public string? NameSurname { get; set; }

        [Required(ErrorMessage = "Bir rol seçiniz.")]
        public int? RoleId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? DepartmentId { get; set; }
        public int? SubDepartmentId { get; set; }
        public bool? IsActive { get; set; }
        public string? ProfileImagePath { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalıdır.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakterden oluşmalıdır.")]
        [RegularExpression(@"^(?=.*[a-zğüşöçı])(?=.*[A-ZĞÜŞÖÇİ])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-zğüşöçıĞÜŞÖÇİ\d@$!%*?&.]{6,15}$",
        ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalıdır.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakterden oluşmalıdır.")]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        [RegularExpression(@"^(?=.*[a-zğüşöçı])(?=.*[A-ZĞÜŞÖÇİ])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-zğüşöçıĞÜŞÖÇİ\d@$!%*?&.]{6,15}$",
        ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")]
        public string RePassword { get; set; }
    }
}
