using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Entities;
using Entities.Domain;
using RepositoryPattern.Infrastructure;
using Utils.DependencyInjection;

namespace Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        //public void ConfigureAuth(IAppBuilder app)
        //{
        //    // Enable the application to use a cookie to store information for the signed in user
        //    app.UseCookieAuthentication(new CookieAuthenticationOptions
        //    {
        //        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        //        LoginPath = new PathString("/Account/Login")
        //    });
        //    // Use a cookie to temporarily store information about a user logging in with a third party login provider
        //    //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        //    EngineContext.Initialize(false);

        //    DependencyResolver.SetResolver(new NoisDependencyResolver());

        //    var scope = EngineContext.Current.ContainerManager.Scope();
        //    app.UseAutofacMiddleware(scope);

        //    TaskManager.Instance.Initialize();
        //    TaskManager.Instance.Start();
        //}
    }
}