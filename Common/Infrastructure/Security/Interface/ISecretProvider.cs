namespace Common.Infrastructure.Security.Interface
{
    public interface ISecretProvider
    {
        string GetSecret(string secretName);
    }
}