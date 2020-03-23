using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models
{
  public class OrderItem
  {
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; }

    public int GameId { get; set; }

    public Game Game { get; set; }

    public int Quantity { get; set; }



  }
}