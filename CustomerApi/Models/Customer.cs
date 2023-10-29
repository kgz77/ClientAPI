using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;


public class Customer
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Имя пользователя является обязательным!")]
    [StringLength(50, ErrorMessage = "Имя пользователя должно содержать не более 50 символов!")]
    [Display(Description = "Имя пользователя")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Номер телефона является обязательным!")]
    [Display(Description = "+996700123456")]
    public string PhoneNumber { get; set; }
}
