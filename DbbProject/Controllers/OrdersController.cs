using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DbbProject.Data;
using DbbProject.Models;
using DbbProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DbbProject.Controllers
{
  public class OrdersController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(ApplicationDbContext context)
    {
      _context = context;
    }

    // POST: Orders/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
      Order order = (Order) TempData["Basket"];
      _context.Add(order);
      await _context.SaveChangesAsync();
      return RedirectToAction("searchGames", "Game");
    }

    //post Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete()
    {
      TempData["Basket"] = null;
      return RedirectToAction("SearchGames", "Game"); // redirect to search page
    }

    //post Remove
    [HttpPost, ActionName("Remove")]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int? id)
    {
      Order order = (Order) TempData["Basket"];
      order.OrderItems.RemoveAll(x => x.GameId == id);
      TempData["Basket"] = order;
      return RedirectToAction("SearchGames", "Game"); // redirect to search page
    }

    //post Add
    [HttpPost, ActionName("Add")]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int id)
    {
      if (TempData["Basket"] == null)
      {
        Order order = (Order)TempData["Basket"];
        order.UserId = _userManager.GetUserAsync(User).Result.Id;
        OrderItem orderItem = new OrderItem()
        {
          Game = _context.Games.Find(id),
          GameId = id,
          Quantity = 1
        };
        order.OrderItems.Add(orderItem);
        TempData["basket"] = order;
      }
      else
      {
        Order order = (Order)TempData["Basket"];

        if(!order.OrderItems.Exists(x=>x.GameId == id)) {
          
          var orderItem = new OrderItem()
          {
            Game = _context.Games.Find(id),
            GameId = id,
            Quantity = 1
          }; 
          order.OrderItems.Add(orderItem);

        } // should add prompt here for user to see
        TempData["basket"] = order;
      }
      return new EmptyResult(); // redirect to search page
    }

  }
}
