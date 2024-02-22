using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using PostmarkDotNet;
using Shouldly;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using Volo.Abp.Settings;

namespace CommunityAbp.Emailing.Postmark.Tests.Emailing
{
    public class PostmarkTests : AbpPostmarkEmailingTestBase
    {
        private readonly IEmailSender _emailSender;
        private ISmtpEmailSenderConfiguration _smtpConfiguration;
        private IBackgroundJobManager _backgroundJobManager;
        private IOptions<AbpPostmarkOptions> _abpPostmarkConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PostmarkTests()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _emailSender = GetRequiredService<IEmailSender>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            _smtpConfiguration = Substitute.For<ISmtpEmailSenderConfiguration>();
            _backgroundJobManager = Substitute.For<IBackgroundJobManager>();
            _abpPostmarkConfiguration = Substitute.For<IOptions<AbpPostmarkOptions>>();
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
            // Arrange
            var invalidApiKey = "invalid-api-key"; // Example of an invalid API key format
            _abpPostmarkConfiguration.Value.Returns(new AbpPostmarkOptions
            {
                ApiKey = invalidApiKey, // Setting an invalid API key
                UsePostmark = true // Assuming the intention is to use Postmark
            });

            var postmarkSender = new PostmarkEmailSender(_smtpConfiguration, _backgroundJobManager, _abpPostmarkConfiguration);

            // Act
            var exception = await Should.ThrowAsync<AbpException>(postmarkSender.BuildClientAsync);

            // Assert
            exception.Message.ShouldContain("Postmark API key is not set or not in the correct format");
        }

        [Fact]
        public async Task SendAsync_WithValidParameters_SendsEmail()
        {
            // Arrange
            string validApiKey = "221d43f5-60c7-4426-96b9-c05508b1aaa0";
            _abpPostmarkConfiguration.Value.Returns(new AbpPostmarkOptions
            {
                ApiKey = validApiKey,
                UsePostmark = true
            });
            _smtpConfiguration.GetDefaultFromAddressAsync().Returns("test@test.com");

            var email = "test@example.com";
            var subject = "Test Subject";
            var body = "Test Body";
            var isBodyHtml = true;
            PostmarkMessage? sentMessage = null;

            var postmarkClientMock = Substitute.For<IAbpPostmarkClient>();
            postmarkClientMock.When(x => x.SendMessageAsync(Arg.Any<PostmarkMessage>())).Do(x => {
                sentMessage = x.Arg<PostmarkMessage>();
            });

            var postmarkSender = new PostmarkEmailSender(_smtpConfiguration, _backgroundJobManager, _abpPostmarkConfiguration, postmarkClientMock);

            // Act
            await postmarkSender.SendAsync(email, subject, body, isBodyHtml);

            // Assert
            sentMessage.ShouldNotBeNull();
            sentMessage.ShouldSatisfyAllConditions(
                x => x.To.ShouldBe(email),
                x => x.Subject.ShouldBe(subject),
                x => x.HtmlBody.ShouldBe(body),
                x => x.TrackOpens.ShouldBe(true)
            );
        }

        [Fact]
        public Task SendAsync_WithBackupEnabled_UsesBackupSender()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        [Fact]
        public Task SendTemplatedEmailAsync_WithValidTemplate_SendsEmail()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        [Fact]
        public Task SendTemplatedEmailAsync_WithInvalidTemplate_ThrowsError()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        [Fact]
        public Task SendAsync_HandlesPostmarkExceptions()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        [Fact]
        public Task BuildClientAsync_ThrowsWhenApiKeyIsInvalid()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        [Fact]
        public void ConfigureClient_SetsExpectedConfiguration()
        {
            // Setup
            // Act
            // Assert
        }

        [Fact]
        public Task ConvertToPostmarkMessage_CreatesExpectedMessage()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }

        // Optional: Integration Test (if applicable)
        // Remember to use this sparingly and with caution, as it involves real external service interaction.
        [Fact(Skip = "Integration test - runs selectively")]
        public Task SendAsync_IntegrationWithActualService()
        {
            // Setup
            // Act
            // Assert
            return Task.CompletedTask;
        }
    }
}