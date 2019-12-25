using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;
using Scrutor;

namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    [ServiceDescriptor(typeof(DynamicCredStore))]
    public sealed class DynamicCredStore : ICredentialStore
    {
        public ICredentialStore CredentialStore { get; set; } = new InMemoryCredentialStore(Credentials.Anonymous);


        public async Task<Credentials> GetCredentials() => await CredentialStore.GetCredentials();
    }
}