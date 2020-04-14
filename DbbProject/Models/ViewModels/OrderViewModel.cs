using System;
using System.Collections.Generic;

namespace DbbProject.Models.ViewModels
{
  public class OrderViewModel
  {
    public int OrderId { get; set; }
    public string UserId { get; set; }
    public List<OrderItemViewModel> OrderItemViewModels { get; set; }
    public DateTime OrderDateTime { get; set; }
  }
}