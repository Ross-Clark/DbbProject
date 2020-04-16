using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbbProject.Models
{
  public class OrderItem
  {
    [Key]
    public int OrderItemId { get; set; } //Primary key for orderItems
    [ForeignKey("Order")]
    public int OrderId { get; set; }

    public Order Order { get; set; }

    [ForeignKey("Game")]
    public int GameId { get; set; }

    public Game Game { get; set; }

    public int Quantity { get; set; }



  }
}