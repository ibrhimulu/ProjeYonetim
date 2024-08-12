using System.ComponentModel.DataAnnotations;

namespace herkesuyurkenkodlama.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli.")]
        [StringLength(30, ErrorMessage = "Karakter yetmezliği.Tekrar deneyin.")]
        public string Username { get; set; }

        //[DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre gerekli.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalı.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakterden oluşmalı.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre gerekli.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakterden oluşmalı.")]
        [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakterden oluşmalı.")]
        [Compare(nameof(Password))]
        public string RePassword { get; set; }
    }
}
