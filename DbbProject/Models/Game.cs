using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace DbbProject.Models
{
  public class Game
  {
    public int GameId { get; set; } // the primary key for games. int

    public string OwnerId { get; set; } // the foreign key used to find the owner in the user table. string

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Game Name")]
    public string Name { get; set; } // the name of the game. string

    [Required]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Game Description")]
    public string Description { get; set; } // the description of the game. string

    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Price { get; set; } // the price of the game displayed as a currency. decimal

    [Display(Name = "Game Image")]
    public byte[] GameImage { get; set; } // the game image. byte[]

    [Display(Name = "Sale Status")]
    [DefaultValue(false)]
    public bool Sold { get; set; } // the status of a game sold or unsold

    public List<OrderItem> OrderItems { get; set; } // navigational property for what OrderItems is is present in. List of OrderItem

  }
}