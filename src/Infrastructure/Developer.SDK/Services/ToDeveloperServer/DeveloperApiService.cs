﻿using Aiursoft.Developer.SDK.Models.ApiAddressModels;
using Aiursoft.Developer.SDK.Models.ApiViewModels;
using Aiursoft.Handler.Exceptions;
using Aiursoft.Handler.Models;
using Aiursoft.Scanner.Interfaces;
using Aiursoft.XelNaga.Models;
using Aiursoft.XelNaga.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aiursoft.Developer.SDK.Services.ToDeveloperServer
{
    public class DeveloperApiService : IScopedDependency
    {
        private readonly DeveloperLocator _serviceLocation;
        private readonly HTTPService _http;
        public DeveloperApiService(
            DeveloperLocator serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<bool> IsValidAppAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            return jResult.Code == ErrorType.Success;
        }

        public async Task<AppInfoViewModel> AppInfoAsync(string appId)
        {
            var url = new AiurUrl(_serviceLocation.Endpoint, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);
            if (jResult.Code != ErrorType.Success)
                throw new AiurUnexpectedResponse(jResult);
            return jResult;
        }
    }
}
