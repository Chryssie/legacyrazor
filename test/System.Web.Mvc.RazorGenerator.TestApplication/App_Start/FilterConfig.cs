using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc.RazorGenerator.TestApplication
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
