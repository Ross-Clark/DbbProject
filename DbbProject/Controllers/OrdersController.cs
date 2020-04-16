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

        // FUNKY GET: Orders/Basket
        public async Task<IActionResult> Basket(String function,int gameId)
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
              UserId = user.Id,
              OrderTotal = 0
            };
            _context.Add(currentOrder);
            _context.SaveChanges();
          }
          else
          {
            currentOrder = await _context.Orders.SingleOrDefaultAsync(o => o.UserId == user.Id && o.Open);
          }

          Game currentGame = await _context.Games.SingleOrDefaultAsync(g => g.GameId == gameId);

          switch (function)
          {
            case "add":

              currentOrder.OrderItems = _context.OrderItems.Where(x => x.OrderId == currentOrder.OrderId).ToList();
              foreach (var orderItem in currentOrder.OrderItems)
              {
                orderItem.Game = _context.Games.First(x => orderItem.GameId == x.GameId);
              }
              if (currentGame.OwnerId != user.Id && (currentOrder.OrderItems == null || !currentOrder.OrderItems.Any(x=>x.GameId == gameId))) // stops the user buying their own game or the same game twice
              { 
                OrderItem newOrderItem = new OrderItem()
                {
                  Game = currentGame,
                  GameId = gameId,
                  Quantity = 1,
                  OrderId = currentOrder.OrderId
                };
                _context.OrderItems.Add(newOrderItem);

                
                if (currentOrder.OrderItems != null && currentOrder.OrderItems.Count > 0)
                {
                  currentOrder.OrderTotal = 0;
                  foreach (var orderItem in currentOrder.OrderItems)
                  {
                    currentOrder.OrderTotal += (orderItem.Quantity * orderItem.Game.Price);
                  }
                }

                _context.Update(currentOrder);
                _context.Entry(currentGame).State = EntityState.Modified;

                _context.SaveChanges();

                return View(currentOrder);
              }
              return RedirectToAction("SearchGames","Game");// this should never be returned, unless the user breaks something or forges a Post
            case "remove":
              currentOrder.OrderItems = _context.OrderItems.Where(x => x.OrderId == currentOrder.OrderId).ToList();
              foreach (var orderItem in currentOrder.OrderItems)
              {
                orderItem.Game = _context.Games.First(x => orderItem.GameId == x.GameId);
              }
              if (currentOrder.OrderItems.Exists(g=>g.GameId==gameId))
              {
                OrderItem oldOrderItem = currentOrder.OrderItems.First(o=>o.GameId==gameId && o.OrderId==currentOrder.OrderId);

                if (currentOrder.OrderItems != null && currentOrder.OrderItems.Count > 0)
                {
                  currentOrder.OrderTotal = 0;
                  foreach (var orderItem in currentOrder.OrderItems)
                  {
                    currentOrder.OrderTotal += (orderItem.Quantity * orderItem.Game.Price);
                  }
                }

                _context.Remove(oldOrderItem);
                _context.Update(currentOrder);
                _context.Entry(currentGame).State = EntityState.Modified;
                _context.SaveChanges();

                return View(currentOrder);
              }
              return View(currentOrder);// this should never be returned, unless something goes wrong (user tries to remove something twice) prevents this

            default:
              currentOrder.OrderItems = _context.OrderItems.Where(x => x.OrderId == currentOrder.OrderId).ToList();
              foreach (var orderItem in currentOrder.OrderItems)
              {
                orderItem.Game = _context.Games.First(x => orderItem.GameId == x.GameId);
              }
              return View(currentOrder);
          }
        }

        //confirmation
        public async Task<IActionResult> Confirmation(int orderId)
        {
          var user = await _userManager.GetUserAsync(User);

          Order currentOrder;
          if (_context.Orders.Any(o => o.UserId == user.Id && orderId == o.OrderId && o.Open)) // prevents check out of other users basket or an already closed basket
          {
            currentOrder = _context.Orders.First(o => o.UserId == user.Id && orderId == o.OrderId);
            currentOrder.OrderItems = _context.OrderItems.Where(x => x.OrderId == currentOrder.OrderId).ToList();
            foreach (var orderItem in currentOrder.OrderItems)
            {
              Game currentGame = _context.Games.First(x => orderItem.GameId == x.GameId);
              orderItem.Game = currentGame;
              currentGame.Sold = true;
              _context.Update(currentGame);
              _context.SaveChanges();
            }
          }
          else
          {
            return RedirectToAction(nameof(Basket)); // prevents viewing this page without an open order 
          }

          currentOrder.OrderDateTime = DateTime.Now;
          currentOrder.Open = false;
          _context.Update(currentOrder);
          _context.SaveChanges();

          return View(currentOrder);
        }

        private bool OpenOrderExists()
        {
          string userId = _userManager.GetUserAsync(User).Result.Id;
          return _context.Orders.Any(order => order.UserId == userId && order.Open);
        }
    }

    //
    // TODO -- redo all of thi
    // only need a basket and confirmation page
    // confirm will be a basket + total cost and order date 
}
