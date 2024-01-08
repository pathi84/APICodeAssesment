using angulartask.Interface;

namespace angulartask.Service
{
    public class StoriesRepository : IStoriesRepository
    {
        private static HttpClient client = new HttpClient();

        public StoriesRepository()
        {

        }

        public async Task<HttpResponseMessage> TopStoriesAsync()
        {
            return await client.GetAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
        }

        public async Task<HttpResponseMessage> GetStoryByIdAsync(int id)
        {
            return await client.GetAsync(string.Format("https://hacker-news.firebaseio.com/v0/item/{0}.json", id));
        }
    }
}
