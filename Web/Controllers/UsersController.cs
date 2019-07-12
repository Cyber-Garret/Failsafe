﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Web.Models;
using Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		UserManager<NeiraUser> _userManager;

		public UsersController(UserManager<NeiraUser> userManager)
		{
			_userManager = userManager;
		}
		public IActionResult Index() => View(_userManager.Users.ToList());

		public IActionResult Create() => View();

		[HttpPost]
		public async Task<IActionResult> Create(CreateUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				NeiraUser user = new NeiraUser { Email = model.Email, UserName = model.Email };
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}
			return View(model);
		}

		public async Task<IActionResult> Edit(string id)
		{
			NeiraUser user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			EditUserViewModel model = new EditUserViewModel { Id = user.Id, Email = user.Email, DisplayName = user.UserName };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				NeiraUser user = await _userManager.FindByIdAsync(model.Id);
				if (user != null)
				{
					user.Email = model.Email;
					user.UserName = model.DisplayName;

					var result = await _userManager.UpdateAsync(user);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					else
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}
			}
			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> Delete(string id)
		{
			NeiraUser user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				IdentityResult result = await _userManager.DeleteAsync(user);
			}
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> ChangePassword(string id)
		{
			NeiraUser user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				NeiraUser user = await _userManager.FindByIdAsync(model.Id);
				if (user != null)
				{
					IdentityResult result =
						await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					else
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Пользователь не найден");
				}
			}
			return View(model);
		}
	}
}