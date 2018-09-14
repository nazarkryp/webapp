namespace WebApp.Infrastructure.Parsers
{
    public interface IOrderByFilterParser
    {
        string[] Parse<T>(string filter);
    }
}
