using System.ComponentModel.DataAnnotations;

namespace IdentityExample.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Kullanıcı ismi gerekldir.")]
        [Display(Name = "Kullanıcı Adı")] //Textbox önünde gözükecek olan yazı
        public string UserName { get; set; }

        [Display(Name = "Tel No:")]
        [RegularExpression(@"^(0(\d{3}) (\d{3}) (\d{2}) (\d{2}))$", ErrorMessage = "Telefon numarası uygun formatta değil")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [Display(Name = "Email Adresiniz")]
        [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değil")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifreniz gereklidir.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)] //Şifre girilirken bu alanın bir şifre alanı olduğunu belirtiyorum. (17. satırda bu şekilde belirtilebilirdi. Aynı işlevi gören farklı kullanımlar)
        public string Password { get; set; }
    }
}
