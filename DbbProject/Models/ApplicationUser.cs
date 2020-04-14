using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DbbProject.Models
{
  // Add profile data for application users by adding properties to the ApplicationUser class
  public class ApplicationUser : IdentityUser
  {

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    // could add regex here for validation against post codes ( dont know pattern for US postal codes ) 
    // US Postal Codes are available for the data annotation but not UK post codes. Unfortunate
    public string PostCode { get; set; } 

    public Byte[] ProfilePicture { get; set; } // add condition somewhere so you don't need to upload one and one is used by default 

    [Required]
    [DataType(DataType.PhoneNumber)]
    public string MobileNumber { get; set; } // could add regex here for validation

  }

}
