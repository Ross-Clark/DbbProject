using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbbProject.Models
{
  public class Order
  {
    public int OrderId { get; set; }

    public string UserId { get; set; }

    public List<OrderItem> OrderItems { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Order Time")]
    [DisplayFormat(DataFormatString = "{ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’}")]
    public DateTime OrderDate { get; set; }

    [DataType(DataType.Currency)]
    [Display(Name = "Total Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal OrderTotal { get; set; }

  }
}