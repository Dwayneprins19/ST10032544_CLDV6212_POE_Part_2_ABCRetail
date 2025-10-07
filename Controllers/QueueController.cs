using Microsoft.AspNetCore.Mvc;
using ABCRetail.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ABCRetail.Controllers
{
	public class QueueController : Controller
	{
		private readonly QueueService _queueService;

		public QueueController(QueueService queueService)
		{
			_queueService = queueService;
		}

		public IActionResult Index()
		{
			return RedirectToAction(nameof(All));
		}

		public IActionResult Enqueue() => View();

		[HttpPost]
		public async Task<IActionResult> Enqueue(string message)
		{
			if(!string.IsNullOrEmpty(message))
			{
				await _queueService.EnqueueMessageAsync("orders", message);
				TempData["Success"] = "Message added to queue successfully.";
			}
			else
			{
				TempData["Error"] = "Please enter a message.";
			}
			return RedirectToAction(nameof(Enqueue));
		}

		public async Task<IActionResult> Dequeue()
		{
			var msg = await _queueService.DequeueMessageAsync("orders");
			ViewBag.Message = msg ?? "No messages in queue.";
			return View();
		}

		public async Task<IActionResult> Peek()
		{
			var msg = await _queueService.PeekMessageAsync("orders");
			ViewBag.Message = msg ?? "No messages to peek.";
			return View();
		}

		public async Task<IActionResult> All()
		{
			var messages = await _queueService.GetAllMessagesAsync("orders");
			return View(messages);
		}

		public async Task<IActionResult> Clear()
		{
			await _queueService.ClearQueueAsync("orders");
			TempData["Success"] = "Queue cleared successfully.";
			return RedirectToAction(nameof(All));
		}
	}
}
