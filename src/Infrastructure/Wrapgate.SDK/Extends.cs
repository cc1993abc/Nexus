﻿using Aiursoft.Scanner;
using Aiursoft.Wrapgate.SDK.Models.ViewModels;
using Aiursoft.Wrapgate.SDK.Services;
using Aiursoft.XelNaga.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace Aiursoft.Wrapgate.SDK
{
    public static class Extends
    {
        public static IServiceCollection AddWrapgateServer(this IServiceCollection services, string wrapgateEndpoint)
        {
            AsyncHelper.TryAsyncThreeTimes(async () =>
            {
                var response = await new WebClient().DownloadStringTaskAsync(wrapgateEndpoint);
                var serverModel = JsonConvert.DeserializeObject<IndexViewModel>(response);
                services.AddSingleton(new WrapgateLocator(wrapgateEndpoint, serverModel.WrapPattern));
            });
            services.AddLibraryDependencies();
            return services;
        }
    }
}
