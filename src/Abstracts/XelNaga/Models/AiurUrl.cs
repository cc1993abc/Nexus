﻿using Aiursoft.XelNaga.Tools;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Aiursoft.XelNaga.Models
{
    public class AiurUrl
    {
        public string Address { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public AiurUrl(string address)
        {
            Address = address;
        }
        public AiurUrl(string address, object param) : this(address)
        {
            var t = param.GetType();
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetValue(param) != null)
                {
                    var propName = prop.Name;
                    var propValue = prop.GetValue(param).ToString();
                    var fromQuery = prop.GetCustomAttributes(typeof(IModelNameProvider), true).FirstOrDefault();
                    if (fromQuery is IModelNameProvider nameProvider && nameProvider.Name != null)
                    {
                        propName = nameProvider.Name;
                    }
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        if (prop.GetValue(param) is DateTime time)
                        {
                            propValue = time.ToString("o", CultureInfo.InvariantCulture);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(propValue))
                    {
                        Params.Add(propName, propValue);
                    }
                }
            }
        }
        public AiurUrl(string host, string path, object param) : this(host + path, param) { }
        public AiurUrl(string host, string controllerName, string actionName, object param) : this(host, $"/{WebUtility.UrlEncode(controllerName)}/{WebUtility.UrlEncode(actionName)}", param) { }
        public override string ToString()
        {
            string appendPart = "?";
            foreach (var param in this.Params)
            {
                appendPart += param.Key.ToLower() + "=" + param.Value.ToUrlEncoded() + "&";
            }
            return this.Address + appendPart.TrimEnd('?', '&');
        }
    }
}
