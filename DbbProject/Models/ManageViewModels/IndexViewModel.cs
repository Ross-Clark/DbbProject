using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DbbProject.Models.ManageViewModels
{
  public class IndexViewModel
  {
    public string Username { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    public string StatusMessage { get; set; }

    public IFormFile ProfilePicture { get; set; }

  }
}
