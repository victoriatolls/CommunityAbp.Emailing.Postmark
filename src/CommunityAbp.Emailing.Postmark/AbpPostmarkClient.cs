using PostmarkDotNet;
using System.Threading.Tasks;

namespace CommunityAbp.Emailing.Postmark;

/// <summary>
/// Wrapper used because Postmark doesn't provide an interface for their client
/// or a parameterless constructor for use with testing
/// </summary>
public interface IAbpPostmarkClient
{
    /// <summary>
    /// SendEmailWithTemplateAsync
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<PostmarkResponse> SendEmailWithTemplateAsync(TemplatedPostmarkMessage message);

    /// <summary>
    /// SendMessageAsync
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<PostmarkResponse> SendMessageAsync(PostmarkMessage message);
}

/// <summary>
/// Wrapper used because Postmark doesn't provide an interface for their client
/// or a parameterless constructor for use with testing
/// </summary>
public class AbpPostmarkClient : IAbpPostmarkClient
{
    private readonly PostmarkClient _client;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="apiKey"></param>
    public AbpPostmarkClient(string apiKey)
    {
        _client = new PostmarkClient(apiKey);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="client">Existing client</param>
    public AbpPostmarkClient(PostmarkClient client)
    {
        _client = client;
    }

    /// <summary>
    /// SendEmailWithTemplateAsync
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<PostmarkResponse> SendEmailWithTemplateAsync(TemplatedPostmarkMessage message)
    {
        return _client.SendEmailWithTemplateAsync(message);
    }

    /// <summary>
    /// SendMessageAsync
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task<PostmarkResponse> SendMessageAsync(PostmarkMessage message)
    {
        return _client.SendMessageAsync(message);
    }
}