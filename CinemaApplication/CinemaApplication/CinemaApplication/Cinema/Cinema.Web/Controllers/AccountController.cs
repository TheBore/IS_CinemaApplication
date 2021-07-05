using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Cinema.Domain;
using Cinema.Domain.Identity;
using Cinema.Domain.DomainModels;

namespace Cinema.Web.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<CinemaUser> userManager;
        private readonly SignInManager<CinemaUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<CinemaUser> userManager, SignInManager<CinemaUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (ModelState.IsValid)
            {
                var userCheck = await userManager.FindByEmailAsync(request.Email);
                if (userCheck == null)
                {
                    var user = new CinemaUser
                    {
                        UserName = request.Email,
                        NormalizedUserName = request.Email,
                        Email = request.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    var result = await userManager.CreateAsync(user, request.Password);

                    if (result.Succeeded)
                    {
                        /* await userManager.AddToRoleAsync(user, "user");*/
                        await userManager.AddToRoleAsync(user, "StandardUser");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(request);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(request);
                }
            }
            return View(request);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(model);

                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);

                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult AllUsers()
        {
            var users = userManager.Users.ToList();
            foreach(var user in users)
            {
                var isAdmin = userManager.IsInRoleAsync(user, "Administrator").Result;
                if (isAdmin)
                {
                    user.Role = "Administrator";
                }
                else
                {
                    user.Role = "StandardUser";
                }
            }
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var userCheck = await userManager.FindByIdAsync(id);

            if (userCheck == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            var isAdmin = userManager.IsInRoleAsync(userCheck, "Administrator").Result;
            if (isAdmin)
            {
                await userManager.RemoveFromRoleAsync(userCheck, "Administrator");
                await userManager.AddToRoleAsync(userCheck, "StandardUser");
            }
            else
            {
                await userManager.RemoveFromRoleAsync(userCheck, "StandardUser");
                await userManager.AddToRoleAsync(userCheck, "Administrator");
            }
            return RedirectToAction("AllUsers", "Account");
        }

        
       

    }
}
