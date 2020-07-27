using System.Threading.Tasks;
using Modules.UserAccess.Application.Contracts;

namespace Modules.UserAccess.Infrastructure
{
    public class UserAccessModule : IUserAccessModule
    {
        public Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
        {
            throw new System.NotImplementedException();
        }

        public Task ExecuteCommandAsync(ICommand command)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            throw new System.NotImplementedException();
        }
    }
}