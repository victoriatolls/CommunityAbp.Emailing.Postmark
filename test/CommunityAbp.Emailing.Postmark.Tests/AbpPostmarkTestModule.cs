using CommunityAbp.Emailing.Postmark.TestBase;
using Volo.Abp.Modularity;

namespace CommunityAbp.Emailing.Postmark.Tests
{
    [DependsOn(
        typeof(AbpPostmarkModule),
        typeof(AbpPostmarkTestBaseModule)
    )]
    public class AbpPostmarkTestModule : AbpModule
    {

    }
}