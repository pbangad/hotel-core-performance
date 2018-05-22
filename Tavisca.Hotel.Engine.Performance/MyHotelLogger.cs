using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavisca.Platform.Common;
using Tavisca.Platform.Common.Logging;
using Tavisca.Platform.Common.Plugins.Json;

namespace Tavisca.Hotel.Engine.Performance
{
    public class MyHotelLogger : Hotel_Sdk.Internals.ILogger
    {
        public async Task Log(Hotel_Sdk.Internals.RequestMessage requestMessage, Hotel_Sdk.Internals.ResponseMessage responseMessage)
        {
            var urlMapping = GetRouteMapping(requestMessage.Url);

            var apiLog = new ApiLog();
            apiLog.Request = new Payload(ByteHelper.ToByteArrayUsingJsonSerialization(requestMessage));
            apiLog.Response = new Payload(ByteHelper.ToByteArrayUsingJsonSerialization(responseMessage));
            apiLog.CorrelationId = responseMessage.Headers[HeaderNames.CorrelationId];
            apiLog.TenantId = responseMessage.Headers[HeaderNames.TenantId];
            apiLog.StackId = responseMessage.Headers[HeaderNames.StackId];
            apiLog.TimeTakenInMs = responseMessage.ResponseTimeMS;
            apiLog.ApplicationName = "be_perf_hotel";
            apiLog.Api = urlMapping.Item1;
            apiLog.Verb = urlMapping.Item2;
            apiLog.IsSuccessful = true;
            apiLog.SetValue("http_status_code", (int)responseMessage.StatusCode);

            foreach (var item in requestMessage.Headers.AllKeys)
            {
                apiLog.RequestHeaders.Add(item, requestMessage.Headers[item]);
            }

            foreach (var item in responseMessage.Headers.AllKeys)
            {
                apiLog.ResponseHeaders.Add(item, responseMessage.Headers[item]);
            }
            //LogTime.LogTimeMapping.TryAdd(apiLog.CorrelationId, responseMessage.ResponseTimeMS);
            await Logger.WriteLogAsync(apiLog);
            //  Console.WriteLine("(MyLogger)CorrelationID " + apiLog.CorrelationId + " TimeTaken: " + apiLog.TimeTakenInMs);
        }

        private static Tuple<string, string> GetRouteMapping(string url)
        {
            var route = url;

            foreach (var callType in RouteMap)
                if (route.Contains(callType.Key))
                    return callType.Value;

            return new Tuple<string, string>(route, route);
        }

        private static readonly Dictionary<string, Tuple<string, string>> RouteMap = new Dictionary<string, Tuple<string, string>>
        {
            {"rooms/search/init/stateless", new Tuple<string, string>("room_search", "init_stateless")},
            {"rooms/search/init", new Tuple<string, string>("room_search", "init")},
            {"rooms/search/status", new Tuple<string, string>("room_search", "status")},
            {"rooms/search/results", new Tuple<string, string>("room_search", "results")},
            {"search/init", new Tuple<string, string>("search", "init")},
            {"search/status", new Tuple<string, string>("search", "status")},
            {"search/results/all", new Tuple<string, string>("search", "results_all")},
            {"search/results", new Tuple<string, string>("search", "results")},
            {"rooms/price", new Tuple<string, string>("rooms_price", "rooms_price")},
            {"book/init", new Tuple<string, string>("book", "init")},
            {"book/status", new Tuple<string, string>("book", "status")},
            {"cancel/init", new Tuple<string, string>("cancel", "init")},
            {"cancel/status", new Tuple<string, string>("cancel", "status")}
        };
    }
}
