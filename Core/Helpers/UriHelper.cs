using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
	public static class UriHelper
	{
		public static string? CombineToFrontUrl(this HttpRequest req, string? path, object? values)
		{
			if (req == null) return null;
			var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
			if (uriBuilder.Uri.IsDefaultPort)
			{
				uriBuilder.Port = -1;
			}

			if (!string.IsNullOrEmpty(path))
			{
				uriBuilder.Path = path;
			}

			if (values != null)
			{
				var props = values.GetType()?.GetProperties();
				var pairs = props?.Select(x => x.Name + "=" + x.GetValue(values, null)).ToArray();
				if (pairs != null)
				{
					var result = string.Join("&", pairs);
					uriBuilder.Query = result;
				}
			}

			return uriBuilder.Uri.AbsoluteUri;
		}

        public static string? RootFrontUrl(this HttpRequest req)
        {
            return req.CombineToFrontUrl(null, null);
        }
    }
}
