namespace Infrastructure.Services.IServices
{
    public interface IApiService
    {
        Task<string> Login(string userName, string password);
    }
}
