using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Emailing;


namespace CommunityAbp.Emailing.Postmark.Tests.Emailing
{
    public class PostmarkTests : AbpPostmarkEmailingTestBase
    {
        private readonly IEmailSender _emailSender;

        public PostmarkTests()
        {
            _emailSender = GetRequiredService<IEmailSender>();
        }

        [Fact]
        public void CanInstantiate()
        {
            _emailSender.ShouldSatisfyAllConditions(
                x => x.ShouldNotBeNull(),
                x => x.ShouldBeOfType<PostmarkEmailSender>()
            );
        }
    }
}
