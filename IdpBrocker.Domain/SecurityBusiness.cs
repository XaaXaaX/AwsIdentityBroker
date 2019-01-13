using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using IdpBrocker.Domain.Configuration;
using IdpBrocker.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace IdpBrocker.Domain
{
    public class SecurityBusiness : ISecurityBusiness
    {
        private const int DurationHours = 8;
        private readonly AwsConfiguration awsConfiguration;
        public SecurityBusiness(IOptions<AwsConfiguration> options) => awsConfiguration = options.Value;
        public async Task<TemporaryCredentials> GetSecurityToken(string identity)
        {
            TemporaryCredentials temporaryCreds = new TemporaryCredentials();
            Credentials sessionCredentials;
            awsConfiguration.ValidateConfiguration();
            AmazonSecurityTokenServiceClient client = new AmazonSecurityTokenServiceClient(
                                                          awsConfiguration.AWSAccessKey,
                                                          awsConfiguration.AWSSecretKey,
                                                          new AmazonSecurityTokenServiceConfig());
            string awsUsername = awsConfiguration.BuildAWSUsername(identity);
            string policy = awsConfiguration.BuildAWSPolicy(awsUsername);
            GetFederationTokenRequest request = new GetFederationTokenRequest
            {
                // default is 12 hours, we convert it to seconds 
                // Min 900 (15 min)
                // Max 36 hours 
                DurationSeconds = DurationHours * 3600,
                Name = awsUsername,
                Policy = policy
            };
            GetFederationTokenResponse startSessionResponse = null;
            startSessionResponse = await client.GetFederationTokenAsync(request);
            if (startSessionResponse != null)
            {
                sessionCredentials = startSessionResponse.Credentials;
                temporaryCreds.User = identity;
                temporaryCreds.AccessKeyId = sessionCredentials.AccessKeyId;
                temporaryCreds.SecretAccessKey = sessionCredentials.SecretAccessKey;
                temporaryCreds.Expiration = sessionCredentials.Expiration;
                temporaryCreds.SecurityToken = sessionCredentials.SessionToken;
                return temporaryCreds;
            }
            else
            {
                throw new Exception("Error while federation token fetch process");
            }
        }
    }
}
