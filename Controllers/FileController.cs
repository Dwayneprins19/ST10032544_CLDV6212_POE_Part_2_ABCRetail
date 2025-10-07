using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;

namespace ABCRetail.Controllers
{
	public class FileController : Controller
	{

		private readonly FileShareService _fileShareService;

		public FileController(FileShareService fileShareService)
		{
			_fileShareService = fileShareService;
		}

		public IActionResult Upload() => View();

		[HttpPost]
		public async Task<IActionResult> Upload(IFormFile file)
		{
			if (file != null)
			{
				using var stream = file.OpenReadStream();
				await _fileShareService.UploadFileAsync("sharedfiles", file.FileName, stream);
				ViewBag.Message = $"File '{file.FileName}' uploaded successfully!";
			}
			else
			{
				ViewBag.Message = "Please select a file to upload.";
			}
			return View();
		}

		public async Task<IActionResult> Download(string fileName)
		{
			var fileStream = await _fileShareService.DownloadFileAsync("sharedfiles", fileName);
			if (fileStream == null) 
				return NotFound("File not found.");

			return File(fileStream, "application/octet-stream", fileName);
		}

		public async Task<IActionResult> Delete(string fileName)
		{
			await _fileShareService.DeleteFileAsync("sharedfiles", fileName);
			TempData["Message"] = $"File '{fileName}' deleted successfully.";
			return RedirectToAction("Upload");
		}
	}
}
