using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using IdpBrocker.Domain;
using IdpBrocker.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdpBrocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISecurityBusiness securityBusiness;

        public CredentialController(
            IHttpContextAccessor httpContextAccessor,
            ISecurityBusiness securityBusiness)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.securityBusiness = securityBusiness;
        }
        /// <summary>
        /// Get token for identity 
        /// it can be a user name ,role or group
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<TemporaryCredentials> GetToken(string identity)
        {
            TemporaryCredentials creds = await securityBusiness.GetSecurityToken(identity);
            return creds;
        }
    }
}
