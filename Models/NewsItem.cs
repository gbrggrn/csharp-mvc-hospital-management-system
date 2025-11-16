using System.ComponentModel.DataAnnotations;

namespace Csharp3_A3.Models
{
	public class NewsItem
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "A title is required for the news item")]
		[MaxLength(100, ErrorMessage = "The title cannot exceed 100 characters")]
		public string Title { get; set; } = string.Empty;

		[Required(ErrorMessage = "Content is required for the news item")]
		[MaxLength(1000, ErrorMessage = "The content cannot exceed 1000 characters")]
		public string Content { get; set; } = string.Empty;
		public string? ImagePath { get; set; } = "";
		public string? Url { get; set; } = "";
	}
}
