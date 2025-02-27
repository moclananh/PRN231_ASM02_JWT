﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Client.Controllers
{
    [JwtAuthentication] // return login if not have token
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";

        public ProductsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "https://localhost:5001/api/products";
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);

            }
            var request = new HttpRequestMessage(HttpMethod.Get, ProductApiUrl);


            request.Headers.Add("Authorization", "Bearer " + token);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string strData = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ProductVm>>(strData, options);
                return View(data);
            }
            return RedirectToAction("Error", "Home", null);
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    return RedirectToAction("Login", "Users", null);

                }
                var request = new HttpRequestMessage(HttpMethod.Get, ProductApiUrl+$"/{id}");

                request.Headers.Add("Authorization", "Bearer " + token);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string productData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ProductVm product = JsonSerializer.Deserialize<ProductVm>(productData, options);

                    return View(product);

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or errors, e.g., log the error
            }

            return NotFound();
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVm product)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);
            }

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, ProductApiUrl);
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful creation (e.g., redirect to the product list)
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle errors
                    ModelState.AddModelError(string.Empty, "Error creating the product.");
                }
            }
            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{ProductApiUrl}/{id}");
                request.Headers.Add("Authorization", "Bearer " + token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string productData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ProductVm product = JsonSerializer.Deserialize<ProductVm>(productData, options);

                    return View(product);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or errors, e.g., log the error
            }

            return NotFound();
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVm product)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);
            }

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Put, $"{ProductApiUrl}/{id}");
                request.Headers.Add("Authorization", "Bearer " + token);
                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    // Handle successful update (e.g., redirect to the product list)
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle errors
                    ModelState.AddModelError(string.Empty, "Error updating the product.");
                }
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{ProductApiUrl}/{id}");
                request.Headers.Add("Authorization", "Bearer " + token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string productData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    ProductVm product = JsonSerializer.Deserialize<ProductVm>(productData, options);
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or errors, e.g., log the error
            }

            return NotFound();
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Users", null);
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ProductApiUrl}/{id}");
            request.Headers.Add("Authorization", "Bearer " + token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Handle successful deletion (e.g., redirect to the product list)
                return RedirectToAction("Index");
            }
            else
            {
                // Handle errors
                return NotFound();
            }
        }
    }
}
