using PostmarkDotNet;

namespace CommunityAbp.Emailing.Postmark;

/// <summary>
/// Postmark constants
/// </summary>
public static class AbpPostmarkConsts
{
    /// <summary>
    /// When sending a templated email, you can specify a template id to use for the email.
    /// </summary>
    public const string PostmarkTemplateId = "PostmarkTemplateId";

    /// <summary>
    /// When sending a templated email, you can specify a alias for the template to use for the email.
    /// </summary>
    public const string PostmarkAlias = "PostmarkAlias";

    /// <summary>
    /// When sending a templated email, you can specify a model to use for the template.
    /// </summary>
    public const string TemplateModel = "TemplateModel";
}

/// <summary>
/// Options for Postmark
/// </summary>
public class AbpPostmarkOptions
{
    /// <summary>
    /// The API key to use for sending emails through Postmark.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// See <a href="https://postmarkapp.com/developer/user-guide/tracking-links#enabling-link-tracking">Postmark Documentation</a> for more information.
    /// </summary>
    public LinkTrackingOptions TrackLinks { get; set; } = LinkTrackingOptions.None;

    /// <summary>
    /// See <a href="https://postmarkapp.com/developer/user-guide/tracking-opens">Postmark Documentation</a> for more information.
    /// </summary>
    public bool TrackOpens { get; set; } = true;

    /// <summary>
    /// Should we use Postmark for sending emails?
    /// </summary>
    public bool? UsePostmark { get; set; }
}