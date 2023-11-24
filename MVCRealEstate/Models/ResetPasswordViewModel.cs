using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MVCRealEstate.Models;

public class ResetPasswordViewModel
{
    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen geçerli bir e-posta yazınız")] 
    public string UserName { get; set; }

}
