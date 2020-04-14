using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DbbProject.Models.ViewModels
{
  public class GameViewModel
  {

    public int GameId { get; set; } //Game Id is used as the primary key

    [Required]
    [MinLength(2), MaxLength(100)]
    [Display(Name = "Title")]
    public string Name { get; set; } // the name of the game

    [Display(Name = "Game Image")]
    public IFormFile Image { get; set; } // an image stored as a IFormFile, later stored as a byte[] in the database

    [Required]
    [MinLength(3), MaxLength(200)]
    [Display(Name = "Description")]
    public string Description { get; set; } // the description of the game

    [Required]
    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; } // the price of the game, stored as a decimal and shown as a currency

  }
}
