using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models
{
  public class Game
  {
    public int GameId { get; set; }

    public string OwnerId { get; set; }

    [Required]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Game Name")]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Game Description")]
    public string Description { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Price { get; set; }

    public byte[] GameImage { get; set; }

    public List<OrderItem> OrderItems { get; set; }

  }
}