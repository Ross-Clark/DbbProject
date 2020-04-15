using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models.ViewModels
{
  public class OrderItemViewModel
  {
    public int GameId { get; set; }

    public int UserId { get; set; }

    public Game Game { get; set; }

    public int Quantity { get; set; } // number left

    [Required]
    public decimal Price { get; set; }
  }
}
