namespace Infrastructure.Factory.IFactory
{
    public interface IHttpFactory
    {
        HttpClient GetClient();
    }
}
