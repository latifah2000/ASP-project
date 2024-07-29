using Azure;
using Dashboard.Data;
using Dashboard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
           _context = context;
		   _webHostEnvironment = webHostEnvironment;

		}


        [Authorize]
        public IActionResult Index()
        {
            var username=HttpContext.User.Identity.Name??null;  // get user data


            //CookieOptions cookie = new CookieOptions(); // create  cookie
            //cookie.Expires = DateTime.Now.AddMinutes(50); // set time long
            //Response.Cookies.Append("userdata", username, cookie); // store user data in my cookie


            HttpContext.Session.SetString("userdata", username);

            ViewBag.Username = username;

           // ViewBag.Username = Request.Cookies["userdata"]; // get user data from my cookie

            return View();
        }

        public IActionResult AddNewitems()
        {

            ViewBag.Username = HttpContext.Session.GetString("userdata");

            var products = _context.products.ToList();
            ViewBag.Products = products;
            return View(products);
        }

		public IActionResult ProductDetails()
		{
			ViewBag.Username = HttpContext.Session.GetString("userdata");



			var productsDetails = _context.productsDetails.Join(

				_context.products,

				prodetail => prodetail.ProductId,
				products => products.Id,

				(prodetail, products) => new
				{
					id = prodetail.Id,
					name = products.Name,
					color = prodetail.Color,
					price = prodetail.Price,
					qty = prodetail.Qty,
					img = prodetail.Images

				}
				).ToList();

			ViewBag.ProductsDetails = productsDetails;

			ViewBag.products = _context.products.ToList();

			return View();

		}


		public IActionResult CreateDetails(ProductsDetails productDetails, IFormFile photo)
		{

			if (photo == null || photo.Length == 0)
			{
				return Content("File Not Selected");
			}

			var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", photo.FileName);

			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				photo.CopyTo(stream);
				stream.Close();
			}

			productDetails.Images = photo.FileName;
			_context.productsDetails.Add(productDetails);
			_context.SaveChanges();

			return RedirectToAction("ProductDetails");
		}

		// GET: //Delete Operation for ProductDetails
		public JsonResult DeleteDetails(int id)
		{
			var detailsDel = _context.productsDetails.SingleOrDefault(p => p.Id == id);

			if (detailsDel != null)
			{
				_context.productsDetails.Remove(detailsDel);
				_context.SaveChanges();
				TempData["details"] = true;
			}
			else
			{
				TempData["details"] = false;
			}

			return Json("ProductDetails");


		}

		[HttpPost]
		[HttpPost]
		public IActionResult UpdateDetails(ProductsDetails productDetails, IFormFile photo)
		{
			if (productDetails == null)
			{
				return BadRequest("Invalid product details");
			}

			var existingProductDetails = _context.productsDetails.FirstOrDefault(p => p.ProductId == productDetails.ProductId);

			if (existingProductDetails == null)
			{
				return NotFound("Product details not found");
			}

			// Update product details fields
			existingProductDetails.Price = productDetails.Price;
			existingProductDetails.Qty = productDetails.Qty;
			existingProductDetails.Color = productDetails.Color;

			if (photo != null && photo.Length > 0)
			{
				// Handle photo update
				var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", photo.FileName);

				try
				{
					using (var stream = new FileStream(path, FileMode.Create))
					{
						photo.CopyTo(stream);
					}
					existingProductDetails.Images = photo.FileName;
				}
				catch (Exception ex)
				{
					// Log the exception
					_logger.LogError(ex, "Error saving file");
					TempData["ErrorMessage"] = "Error saving file.";
					return RedirectToAction("ProductDetails");
				}
			}

			try
			{
				_context.productsDetails.Update(existingProductDetails);
				_context.SaveChanges();
				TempData["SuccessMessage"] = "Product details updated successfully";
			}
			catch (Exception ex)
			{
				// Log the exception
				_logger.LogError(ex, "Error updating product details");
				TempData["ErrorMessage"] = "Error updating product details.";
			}

			return RedirectToAction("ProductDetails");
		}


		// GET://Edit for Product details table
		public IActionResult EditDetails(int id)
		{
			var productDetails = _context.productsDetails.SingleOrDefault(p => p.Id == id);

			if (productDetails == null)
			{
				return NotFound();
			}

			ViewBag.Products = _context.products.ToList(); // Load products for dropdown
			return View(productDetails);
		}






		public IActionResult CreateProducts(Products products)
        {

            if (ModelState.IsValid)
            {
                _context.products.Add(products);
                _context.SaveChanges();
                TempData["Add"] = "ÊãÊ ÇáÅÖÇÝÉ ÈäÌÇÍ";
                return RedirectToAction("AddNewitems");
            }



            TempData["Add"] = "áã ÊÊã ÇáÅÖÇÝÉ íÑÌì ÇáÊÃßÏ ãä ÕÍÉ ÇáãÏÎáÇÊ";


            var product = _context.products.ToList();
            return View("AddNewitems", product);

            //_context.Add(products);
            // _context.SaveChanges();
            //return RedirectToAction("AddNewitems");
        }

        public IActionResult Update(Products Product)
        {
            if (ModelState.IsValid)
            {
                _context.products.Update(Product);
                _context.SaveChanges();
            }

            return RedirectToAction("AddNewitems");
        }

        public IActionResult Edit(int record)
        {
            var Product = _context.products.SingleOrDefault(x => x.Id == record);

            return View(Product);
        }

        [HttpPost]
        public JsonResult Delete(int record)
        {
            var productdel = _context.products.SingleOrDefault(p => p.Id == record);
           
            if (productdel != null)
            {
                _context.products.Remove(productdel);
                _context.SaveChanges();
                TempData["del"] = true;
            }
            else
            {
                TempData["del"] = false;
            }

            return Json("AddNewitems");
        }

        public JsonResult GetData(int id)
        {
            var product = _context.products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                return Json(product);
            }
            else
            {
                return Json(null);
            }


        }

		public JsonResult GetDetails(int id)
		{
			var productDetails = _context.productsDetails.FirstOrDefault(p => p.Id == id);

			if (productDetails != null)
			{
				return Json(productDetails);
			}
			else
			{
				return Json(null);
			}


		}
		public IActionResult AddDemag(Damegedproducts damege)
        {

            _context.Add(damege);
            _context.SaveChanges();

            return RedirectToAction("Demag");
        }
        public IActionResult Demag()
        {
            ViewBag.Username = HttpContext.Session.GetString("userdata");
            var products= _context.products.ToList();

            var Productsdemage = _context.damegedproducts.Join
                (
                     _context.products,

                      demag => demag.ProductId,
                      products =>products.Id,


                     (demag, products) => new
                     {
                         demag,
                         products
                     }
                    
                ).Join
                (
                  _context.productsDetails,

                  p=>p.demag.ProductId,
                  c=>c.ProductId,

                  (p, c) => new
                  {
                      id=p.demag.Id,
                      name=p.products.Name,
                      color=c.Color,
                      qty=p.demag.Qty
                  }
                  ).ToList();

            Console.WriteLine($"{Productsdemage}");
                
               
            ViewBag.products = products;
            ViewBag.damage = Productsdemage;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
