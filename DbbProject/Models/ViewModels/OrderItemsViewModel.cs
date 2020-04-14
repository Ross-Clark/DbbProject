using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models.ViewModels
{
  public class OrderItemViewModel
  {
    public int GameId { get; set; }

    public int UserId { get; set; }

    [Required]
    public string Name { get; set; }

    public int Quantity { get; set; } // number left

    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }
  }
}
