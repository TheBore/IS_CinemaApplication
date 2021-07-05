using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Domain;
using Cinema.Domain.DomainModels;
using Cinema.Domain.Identity;
using Cinema.Service.Interface;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    public class UserController : Controller
    {

        private readonly UserManager<CinemaUser> userManager;

        public UserController(UserManager<CinemaUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
      
        public IActionResult ImportUsers(IFormFile file)
        {

            //make a copy to READ DATA
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from copy file

            List<UserRegistrationDto> users = getAllUsersFromFile(file.FileName);

            ImportAllUsers(users);

            return RedirectToAction("Index", "User");

        }

        private List<UserRegistrationDto> getAllUsersFromFile(string fileName)
        {

            List<UserRegistrationDto> users = new List<UserRegistrationDto>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRegistrationDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),
                            Role = reader.GetValue(3).ToString()
                        }) ;
                    }
                }
            }

            return users;
        }
        public async Task<Boolean> ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {
                    var user = new CinemaUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart(),

                    };
                    var result = userManager.CreateAsync(user, item.Password).Result;
                    await userManager.AddToRoleAsync(user, item.Role);
              
                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }
    }
}