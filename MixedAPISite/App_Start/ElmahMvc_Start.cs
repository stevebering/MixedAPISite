[assembly: WebActivator.PreApplicationStartMethod(typeof(MixedAPISite.App_Start.ElmahMvc), "Start")]
namespace MixedAPISite.App_Start
{
    public class ElmahMvc
    {
        public static void Start()
        {
            Elmah.Mvc.Bootstrap.Initialize();
        }
    }
}