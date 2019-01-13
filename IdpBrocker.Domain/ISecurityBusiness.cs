using System.Threading.Tasks;
using IdpBrocker.Domain.Models;

namespace IdpBrocker.Domain
{
    public interface ISecurityBusiness
    {
        Task<TemporaryCredentials> GetSecurityToken(string userName);
    }
}