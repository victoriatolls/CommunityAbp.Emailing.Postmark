using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.MultiTenancy;

namespace CommunityAbp.Emailing.Postmark;

/// <summary>
/// IPostmarkSmtpEmailSender
/// </summary>
public interface IPostmarkSmtpEmailSender : IEmailSender
{
}

/// <summary>
/// Postmark email sender.
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="currentTenant">
/// The current tenant
/// </param>
/// <param name="smtpConfiguration">
/// The SMTP configuration
/// </param>
/// <param name="backgroundJobManager">
/// The background job manager
/// </param>
/// <param name="abpPostmarkConfiguration">
/// The Postmark configuration
/// </param>
/// <param name="client">
/// The Postmark client
/// </param>
[Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
public class PostmarkEmailSender(ICurrentTenant currentTenant, ISmtpEmailSenderConfiguration smtpConfiguration, IBackgroundJobManager backgroundJobManager, IOptions<AbpPostmarkOptions> abpPostmarkConfiguration, IAbpPostmarkClient? client = null) : EmailSenderBase(currentTenant, smtpConfiguration, backgroundJobManager), IPostmarkSmtpEmailSender
{
    /// <summary>
    /// You can also send to postmark using Smtp, so if the user has not configured Postmark, we can use the SmtpEmailSender as a backup
    /// but that'll also work if postmark is not enabled in the options.
    /// </summary>
    private readonly SmtpEmailSender _backupSender = new(currentTenant, smtpConfiguration, backgroundJobManager);

    private IAbpPostmarkClient? _client = client;

    /// <summary>
    /// Postmark options
    /// </summary>
    protected AbpPostmarkOptions AbpPostmarkOptions { get; } = abpPostmarkConfiguration.Value;

    /// <summary>
    /// Default SMTP configuration, used for backup
    /// </summary>
    protected ISmtpEmailSenderConfiguration SmtpConfiguration { get; } = smtpConfiguration;

    /// <summary>
    /// Construct a Postmark client
    /// </summary>
    /// <returns></returns>
    /// <exception cref="AbpException"></exception>
    public async Task<IAbpPostmarkClient> BuildClientAsync()
    {
        var apiKeyValue = AbpPostmarkOptions.ApiKey ?? await SmtpConfiguration.GetUserNameAsync();
        if (!Guid.TryParse(apiKeyValue, out var apiKey))
        {
            throw new AbpException("Postmark API key is not set or not in the correct format!");
        }
        _client ??= new AbpPostmarkClient(apiKey.ToString());

        return _client;
    }

    /// <inheritdoc />
    public override async Task SendAsync(string to, string? subject, string? body, bool isBodyHtml = true, AdditionalEmailSendingArgs? additionalEmailSendingArgs = null)
    {
        var mailMessage = BuildMailMessage(null, to, subject, body, isBodyHtml, additionalEmailSendingArgs);

        // Check if we are sending a templated email
        if (additionalEmailSendingArgs?.ExtraProperties?.ContainsKey(AbpPostmarkConsts.PostmarkTemplateId) == true && (AbpPostmarkOptions.UsePostmark ?? false))
        {
            // Safely attempt to retrieve and unbox the PostmarkTemplateId
            var postmarkTemplateIdObj = additionalEmailSendingArgs.ExtraProperties?.GetOrDefault(AbpPostmarkConsts.PostmarkTemplateId);
            var postmarkTemplateId = postmarkTemplateIdObj is long v ? v : default; // Use default(long) or a specific default value

            // Safely attempt to retrieve and cast the TemplateModel
            var templateModelObj = additionalEmailSendingArgs.ExtraProperties?.GetOrDefault(AbpPostmarkConsts.TemplateModel);
            var templateModel = templateModelObj as Dictionary<string, object> ?? [];

            if (postmarkTemplateId != default)
            {
                await SendTemplatedEmailAsync(mailMessage, postmarkTemplateId, templateModel);
            }
            else
            {
                await SendAsync(mailMessage);
            }
        }
        else if (additionalEmailSendingArgs?.ExtraProperties?.ContainsKey(AbpPostmarkConsts.PostmarkAlias) == true)
        {
            // Safely attempt to retrieve and unbox the PostmarkTemplateId
            var postmarkAliasObj = additionalEmailSendingArgs?.ExtraProperties?.GetOrDefault(AbpPostmarkConsts.PostmarkAlias);
            var postmarkAlias = postmarkAliasObj is string v ? v : default; // Use default(string) or a specific default value

            // Safely attempt to retrieve and cast the TemplateModel
            var templateModelObj = additionalEmailSendingArgs?.ExtraProperties?.GetOrDefault(AbpPostmarkConsts.TemplateModel);
            var templateModel = templateModelObj as Dictionary<string, object> ?? [];

            if (postmarkAlias != default)
            {
                await SendTemplatedEmailAsync(mailMessage, postmarkAlias, templateModel);
            }
            else
            {
                await SendAsync(mailMessage);
            }
        }
        else
        {
            // Handle non-templated email sending (existing logic)
            await SendAsync(mailMessage);
        }
    }
    
    private async Task SendTemplatedEmailAsync(MailMessage mail, long postmarkTemplateId, Dictionary<string, object> templateModel)
    {
        var client = await BuildClientAsync();
        var mailMessage = await ConvertToPostmarkMessage(mail);

        // Now, instead of setting the body, we'll use the template fields
        var templatedMessage = new TemplatedPostmarkMessage
        {
            From = mailMessage.From,
            To = mailMessage.To,
            Cc = mailMessage.Cc,
            Bcc = mailMessage.Bcc,
            TemplateId = postmarkTemplateId,
            TemplateModel = templateModel,
            Headers = mailMessage.Headers,
            Attachments = mailMessage.Attachments,
            ReplyTo = mailMessage.ReplyTo,
            Tag = mailMessage.Tag,
            TrackOpens = mailMessage.TrackOpens,
            TrackLinks = mailMessage.TrackLinks,
            MessageStream = mailMessage.MessageStream
        };

        await client.SendEmailWithTemplateAsync(templatedMessage);
    }

    private async Task SendTemplatedEmailAsync(MailMessage mail, string postmarkAlias, Dictionary<string, object> templateModel)
    {
        var client = await BuildClientAsync();
        var mailMessage = await ConvertToPostmarkMessage(mail);

        // Now, instead of setting the body, we'll use the template fields
        var templatedMessage = new TemplatedPostmarkMessage
        {
            From = mailMessage.From,
            To = mailMessage.To,
            Cc = mailMessage.Cc,
            Bcc = mailMessage.Bcc,
            TemplateAlias = postmarkAlias,
            TemplateModel = templateModel,
            Headers = mailMessage.Headers,
            Attachments = mailMessage.Attachments,
            ReplyTo = mailMessage.ReplyTo,
            Tag = mailMessage.Tag,
            TrackOpens = mailMessage.TrackOpens,
            TrackLinks = mailMessage.TrackLinks,
            MessageStream = mailMessage.MessageStream
        };

        await client.SendEmailWithTemplateAsync(templatedMessage);
    }
    
    /// <summary>
    /// Send the email
    /// </summary>
    /// <param name="mail"></param>
    protected override async Task SendEmailAsync(MailMessage mail)
    {
        if (AbpPostmarkOptions.UsePostmark == false)
        {
            await _backupSender.SendAsync(mail);
            return;
        }

        var c = await BuildClientAsync();
        var mailMessage = await ConvertToPostmarkMessage(mail);
        await c.SendMessageAsync(mailMessage);
    }

    private async Task<PostmarkMessage> ConvertToPostmarkMessage(MailMessage mail)
    {
        var from = mail.From?.Address ?? await SmtpConfiguration.GetDefaultFromAddressAsync();
        // Convert the headers to a dictionary
        var headerDict = new Dictionary<string, string>();
        foreach (var key in mail.Headers.AllKeys)
        {
            if (!string.IsNullOrEmpty(key) && !headerDict.ContainsKey(key))
                headerDict.Add(key, mail.Headers[key] ?? string.Empty);
        }

        var mailMessage = new PostmarkMessage
        {
            From = from,
            To = string.Join(", ", mail.To.Select(to => to.Address)),
            Cc = string.Join(", ", mail.CC.Select(cc => cc.Address)),
            Bcc = string.Join(", ", mail.Bcc.Select(bcc => bcc.Address)),
            Subject = mail.Subject,
            HtmlBody = mail.IsBodyHtml ? mail.Body : null,
            TextBody = !mail.IsBodyHtml ? mail.Body : null,
            ReplyTo = mail.ReplyToList.Select(replyTo => replyTo.Address).FirstOrDefault(),
            Headers = new HeaderCollection(headerDict),
            TrackOpens = AbpPostmarkOptions.TrackOpens,
            TrackLinks = AbpPostmarkOptions.TrackLinks
        };

        // Handle attachments
        if (mail.Attachments.Any())
        {
            foreach (var attachment in mail.Attachments)
            {
                attachment.ContentStream.Position = 0; // Ensure stream is at the beginning
                mailMessage.AddAttachment(attachment.ContentStream, attachment.Name, attachment.ContentType.MediaType, attachment.ContentId);
            }
        }

        return mailMessage;
    }

    private async Task SendTemplatedEmailAsync(MailMessage mail, long postmarkTemplateId, Dictionary<string, object> templateModel)
    {
        var c = await BuildClientAsync();
        var mailMessage = await ConvertToPostmarkMessage(mail);

        // Now, instead of setting the body, we'll use the template fields
        var templatedMessage = new TemplatedPostmarkMessage
        {
            From = mailMessage.From,
            To = mailMessage.To,
            Cc = mailMessage.Cc,
            Bcc = mailMessage.Bcc,
            TemplateId = postmarkTemplateId,
            TemplateModel = templateModel,
            Headers = mailMessage.Headers,
            Attachments = mailMessage.Attachments,
            ReplyTo = mailMessage.ReplyTo,
            Tag = mailMessage.Tag,
            TrackOpens = mailMessage.TrackOpens,
            TrackLinks = mailMessage.TrackLinks,
            MessageStream = mailMessage.MessageStream
        };

        await c.SendEmailWithTemplateAsync(templatedMessage);
    }
}