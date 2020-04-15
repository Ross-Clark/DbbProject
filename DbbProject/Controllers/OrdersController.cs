using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DbbProject.Data;
using DbbProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace DbbProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
          _context = context;
          _userManager = userManager;
        }


        // GET: Orders/Basket
        public IActionResult Basket()
        { 
          var user = _userManager.GetUserAsync(User).Result;
          if (!OpenOrderExists())
          { 
            Order order = new Order()
            {
              Open = true,
              OrderDateTime = DateTime.Now,
              OrderItems = new List<OrderItem>(), 
              UserId = user.Id
            };
            _context.Add(order);
            _context.SaveChanges();
            return View(order);
          }
          else
          {
            Order order = _context.Orders.First(o => o.Open && o.UserId == user.Id); // there should only every be one open order per customer
            return View(order);
          }
        }

        // POST: Orders/Basket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Basket(String action,int gameId)
        {
          var  user = await _userManager.GetUserAsync(User);

          Order currentOrder;

          if (!OpenOrderExists())
          {
            currentOrder = new Order()
            {
              Open = true,
              OrderDateTime = DateTime.Now,
              OrderItems = new List<OrderItem>(),
              UserId = user.Id
            };
            _context.Add(currentOrder);
            _context.SaveChanges();
          }
          else
          {
            currentOrder = await _context.Orders.SingleOrDefaultAsync(o => o.UserId == user.Id && o.Open);
          }

          Game currentGame = await _context.Games.SingleOrDefaultAsync(g => g.GameId == gameId);

          switch (action)
          {
            case "add":
              if(currentGame.OwnerId != user.Id){ // stops the user buying their own game
                OrderItem newOrderItem = new OrderItem()
                {
                  Game = currentGame,
                  GameId = gameId,
                  Order = currentOrder,
                  Quantity = 1,
                  OrderId = currentOrder.OrderId

                };
                _context.Add(newOrderItem);
                currentOrder.OrderItems.Add(newOrderItem);
                _context.Update(currentOrder);
                return View(currentOrder);
              }
              return RedirectToAction("SearchGames","Game");// this should never be returned, unless the user breaks something or forges a Post
            case "remove":
              if (currentOrder.OrderItems.Exists(g=>g.GameId==gameId))
              {
                OrderItem oldOrderItem = currentOrder.OrderItems.First(o=>o.GameId==gameId);
                currentOrder.OrderItems.Remove(oldOrderItem);
                _context.Orders.Update(currentOrder);
                _context.OrderItems.Remove(oldOrderItem);
                return View(currentOrder);
              }
              return View(currentOrder);// this should never be returned, unless something goes wrong (user tries to remove something twice) prevents this

            default:
              return NotFound();
          }
        }

        private bool OpenOrderExists()
        {
          string userId = _userManager.GetUserAsync(User).Result.Id;
          return _context.Orders.Any(order => order.UserId == userId && order.Open);
        }
    }

    //
    // TODO -- redo all of this
    // just store it all in the order db
    // validate, so users cant have more than 1 active order
    // add a property in orders to tell if they have been finished
    // delete most of this controller and make a normal ICRUD template to rework
    // only need a basket and conirmation page
    // basket will just be a details page for 1 order, pick the only closed order a user should have
    //
    // make create the add function just validate to make this work
}
