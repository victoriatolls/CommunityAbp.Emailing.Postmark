using Volo.Abp.Emailing;
using Volo.Abp.Modularity;

namespace CommunityAbp.Emailing.Postmark;

/// <summary>
/// Module for Postmark email provider.
/// </summary>
[DependsOn(typeof(AbpEmailingModule))]
public class AbpPostmarkModule : AbpModule
{
}