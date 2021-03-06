﻿using Aiursoft.Handler.Models;
using System;

namespace Aiursoft.Handler.Exceptions
{
    /// <summary>
    /// Throw this exception if the json respond by the Aiursoft Server was not with code = 0.
    /// Catch it in your own code or just use `AiurExpHandler`.
    /// </summary>
    public class AiurUnexpectedResponse : Exception
    {
        public AiurProtocol Response { get; set; }
        public ErrorType Code => Response.Code;
        public AiurUnexpectedResponse(AiurProtocol response) : base(response.Message)
        {
            Response = response;
        }
    }
}
