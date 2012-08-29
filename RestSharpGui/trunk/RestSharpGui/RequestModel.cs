﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RestSharp;

namespace Swensen.RestSharpGui {
    public class RequestModel {
        public Uri Url { get; private set;}
        public Method Method { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string Body { get; private set; }

        public static List<string> TryCreate(RequestViewModel vm, out RequestModel requestModel) {
            List<string> validationErrors = new List<string>();

            Uri url = null;
            if(String.IsNullOrWhiteSpace(vm.Url))
                validationErrors.Add("Request URL may not be empty");
            else {
                var forgivingUrl = vm.Url.StartsWith("www.") ? "http://" + vm.Url : vm.Url;
                if(!Uri.TryCreate(forgivingUrl, UriKind.Absolute, out url))
                    validationErrors.Add("Request URL is invalid");    
            }


            if(vm.Method == null)
                validationErrors.Add("Request HTTP Method must be selected");

            var headers = new Dictionary<string,string>();
            foreach (var line in vm.Headers) {
                var kv = line.Split(':');
                if (kv.Length != 2)
                    validationErrors.Add("Invalid header line: " + line);
                else {
                    var key = kv[0].Trim();
                    var value = kv[1].Trim();
                    if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value))
                        validationErrors.Add("Invalid header line: " + line);
                    else
                        headers.Add(key, value);
                }
            }

            if (validationErrors.Count > 0)
                requestModel = null;
            else {
                requestModel = new RequestModel() {
                    Url = url,
                    Method = vm.Method.Value,
                    Headers = headers,
                    Body = vm.Body
                };
            }

            return validationErrors;                
        }

        public RestRequest ToRestRequest() {
            var rr = new RestRequest(Url.ToString(), Method);
            rr.AddBody(Body);
            foreach (var header in Headers)
                rr.AddHeader(header.Key, header.Value);

            return rr;
        }
    }
}