using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using ABCRetail.Models;

namespace ABCRetail.Controllers
{
	public class ProductController : Controller
	{

		private readonly ProductTableService _productService;
		private readonly BlobStorageService _blobService;

		public ProductController(ProductTableService productService, BlobStorageService blobService)
		{
			_productService = productService;
			_blobService = blobService;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _productService.GetProductsAsync();
			return View(products);
		}

		public IActionResult Create() => View();

		[HttpPost]
		public async Task<IActionResult> Create(ProductEntity product, IFormFile imageFile)
		{
			if (ModelState.IsValid && imageFile != null) 
			{
				using var stream = imageFile.OpenReadStream();
				await _blobService.UploadBlobAsync("products", stream, imageFile.FileName);
				product.ImageUrl = _blobService.GetBlobUrl("products", imageFile.FileName);
				await _productService.AddProductAsync(product);
				return RedirectToAction(nameof(Index));
			}
			return View(product);
		}

		public async Task<IActionResult> Delete(string partitionKey, string rowKey)
		{
			var product = await _productService.GetProductAsync(partitionKey, rowKey); 
			if (product != null)
			{
				await _productService.DeleteProductAsync(partitionKey, rowKey); 
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
