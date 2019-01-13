using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace IdpBrocker.Domain.Models
{
    public class TemporaryCredentials
    {
        public string User { get; set; }

        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }

        public string SecurityToken { get; set; }

        public DateTime Expiration { get; set; }

    }
}
