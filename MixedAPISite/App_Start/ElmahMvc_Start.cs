using System.Web.Http;
using MixedAPISite.Models;
[assembly: WebActivator.PreApplicationStartMethod(typeof(MixedAPISite.App_Start.ElmahMvc), "Start")]
namespace MixedAPISite.App_Start
{
    public class ElmahMvc
    {
        public static void Start()
        {
            Elmah.Mvc.Bootstrap.Initialize();

            // set up elmah to handle errors on web api requests as well.
            GlobalConfiguration.Configuration.Filters.Add(new ElmahErrorAttribute());
        }
    }
}