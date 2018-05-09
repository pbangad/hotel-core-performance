using Oski.Hotel;
using PerformanceTests.TestLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tavisca.Automation.Utilities;
using Tavisca.Platform.Common.Logging;
using Tavisca.Platform.Common.Plugins.Json;
using TestRunner;

namespace TestLibraryHotel
{
    [Test("new_hotel_search", "booking test for HBE")]

    public class BESearchTest : Test
    {
        public const int checkInAfterNumberOfDays = 10, stayPeriod = 14;
        protected HotelEngineFacade engineFacade;
        protected PosConfig PosConfig;
        private static string _defaultEnvironment;
        private static string _defaultTenant;
        protected static string _POSID;
        protected static string _URL;
        private HotelEngine engine;
        private Oski.Hotel.Environment env;
        public Hotel_Sdk.Internals.ILogger logger;

        public BESearchTest()
        {
            LoggerInitialization.RunOnce();
            _defaultEnvironment = ConfigurationManager.AppSettings["HotelEnvironment"];
            _defaultTenant = ConfigurationManager.AppSettings["HotelTenant"];
            _POSID = ConfigurationManager.AppSettings["HotelPOSID"];
            _URL = ConfigurationManager.AppSettings["HotelURL"];
            env = new Oski.Hotel.Environment { BaseUrl = _URL, TenantId = _defaultTenant };
            logger = new PerformanceTests.TestLogger.MyHotelLogger();
        }
        public override async Task ExecuteAsync(TestRunner.TestSettings settings, CancellationToken cancellationToken)
        {
            int iterations = 20;
            int pageSize = 200;
            bool resultLoop = false;
            bool debug = true;
            if (settings.Parameters.TryGetValue("pgSize",out string pgSize))
            {
                pageSize = int.Parse(pgSize);
                if (pageSize <10)
                {
                    pageSize = 10;
                }
            }
            if (settings.Parameters.TryGetValue("multiResult",out string multiResult))
            {
                resultLoop = bool.Parse(multiResult);
            }

            if (settings.Parameters.TryGetValue("debug",out string debugTrue))
            {
                debug = bool.Parse(debugTrue);
            }
            env.DisableDebugging = !debug;

            var stayDates = DateTimeHelper.GetStartAndEndDates(checkInAfterNumberOfDays, stayPeriod);

            var TRsStatus = new GetSearchStatusRs();
            var TRsResult = new GetSearchResultsRs();
            var TRsRoom = new GetRoomSearchResultsRs();
            var TRsPrice = new PriceRs();
            var TRsInitBook = new InitBookRs();

            var CorrelationId = Guid.NewGuid().ToString() + "-" + settings.RunID;
            
            engine = new HotelEngine(env, (Hotel_Sdk.Internals.ILogger)logger);

            //Hotel Search
            var timer = Stopwatch.StartNew();

            var reqInit =  engine.InitSearch()
                //.WithCurrency("USD")
                .WithPointOfSale(_POSID)//.WithCountryFilter("US")
                .WithRoomOccupancy(new Occupant() {Type= "adult", Age=25 })
                .WithCircleRegion(SearchCriteria.Paris.Circle.Center, SearchCriteria.Paris.Circle.RadiusInKm)
                //.WithCircleRegion(new GeoCode() { Latitude = 36.1699, Longitude = 115.1398 },50.5)
                .WithStayPeriod(stayDates[0], stayDates[1]);
            var TRsInit = await reqInit.ExecuteAsync(CorrelationId);
            
            if (TRsInit.StatusCode==System.Net.HttpStatusCode.OK)
            {
                    var reqStatus = engine.GetSearchStatus().WithSessionId(TRsInit.SessionId);
                    TRsStatus = await reqStatus.GetCompleteSearchStatus(CorrelationId, 200);
            if (TRsStatus.StatusCode==System.Net.HttpStatusCode.OK && TRsStatus.ErrorInfo == null)
            {
                    int pageCount = (TRsStatus.HotelCount / pageSize) +(TRsStatus.HotelCount % pageSize > 0 ? 1:0);
                    for (int iterator = 0; iterator < pageCount; iterator++)
                    {
                        var reqResultAll = engine.GetSearchResults()
                            .WithSessionId(TRsInit.SessionId)
                            .WithPaging(new Paging() { PageNo = iterator + 1, PageSize = pageSize, OrderBy = "price asc, rating desc" })
                            .WithOptionalData()
                            .WithContentPreference(ContentPreference.Basic);
                        TRsResult = await reqResultAll.ExecuteAsync(CorrelationId);
                        if (!resultLoop) break;
                    }
            }
            }

            timer.Stop();
            double TimeTakenInMS = timer.ElapsedMilliseconds;

            await PushToELK(settings, TRsResult, TimeTakenInMS);
            System.Console.WriteLine("CorID: " + TRsResult.CorrelationId + " TimeTaken: " + TimeTakenInMS);
        }
        private static async Task PushToELK(TestRunner.TestSettings settings, Oski.Hotel.GetSearchResultsRs searchResults, double TimeTakenInMS)
        {
            var apilog = new ApiLog()
            {
                Response = new Payload(ByteHelper.ToByteArrayUsingJsonSerialization(searchResults)),
                Request = null,
                TimeTakenInMs = TimeTakenInMS,
                CorrelationId = searchResults.CorrelationId,
                IsSuccessful = true,
                ApplicationName = "be_perf_hotel",
                Verb = "search",
                Api = "search"
            };
            apilog.SetValue("method_name", "_search");
            apilog.SetValue("session_id", searchResults.SessionId);
            apilog.SetValue("run_id", settings.RunID);
            apilog.SetValue("counter", 1);

            foreach (string item in searchResults.Headers.Keys)
            {
                apilog.ResponseHeaders.Add(item, searchResults.Headers[item]);
            }
            LogTime.LogTimeMapping.TryAdd(apilog.CorrelationId, TimeTakenInMS);
            await Logger.WriteLogAsync(apilog);
        }
    }
}
