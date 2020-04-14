using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DbbProject.Models.AccountViewModels
{
  public class RegisterViewModel
  {
    [Required]
    [Display(Name = "User Name")]
    [MaxLength(20)]
    [MinLength(3)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Surname")]
    public string Surname { get; set; }

    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string MobileNumber { get; set; }

    [Required]
    // again no post code data type, just postal
    // could add regex validation for uk postcodes
    [Display(Name = "PostCode")]
    public string PostCode { get; set; }

    public IFormFile ProfilePicture { get; set; }
  }
}
