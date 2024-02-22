using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;

namespace CommunityAbp.Emailing.Postmark.Tests.Emailing
{
    public class PostmarkTests : AbpPostmarkEmailingTestBase
    {
        private readonly IEmailSender _emailSender;
        private ISmtpEmailSenderConfiguration _smtpConfiguration;
        private IBackgroundJobManager _backgroundJobManager;
        private IOptions<AbpPostmarkOptions> _abpPostmarkConfiguration;
        private PostmarkEmailSender _postmarkEmailSender;

        public PostmarkTests()
        {
            _emailSender = GetRequiredService<IEmailSender>();
            _smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _abpPostmarkConfiguration = Substitute.For<IOptions<AbpPostmarkOptions>>();
            _postmarkEmailSender = new PostmarkEmailSender(_smtpConfiguration, _backgroundJobManager, _abpPostmarkConfiguration);
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            _smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _abpPostmarkConfiguration = Substitute.For<IOptions<AbpPostmarkOptions>>();
            _postmarkEmailSender = new PostmarkEmailSender(_smtpConfiguration, _backgroundJobManager, _abpPostmarkConfiguration);
        }

        [Fact]
        public void CanInstantiate()
        {
            _emailSender.ShouldSatisfyAllConditions(
                x => x.ShouldNotBeNull(),
                x => x.ShouldBeOfType<PostmarkEmailSender>()
            );
        }

        [Fact]
        public async Task ConfigurationThrowsWhenApiKeyInvalid()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task SendAsync_WithValidParameters_SendsEmail()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task SendAsync_WithBackupEnabled_UsesBackupSender()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task SendTemplatedEmailAsync_WithValidTemplate_SendsEmail()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task SendTemplatedEmailAsync_WithInvalidTemplate_ThrowsError()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task SendAsync_HandlesPostmarkExceptions()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task BuildClientAsync_ThrowsWhenApiKeyIsInvalid()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public void ConfigureClient_SetsExpectedConfiguration()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public async Task ConvertToPostmarkMessage_CreatesExpectedMessage()
        {
            // Setup
            // Act
            // Assert
        }

        // Optional: Integration Test (if applicable)
        // Remember to use this sparingly and with caution, as it involves real external service interaction.
        [Fact(Skip = "Integration test - runs selectively")]
        public async Task SendAsync_IntegrationWithActualService()
        {
            // Setup
            // Act
            // Assert
        }
    }
}