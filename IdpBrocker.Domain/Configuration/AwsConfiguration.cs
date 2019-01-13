using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdpBrocker.Domain.Configuration
{
    public class AwsConfiguration
    {
        public string AWSAccessKey { get; set; } = "Your-AWS-AccessKey";
        public string AWSSecretKey { get; set; } = "Your-AWS-SecretKey";
        public string PersonalFoldersBucket { get; set; } = "Your-Default-Bucketname";
        public string PolicyAdministrator { get; set; }
        public string PolicyUser0 { get; set; }
        public string PolicyUser1 { get; set; }

        public static string AwsConfigSection = "AwsConfiguration";

        public void ValidateConfiguration()
        {
            if (((AWSAccessKey.Equals("") || AWSAccessKey.Equals("AccessKey"))) ||
                ((AWSSecretKey.Equals("") || AWSSecretKey.Equals("SecretKey"))) ||
                ((PersonalFoldersBucket.Equals("") || PersonalFoldersBucket.Equals("Bucket"))))
            {
                throw new Exception(
                    "verify if all required keys " + 
                    $"{nameof(PersonalFoldersBucket)},{nameof(AWSAccessKey)},{nameof(AWSSecretKey)}) "+
                    "are set in configuration file and their values are valid ones");
            }
        }

        public string BuildAWSPolicy(string awsUsername)
        {
            // Split username@domain to retrieve the username only
            string[] usernameParts = awsUsername.Split('@');
            string uName = usernameParts[0];

            // Prepend the username with "policy_" string.
            // Retrieve Access policy corresponding to the "policy_"+username 
            // key from web.config file.

            string policy = (string)this.GetType().GetProperty("Policy" + uName).GetValue(this);

            // Check if valid policy was retrieved from Web.config file
            if (string.IsNullOrWhiteSpace(policy))
            {
                throw new Exception("Unable to fetch policy for '" + awsUsername +
                "' from Web.config");
            }

            // Replace the [*] values with real values at runtime.
            policy = policy.Replace("[BUCKET-NAME]", PersonalFoldersBucket);
            policy = policy.Replace("[USER-NAME]", uName.ToLowerInvariant());
            policy = policy.Replace("'", "\"");
            return policy;
        }

        public string BuildAWSUsername(string userName)
        {
            string[] usernameParts = userName.Split('\\');
            if (!CheckSpecialCharacters(usernameParts[1]))
            {
                throw new Exception("SampleFederationProxy :: Windows username retrieved " +
                "should strictly be alphanumeric to run this application");
            }
            // Return result in the form of username@domain
            return usernameParts[1] + "@" + usernameParts[0];
        }
        //----------------------------------------------------------------------
        // Function to check if user name contains special characters
        // only alphanumeric usernames are permitted.
        //----------------------------------------------------------------------
        public Boolean CheckSpecialCharacters(string userName)
        {
            string pattern = @"^[a-zA-Z0-9]*$";
            Regex unameRgx = new Regex(pattern);
            return unameRgx.IsMatch(userName);
        }

    }
}
