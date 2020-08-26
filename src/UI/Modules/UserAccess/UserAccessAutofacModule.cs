using Autofac;
using Modules.UserAccess.Application.Contracts;
using Modules.UserAccess.Infrastructure;

namespace UI.Modules.UserAccess
{
    public class UserAccessAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserAccessModule>()
                .As<IUserAccessModule>()
                .InstancePerLifetimeScope();
        }
    }
}