using Csharp3_A3.Models;
using Csharp3_A3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Csharp3_A3.Controllers
{
	public class NewsController : Controller
	{
		private readonly NewsService _newsService;

		public NewsController(NewsService newsService) 
		{
			_newsService = newsService;
		}

		[HttpGet]
		public async Task<IActionResult> NewsManagement()
		{
			var newsItems = await _newsService.GetAllAsync();
			return View(newsItems);
		}

		[HttpGet]
		public async Task<IActionResult> EditNews(int id)
		{
			var newsItem = await _newsService.GetByIdAsync(id);
			if (newsItem == null)
			{
				return NotFound();
			}

			return View(newsItem);
		}

		[HttpGet]
		public async Task<IActionResult> Default()
		{
			var newsItems = await _newsService.GetAllAsync();
			return View(newsItems);
		}

		[HttpGet]
		public IActionResult AddNews()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Details(int Id)
		{
			var newsItem = await _newsService.GetByIdAsync(Id);
			if (newsItem == null)
			{
				return NotFound();
			}
			
			return View(newsItem);
		}

		[HttpPost]
		public async Task<IActionResult> Search(string query)
		{
			var allNews = await _newsService.GetAllAsync();
			if (allNews == null || string.IsNullOrWhiteSpace(query))
			{
				return View(new List<NewsItem>());
			}

			var results = allNews.Where(n => n.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || n.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

			return View(results);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			await _newsService.DeleteAsync(id);
			return RedirectToAction("NewsManagement", "News");
		}

		[HttpPost]
		public async Task<IActionResult> EditNews(NewsItem newsItem)
		{
			if (!ModelState.IsValid)
				return View(newsItem);

			var itemToUpdate = await _newsService.GetByIdAsync(newsItem.Id);
			if (itemToUpdate == null)
				return NotFound();

			itemToUpdate.Title = newsItem.Title;
			itemToUpdate.Content = newsItem.Content;
			itemToUpdate.ImagePath = newsItem.ImagePath;
			itemToUpdate.Url = newsItem.Url;

			await _newsService.UpdateAsync(itemToUpdate);

			return RedirectToAction("NewsManagement", "News");
		}

		[HttpPost]
		public async Task<IActionResult> AddNews(NewsItem newsItem)
		{
			await _newsService.AddAsync(newsItem);
			return RedirectToAction("NewsManagement", "News");
		}
	}
}
