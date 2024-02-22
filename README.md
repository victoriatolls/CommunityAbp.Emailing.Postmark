# CommunityAbp.Emailing.Postmark
Postmark is a cross-platform, popular open source mail client library for .net. This abp module provides an integration package to use the Postmark as the email sender.

## Getting Started

Into your abp Domain project, install the library:
```powershell
Install-Package CommunityAbp.Emailing.Postmark
```

Then, into your abp Domain project, add the following code to your module class:
```csharp
[DependsOn(typeof(AbpPostmarkModule))]
public class YourProjectDomainModule : AbpModule
{
    // ..

	public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Setup postmark
        var configuration = context.Services.GetConfiguration();
        Configure<AbpPostmarkOptions>(options =>
        {
            options.UsePostmark = configuration.GetValue("Postmark:Enabled", false);
            options.ApiKey = configuration.GetValue("Postmark:ApiKey", string.Empty);
        });
    }
}
```

Finally, into your project that is sending email, add the following code to your appsettings.json file:
```json
{
  "Postmark": {
    "Enabled": true,
	"ApiKey": "(your server's API key from Postmark"
  }
}
```

## Usage

It's not used any differently than the default [Emailing Sending System](https://docs.abp.io/en/abp/latest/Emailing) from abp. You simply inject IEmailSender and use as you need. If postmark is enabled, it'll send through postmark.

## Advanced Usage

You can send [Postmark Templated Emails](https://postmarkapp.com/email-templates)!

```csharp
var templateModel = new Dictionary<string, object?>
{
    { "product_url", "product_url_Value" },
    { "product_name", "product_name_Value" },
    { "name", "name_Value" },
    { "action_url", "action_url_Value" },
    { "login_url", "login_url_Value" },
    { "username", "username_Value" },
    { "trial_length", "trial_length_Value" },
    { "trial_start_date", "trial_start_date_Value" },
    { "trial_end_date", "trial_end_date_Value" },
    { "support_email", "support_email_Value" },
    { "live_chat_url", "live_chat_url_Value" },
    { "sender_name", "sender_name_Value" },
    { "help_url", "help_url_Value" },
    { "company_name", "company_name_Value" },
    { "company_address", "company_address_Value" },
};
var prop = new Dictionary<string, object?> {
    { AbpPostmarkConsts.PostmarkTemplateId, 34989179L },
    { AbpPostmarkConsts.TemplateModel, templateModel }
  };
await _emailSender.SendAsync("your_cool_address@someprovider.com", null, null, additionalEmailSendingArgs: new AdditionalEmailSendingArgs() { ExtraProperties = new ExtraPropertyDictionary(prop) });
```

**Note**: I'm not setting a subject or body, that's because the template has those things defined so they just get ignored here.
