using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DbbProject.Data;
using DbbProject.Models;
using DbbProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DbbProject.Controllers
{
  public class GameController : Controller
  {
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    public GameController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    public async Task<IActionResult> OwnGames(string sortOrder)
  {
    //returns all games under a user id
    var games = _context.Games.Where(x => x.OwnerId == _userManager.GetUserId(User));

    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
    ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";

      switch (sortOrder) // sorts games list. default by name
    {
      case "name_desc":
        games = games.OrderByDescending(g => g.Name);
        break;
      case "description":
        games = games.OrderBy(g => g.Description);
        break;
      case "description_desc":
        games = games.OrderByDescending(g => g.Description);
        break;
      default:
        games = games.OrderBy(s => s.Name);
        break;
    }

      return View(await games.ToListAsync());
  }

    public async Task<IActionResult> SearchGames(string searchString, string sortOrder)
  {

      // displays all games unless a search term is entered and submitted
      // overload which allows for sorting

      ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
      ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";

      if (!string.IsNullOrEmpty(searchString))
      {
        var games = _context.Games.Where(g => g.Name.Contains(searchString)
                                            || g.Description.Contains(searchString));


        switch (sortOrder) // sorts games list. default by name
        {
          case "name_desc":
            games = games.OrderByDescending(g => g.Name);
            break;
          case "description":
            games = games.OrderBy(g => g.Description);
            break;
          case "description_desc":
            games = games.OrderByDescending(g => g.Description);
            break;
          default:
            games = games.OrderBy(s => s.Name);
            break;
        }
          return View(await games.ToListAsync());
      }
      else
      {
        var games = _context.Games.Where(x=>x==x); // this is a hacky way of getting this to be an iqueryable, so i cant sort it needs refactoring

        switch (sortOrder) // sorts games list. default by name
        {
          case "name_desc":
            games = games.OrderByDescending(g => g.Name);
            break;
          case "description":
            games = games.OrderBy(g => g.Description);
            break;
          case "description_dec":
            games = games.OrderByDescending(g => g.Description);
            break;
          default:
            games = games.OrderBy(s => s.Name);
            break;
        }

        return View(await games.ToListAsync());
      }
  }

  // GET: Game/Create
  public IActionResult Create()
  {
    return View();
  }

  // POST: Game/Create
    [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(GameViewModel game)
  {
    ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
    if (ModelState.IsValid)
    {
      //load image file via memory stream, validating against its existence
      var imageMemoryStream = new MemoryStream();
      if (game.Image != null && game.Image.Length > 0)
      {
        game.Image.CopyTo(imageMemoryStream);
      }

      // creates a new game using data from the view model
      // saves image as byte[]

      var newGame = new Game()
      {
        OwnerId = currentUser.Id,
        Name = game.Name,
        GameImage = imageMemoryStream.Length > 0 ? imageMemoryStream.ToArray() : null,
        Description = game.Description,
        Price = game.Price
      };
      _context.Add(newGame);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(OwnGames));
    }

    return View(game);
  }

  // GET: Game/Edit/5
  public async Task<IActionResult> Edit(int? id)
  {
    
    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
    
    if (id == null)
    {
      return NotFound(); // no game throws error
    }

    var game = await _context.Games.SingleOrDefaultAsync(a => a.GameId == id);

    if (game == null || game.OwnerId != currentUser.Id)
    {
      return NotFound(); // if the game doesn't exist or the user does not own the game, don't let the user see it
    }


      var gameViewModel = new GameViewModel()
    {
      GameId = game.GameId,
      Name = game.Name,
      Description = game.Description,
      Price = game.Price
    };


    return View(gameViewModel);

  }

    // POST: Game/Edit/5
    [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, GameViewModel game)
  {
    ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);// find current user

   
    if (id != game.GameId) // if the game does not exist, return error
    {
      return NotFound();
    }

    Game currentGame = await _context.Games.SingleOrDefaultAsync(a => a.GameId == id); // find game with corresponding id

    if (currentGame.OwnerId != currentUser.Id) // check if the user is the correct user should add admin somewhere else
    {
      return Forbid(); // throws forbidden error, users shouldn't be able to edit other users stuff
    }

    if (ModelState.IsValid) // validate model
    {
      // handle images first
      byte[] imageByteArray = new byte[0];
      if (game.Image != null && game.Image.Length > 0) //check if a new image is supplied and use if so use it
      {

          var imageMemoryStream = new MemoryStream();
          game.Image.CopyTo(imageMemoryStream);
          imageByteArray = imageMemoryStream.ToArray();
      }
      else // use old image or no image
      {
        if (currentGame.GameImage != null && currentGame.GameImage.Length > 0) // handle if there was never an image
        {
          imageByteArray = currentGame.GameImage.ToArray(); // use old image
        }
      }

        // assign the updated values
        currentGame.Name = game.Name;
        currentGame.Description = game.Description;
        currentGame.Price = game.Price;
        currentGame.GameImage = imageByteArray.Length > 0 ? imageByteArray : null; // ternary operator used to correctly set null

      try
      {
        _context.Update(currentGame);
        await _context.SaveChangesAsync(); //async save to database
      }
      catch (DbUpdateConcurrencyException) // if this has already been updated or deleted
        {
          if (!_context.Games.Any(a => a.GameId == id))
          {
            return NotFound(); // if its deleted / incorrect id
          }
          else
          {
            throw;
          }
        } 
        return RedirectToAction(nameof(OwnGames));
    }
    return View(game);
  } 

  // GET: Game/Delete/5
  public async Task<ActionResult> Delete(int? id)
  {
    var user = await _userManager.GetUserAsync(HttpContext.User);

    if (id == null)
    {
      return NotFound(); // checks if id is valid
    }

    var currentGame = await _context.Games.SingleOrDefaultAsync(g => g.GameId == id);

    if (currentGame == null || currentGame.OwnerId != user.Id)
    {
      return Forbid(); // the user should only be able to delete their own games
    }

    var model = new GameViewModel()
    {
      GameId = currentGame.GameId,
      Name = currentGame.Name,
      Description = currentGame.Description,
      Price = currentGame.Price
    };

    return View(model);
    }

  // POST: Game/Delete/5
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(int id)
  {
    var user = await _userManager.GetUserAsync(HttpContext.User);

    var currentGame = await _context.Games.SingleOrDefaultAsync(m => m.GameId == id);

    if (currentGame.OwnerId == user.Id) // rechecks the games owner to prevent post injection attack
    {
      _context.Games.Remove(currentGame); //removes the current game from db
    }

    await _context.SaveChangesAsync(); // async removal of current game from db

    return RedirectToAction(nameof(OwnGames)); //redirect back to owngames
    }

    // GET: ComputerAccessories/Details/5
    public async Task<IActionResult> Details(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    var game = await _context.Games
      .SingleOrDefaultAsync(g => g.GameId == id);

    if (game == null)
    {
      return NotFound();
    }

    var gameViewModel = new GameViewModel()
    {
      GameId = game.GameId,
      Name = game.Name,
      Description = game.Description,
      Price = game.Price
    };

    ViewBag.ImageData = game.GameImage;

    return View(gameViewModel);
  }
  }
}