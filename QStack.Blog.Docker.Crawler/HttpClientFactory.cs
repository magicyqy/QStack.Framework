﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QStack.Blog.Docker.Crawler
{
	public static class HttpClientFactory
	{
		private static readonly Dictionary<string, HttpClient> _httpClients = new Dictionary<string, HttpClient>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static HttpClient GetHttpClient(string name, string user, string password)
		{
			name = string.IsNullOrWhiteSpace(name) ? "default" : name;
			if (!_httpClients.ContainsKey(name))
			{
				var httpClient = new HttpClient(new SocketsHttpHandler
				{
					Credentials = string.IsNullOrWhiteSpace(user) ? null : new NetworkCredential(user, password)
				});
				_httpClients.Add(name, httpClient);
			}

			return _httpClients[name];
		}
	}
}
