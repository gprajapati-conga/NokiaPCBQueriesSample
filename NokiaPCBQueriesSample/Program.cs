﻿using Apttus.Lightsaber.Extensibility.Framework.Common.Interface;
using Apttus.Lightsaber.Extensibility.Framework.Common.Model;
using Apttus.Lightsaber.Extensibility.Framework.Library.Implementation;
using Apttus.Lightsaber.MongoProvider.Helpers;
using Apttus.Lightsaber.MongoProvider.Implementation;
using Apttus.Lightsaber.MongoProvider.MongoDbOperations;
using Apttus.Lightsaber.Nokia.Pricing;
using Apttus.OpenTracingTelemetry;
using Apttus.OpenTracingTelemetry.Extensions;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Apttus.Lightsaber.Pricing.Common.Models;
using Apttus.Lightsaber.Pricing.Common.Entities;
using System.Linq;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Apttus.Lightsaber.Pricing.Common.Implementation;
using Apttus.Lightsaber.Nokia.Common;
using Apttus.Lightsaber.Pricing.Common.Messages.Cart;

namespace NokiaPCBQueriesSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string quoteType = "Chandresh";
            string portfolio = "POSTTest";

            IApttusOpenTracer apttusOpenTracer = SetupTestTracer();
            var mongoOperation = GetMongoProvider(apttusOpenTracer);

            var propJson = File.ReadAllText(@$"Data\{quoteType}\Proposal.json");
            Proposal propObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Proposal>(propJson);

            var lineItemsJson = File.ReadAllText(@$"Data\{quoteType}\207_AddlineItem.json");
            var lineItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Apttus.Lightsaber.Pricing.Common.Entities.LineItem>>(lineItemsJson);

            //var propJson = File.ReadAllText(@$"Data\{quoteType}\{portfolio}\Proposal.json");
            //Proposal propObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Proposal>(propJson);

            //var lineItemsJson = File.ReadAllText(@$"Data\{quoteType}\{portfolio}\LineItems.json");
            //var lineItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Apttus.Lightsaber.Pricing.Common.Entities.LineItem>>(lineItemsJson);

            // Generate batch files

            //List<IEnumerable<Apttus.Lightsaber.Pricing.Common.Entities.LineItem>> lineItemsGroup = 
            //    new List<IEnumerable<Apttus.Lightsaber.Pricing.Common.Entities.LineItem>>();
            //var batchSize = 50;
            //var skip = 0;

            //while (skip < lineItems.Count)
            //{
            //    var lItems = lineItems.Skip(skip).Take(batchSize);
            //    skip = skip + lItems.Count();

            //    lineItemsGroup.Add(lItems);
            //}

            //List<List<Apttus.Lightsaber.Pricing.Common.Entities.LineItem>> lineItemsGroup = SplitIntoChunks(lineItems, 50);

            //int index = 1;
            //foreach(var lineItemsg in lineItemsGroup)
            //{
            //    File.WriteAllText(@$"Data\Output\Batch-{index}.json", Newtonsoft.Json.JsonConvert.SerializeObject(lineItemsg));
            //    index++;
            //}  

            var lineItemsModel = lineItems.Select(li => new LineItemModel(li, new PriceListItemModel(li.PriceListItem))).ToList();
            var numbers = Enumerable.Range(1, lineItemsModel.Count);
            List<ProductLineItemModel> productLineItemModels = new List<ProductLineItemModel>();

            foreach (var num in numbers)
            {
                productLineItemModels.Add(new ProductLineItemModel() { PrimaryLineNumber = num, ChargeLines = new List<LineItemModel>() { lineItemsModel[num - 1] } });
            }

            var batchPriceRequest = new BatchPriceRequest { Cart = new ProductConfigurationModel(new ProductConfiguration()), CartId = "cart1", LineItems = productLineItemModels };
            batchPriceRequest.Cart.Set(Apttus.Lightsaber.Nokia.Common.Constants.PROPOSAL_CONFIG_RELATIONSHIP_NAME, propObject);

            batchPriceRequest.CartContext = new CartContext();
            batchPriceRequest.CartContext.LineItems = productLineItemModels;

            AggregateCartRequest cartRequest = new AggregateCartRequest();
            cartRequest.Cart = batchPriceRequest.Cart;
            cartRequest.CartContext = batchPriceRequest.CartContext;
            cartRequest.CartId = "cart1";

            IExtensibilityDBOperation extensibilityDB = new MongoDBOperation(mongoOperation);
            IExtensibilityLogger extensibilityLogger = new RuntimeOpenTracer(apttusOpenTracer);

            var roundingService = new RoundingService();

            CEFContext cefContext = new CEFContext("", extensibilityDB, extensibilityLogger, null, null, roundingService, new LineItemService(roundingService));

            //Console.WriteLine("Iteration-1:");

            //NokiaPCBQuery nokiaPCBQueryInstance = new NokiaPCBQuery();
            //Type type = typeof(NokiaPCBQuery);
            //MethodInfo mi = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            //mi.Invoke(nokiaPCBQueryInstance, new object[] { cefContext, "NokiaPCBQueriesSample.NokiaPCBQuery" });

            //nokiaPCBQueryInstance.BeforePricingBatchAsync(null).Wait();

            //Console.WriteLine("Iteration-2:");

            //nokiaPCBQueryInstance = new NokiaPCBQuery();
            //type = typeof(NokiaPCBQuery);
            //mi = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            //mi.Invoke(nokiaPCBQueryInstance, new object[] { cefContext, "NokiaPCBQueriesSample.NokiaPCBQuery" });

            //nokiaPCBQueryInstance.BeforePricingBatchAsync(null).Wait();

            //Console.WriteLine("Iteration-3:");

            //nokiaPCBQueryInstance = new NokiaPCBQuery();
            //type = typeof(NokiaPCBQuery);
            //mi = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            //mi.Invoke(nokiaPCBQueryInstance, new object[] { cefContext, "NokiaPCBQueriesSample.NokiaPCBQuery" });

            //nokiaPCBQueryInstance.BeforePricingBatchAsync(null).Wait();


            NokiaBasePriceCallback nokiaBasePriceCallbackInstance = new NokiaBasePriceCallback();
            Type type = typeof(NokiaBasePriceCallback);
            MethodInfo mi = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(nokiaBasePriceCallbackInstance, new object[] { cefContext, "Apttus.Lightsaber.Nokia.Pricing.NokiaBasePriceCallback" });

            nokiaBasePriceCallbackInstance.BeforePricingBatchAsync(batchPriceRequest).Wait();
            nokiaBasePriceCallbackInstance.OnPricingBatchAsync(batchPriceRequest).Wait();
            nokiaBasePriceCallbackInstance.AfterPricingBatchAsync(batchPriceRequest).Wait();

            NokiaTotallingCallback nokiaTotallingCallbackInstance = new NokiaTotallingCallback();
            type = typeof(NokiaTotallingCallback);
            mi = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(nokiaTotallingCallbackInstance, new object[] { cefContext, "Apttus.Lightsaber.Nokia.Pricing.NokiaTotallingCallback" });

            nokiaTotallingCallbackInstance.BeforePricingCartAdjustmentAsync(cartRequest).Wait();
            nokiaTotallingCallbackInstance.AfterPricingCartAdjustmentAsync(cartRequest).Wait();
            nokiaTotallingCallbackInstance.OnCartPricingCompleteAsync(cartRequest).Wait();

            Console.WriteLine("Completed!");
            Console.ReadLine();
        }

        static IMongoOperation GetMongoProvider(IApttusOpenTracer apttusOpenTracer)
        {
            string MongoDbConnectionString = "mongodb://127.0.0.1:27017";
            //string MongoDbConnectionString = "mongodb://admin:wtr7joumsn3pvquecudu@c45def85-bb36-4063-86b5-92dab3499dc4-0.br37s45d0p54n73ffbr0.databases.appdomain.cloud:31921,c45def85-bb36-4063-86b5-92dab3499dc4-1.br37s45d0p54n73ffbr0.databases.appdomain.cloud:31921/ibmclouddb?authSource=admin&replicaSet=replset&ssl=true&sslCAFile=48b1ff99-7b37-4370-b9e6-229f4b939777";
            string MongoDbName = "nokia-lsdb";
            IBsonHelper bsonHelper = new BsonHelper();
            var client = new MongoClient(MongoDbConnectionString);
            var db = client.GetDatabase(MongoDbName);
            return new MongoOperation(db, bsonHelper, apttusOpenTracer);
        }

        static IApttusOpenTracer SetupTestTracer()
        {
            var openTracerType = OpenTracerType.Jaeger;
            var isTracingEnable = true;
            var isLoggingEnable = true;
            var minLogLevel = LogLevel.Trace;
            var iTracer = OpenTracingExtensions.SetupOpenTracing("DUMMY", openTracerType,
                isTracingEnable, isLoggingEnable, minLogLevel);
            return iTracer;
        }

        public static List<List<T>> SplitIntoChunks<T>(List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("Chunk size must be greater than 0");
            }
            List<List<T>> retVal = new List<List<T>>();
            int index = 0;
            while (index < list.Count)
            {
                int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                retVal.Add(list.GetRange(index, count));
                index += chunkSize;
            }
            return retVal;
        }
    }
}
