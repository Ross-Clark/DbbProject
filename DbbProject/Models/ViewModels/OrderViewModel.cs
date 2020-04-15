using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models.ViewModels
{
  public class OrderViewModel
  {
    public string UserId { get; set; }
    public List<OrderItem> OrderItem { get; set; }
    public DateTime OrderDateTime { get; set; }
    [DataType(DataType.Currency)]
    public decimal TotalPrice { get; set; }
  }
}