using angulartask.Interface;
using HackerStory.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoriesRepository _storyRepo;
        public StoriesController(IMemoryCache memoryCache,IStoriesRepository storyRepo)
        {
            _storyRepo = storyRepo;
            MemoryCache = memoryCache;
        }

        const string newStoriesApi = "https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty";
        const string StoryApi = "https://hacker-news.firebaseio.com/v0/item/{0}.json?print=pretty";

        private static HttpClient client = new HttpClient();

        public IMemoryCache MemoryCache { get; }

        [HttpGet]
        public async Task<IActionResult> GetStoriesAsync(string? searchTerm)
        {
            try
            {
                List<Stories> stories = new List<Stories>();

                var response = await _storyRepo.TopStoriesAsync();
                if (response.IsSuccessStatusCode)
                {
                    var storiesResponse = response.Content.ReadAsStringAsync().Result;
                    var newStoryIDs = JsonSerializer.Deserialize<List<int>>(storiesResponse);

                    var tasks = newStoryIDs.OrderByDescending(x => x).Select(GetStoryAsync);
                    stories = (await Task.WhenAll(tasks)).ToList();
                    string searchString = "";
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        var search = searchString.ToLower();

                        stories = stories.Where(s =>
                                            s.title.ToLower().IndexOf(search) > -1 || s.by.ToLower().IndexOf(search) > -1)
                                            .ToList();
                    }
                }

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, "exception occured in API");
            }

}


        private async Task<Stories> GetStoryAsync(int storyId)
        {
            return await MemoryCache.GetOrCreateAsync<Stories>(storyId,
                async cacheEntry =>
                {
                    Stories story = new Stories();

                    var response = await _storyRepo.GetStoryByIdAsync(storyId);
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonSerializer.Deserialize<Stories>(storyResponse);
                    }
                    else
                    {
                        story.title = string.Format("Exception Occured while fetching the story: (ID {0})", storyId);
                    }

                    return story;
                });
        }
    }
}