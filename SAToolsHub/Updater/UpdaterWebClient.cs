using System;
using System.Net;

namespace SAToolsHub.Updater
{
	public class UpdaterWebClient : WebClient
	{
		protected override WebRequest GetWebRequest(Uri address)
		{
			var request = base.GetWebRequest(address) as HttpWebRequest;

			if (request != null)
			{
				request.UserAgent = "sa-tools";
				request.Timeout = 5000;
			}

			return request;
		}
	}
}
