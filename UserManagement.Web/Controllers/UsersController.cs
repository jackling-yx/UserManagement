using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserValidator _userValidator;
    public UsersController(IUserService userService, IUserValidator userValidator)
    {
        _userService = userService;
        _userValidator = userValidator;
    }

    [HttpGet("all")]
    public ViewResult List()
    {
        var items = _userService.GetAll().Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet("active")]
    public ViewResult ActiveOnly()
    {
        var items = _userService.FilterByActive(true)
            .Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View("List", model);
    }

    [HttpGet("inactive")]
    public ViewResult InactiveOnly()
    {
        var items = _userService.FilterByActive(false)
            .Select(p => new UserListItemViewModel
            {
                Id = p.Id,
                Forename = p.Forename,
                Surname = p.Surname,
                Email = p.Email,
                IsActive = p.IsActive,
                DateOfBirth = p.DateOfBirth
            });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View("List", model);
    }

    [HttpGet("add")]
    public ViewResult AddUser()
    {
        return View("Add");
    }

    [HttpPost("add")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddUser(string forename, string surname, string email, DateTime dateOfBirth)
    {
        if (IsUserInputValid(forename, surname, email, dateOfBirth) == false)
        {
            ModelState.AddModelError(string.Empty, "Form is invalid, please check and try again");
            return View("Add");
        }

        await _userService.CreateUserAsync(forename, surname, email, dateOfBirth);
        return RedirectToAction("List");
    }

    [HttpGet("view")]
    public async Task<ViewResult> ViewUser(long id)
    {
        var user = await _userService.GetUserAsync(id);

        if (user == null)
        {
            return View("List");
        }

        var model = new UserListItemViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                IsActive = user.IsActive
            };

        return View("View", model);
    }

    [HttpGet("edit")]
    public async Task<ViewResult> EditUser(long id)
    {
        var user = await _userService.GetUserAsync(id);

        var model = new UserListItemViewModel
        {
            Id = id,
            Forename = user.Forename,
            Surname = user.Surname,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            IsActive = user.IsActive
        };

        return View("Edit", model);
    }

    [HttpPost("edit")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditUser(long id, string forename, string surname, string email, bool isActive, DateTime dateOfBirth)
    {
        if (IsUserInputValid(forename, surname, email, dateOfBirth) == false)
        {
            ModelState.AddModelError(string.Empty, "Forename, Surname and Email are required.");
        }

        await _userService.UpdateUser(id, forename, surname, email, dateOfBirth);
        return RedirectToAction("List");
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteUser(long id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if(result == null || !result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return RedirectToAction("List");
        }

        return RedirectToAction("List");
    }

    [HttpPost("throwException")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ThrowException()
    {
        await _userService.ThrowExceptionAsync();
        return RedirectToAction("List");
    }

    private bool IsUserInputValid(string forename, string surname, string email, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(forename) || string.IsNullOrWhiteSpace(surname) || !_userValidator.IsValidEmail(email) || !_userValidator.IsAdult(dateOfBirth))
        {
            return false;
        }
        return true;
    }
}
