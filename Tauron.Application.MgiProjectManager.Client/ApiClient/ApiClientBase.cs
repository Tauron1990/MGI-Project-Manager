﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.MgiProjectManager.Client.ApiClient
{
    internal abstract class ApiClientBase
    {
        public Func<Task<string>> RetrieveAuthorizationToken { get; set; }

        // Called by implementing swagger client classes
        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();

            if (RetrieveAuthorizationToken == null) return msg;

            var token = await RetrieveAuthorizationToken();
            msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return msg;
        }
    }
}