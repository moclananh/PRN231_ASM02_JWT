using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System;
using Client.Models;
using System.Text.Json;

namespace Client.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";

        public UsersController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:5001/api/Users";
        }
        public IActionResult Login()
        {
            return View();
        }
        // POST: Authentication/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            try
            {
                // Create a JSON payload with the email and password

                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ProductApiUrl + "/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    // save JWT token to session
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string strData = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<LoginResponse>(strData, options);
                    HttpContext.Session.SetString("Token", data.Data.ToString());

                    HttpContext.Session.SetString("email", request.Email);
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    // Handle authentication failure, e.g., display an error message
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                ModelState.AddModelError(string.Empty, "An error occurred during login.");
            }

            // If login fails, return the login view with validation errors
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm member)
        {
            if (string.IsNullOrWhiteSpace(member.Email) || string.IsNullOrWhiteSpace(member.Password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            try
            {
                // Create a JSON payload with the email and password

                var content = new StringContent(JsonSerializer.Serialize(member), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(ProductApiUrl + "/Register", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    // Handle authentication failure, e.g., display an error message
                    ModelState.AddModelError(string.Empty, "Register failed.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                ModelState.AddModelError(string.Empty, "An error occurred during register.");
            }

            // If login fails, return the login view with validation errors
            return View();
        }
    }
}
