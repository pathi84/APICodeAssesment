namespace angulartask.Interface
{
    public interface IStoriesRepository
    {
        Task<HttpResponseMessage> TopStoriesAsync();
        Task<HttpResponseMessage> GetStoryByIdAsync(int id);
    }
}
