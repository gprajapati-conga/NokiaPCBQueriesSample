using Apttus.Lightsaber.Extensibility.Framework.Library;
using Apttus.Lightsaber.Extensibility.Framework.Library.Interfaces;
using Apttus.Lightsaber.Nokia.Pricing;
using Apttus.Lightsaber.Pricing.Common.Callback;
using Apttus.Lightsaber.Pricing.Common.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apttus.Lightsaber.Nokia.Common;

namespace NokiaPCBQueriesSample
{
    public class NokiaPCBQuery : CodeExtensibility, IPricingBasePriceCallback
    {
        private IDBHelper dBHelper = null;

        ILogHelper Logger
        {
            get
            {
                return GetLogHelper();
            }
        }

        public async Task AfterPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }

        public async Task BeforePricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            dBHelper = GetDBHelper();

            //var defaultExchangeRateQuery = QueryHelper.GetDefaultExchangeRateQuery("USD");
            //decimal? defaultExchangeRate = (await dBHelper.FindAsync<CurrencyTypeQueryModel>(defaultExchangeRateQuery)).FirstOrDefault()?.ConversionRate;

            //List<string> productList = new List<string>() { "01t41000005IQySAAW", "01t41000005IR0FAAW", "01t41000005IR1XAAW", "01t41000005K1kNAAS" };
            //var countryPriceListItemQuery = QueryHelper.GetCountryPriceListItemQuery(productList);
            //var countryPriceListItems = await dBHelper.FindAsync<CountryPriceListItemQueryModel>(countryPriceListItemQuery);

            //HashSet<string> ProductId_set = new HashSet<string>() { "01t41000005JyBEAA0", "01t41000005JyBPAA0", "01t41000005JyBYAA0" };
            //var productExtensionsQuery = QueryHelper.GetProductExtensionsQuery(ProductId_set, "EUR");
            //List<ProductExtensionsQueryModel> Prod_extList = await dBHelper.FindAsync<ProductExtensionsQueryModel>(productExtensionsQuery);

            //var maintenanceAndSSPRuleQuery = QueryHelper.GetMaintenanceAndSSPRuleQuery("RG_NAM", "Gold & Advanced Exchange Next Business Day");
            //var maintenanceSSPRuleList = await dBHelper.FindAsync<MaintenanceAndSSPRuleQueryModel>(maintenanceAndSSPRuleQuery);

            var proposal = new Proposal(new Dictionary<string, object>()
            {
                { "NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c", "Europe" },
                { "NokiaCPQ_IsPMA__c", false },
                { "NokiaCPQ_LEO_Discount__c", false },
                { "NokiaCPQ_Maintenance_Type__c", "Gold (Return for Exchange)"},
                { "NokiaCPQ_Maintenance_Level__c", string.Empty },
                { "Apttus_Proposal__Account__r.Partner_Program__c", "GPP 3.0" }
            });

            var nokiaMaintenanceAndSSPRulesQuery = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
            var nokiaMaintenanceSSPRules = await dBHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery);

            Console.WriteLine($"Count:{nokiaMaintenanceSSPRules.Count}");

            proposal = new Proposal(new Dictionary<string, object>()
            {
                { "NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c", "Europe" },
                { "NokiaCPQ_IsPMA__c", true },
                { "NokiaCPQ_LEO_Discount__c", true },
                { "NokiaCPQ_Maintenance_Type__c", "Gold (Return for Exchange)"},
                { "NokiaCPQ_Maintenance_Level__c", "Nokia Brand of Service AED" },
                { "Apttus_Proposal__Account__r.Partner_Program__c", "GPP 3.0" }
            });
            var nokiaMaintenanceAndSSPRulesQuery2 = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
            var nokiaMaintenanceSSPRules2 = await dBHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery2);

            Console.WriteLine($"Count2:{nokiaMaintenanceSSPRules2.Count}");

            proposal = new Proposal(new Dictionary<string, object>()
            {
                { "NokiaCPQ_Maintenance_Accreditation__r.Pricing_Cluster__c", "Europe" },
                { "NokiaCPQ_IsPMA__c", true },
                { "NokiaCPQ_LEO_Discount__c", false },
                { "NokiaCPQ_Maintenance_Type__c", "Gold (Return for Exchange)"},
                { "NokiaCPQ_Maintenance_Level__c", "Nokia Brand of Service AED" },
                { "Apttus_Proposal__Account__r.Partner_Program__c", "GPP 3.0" }
            });
            var nokiaMaintenanceAndSSPRulesQuery3 = QueryHelper.GetNokiaMaintenanceAndSSPRulesQuery(proposal);
            var nokiaMaintenanceSSPRules3 = await dBHelper.FindAsync<NokiaMaintenanceAndSSPRulesQueryModel>(nokiaMaintenanceAndSSPRulesQuery3);

            Console.WriteLine($"Count3:{nokiaMaintenanceSSPRules3.Count}");

            //var tierDiscountDetailQuery = QueryHelper.GetTierDiscountDetailQuery(partnerProgram: "GPP 3.0", partnerType: "Value Added Reseller");
            //var tierDiscountDetailQueryModels = await dBHelper.FindAsync<TierDiscountDetailQueryModel>(tierDiscountDetailQuery);

            //var indirectMarketPriceListQuery = QueryHelper.GetIndirectMarketPriceListQuery();
            //var priceListCollection = await dBHelper.FindAsync<PriceListQueryModel>(indirectMarketPriceListQuery);

            //var pricingGuidanceSettingQuery = QueryHelper.GetPricingGuidanceSettingQuery("QTC");
            //var pricingGuidanceSettingThresold = (await dBHelper.FindAsync<PricingGuidanceSettingQueryModel>(pricingGuidanceSettingQuery)).FirstOrDefault()?.Threshold__c;

            //var shippingLocationQuery = QueryHelper.GetShippingLocationForDirectQuoteQuery("Fixed Access - FBA", null);
            //var shippingLocations = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

            //var shippingLocationQuery2 = QueryHelper.GetShippingLocationForIndirectQuoteQuery("Fixed Access - FBA", "Europe Southern");
            //var shippingLocations2 = await dBHelper.FindAsync<ShippingLocationQueryModel>(shippingLocationQuery);

            //var productDisc = await QueryHelper.ExecuteProductDiscountQuery(dBHelper, "Market Europe", new List<string>() { "HW" });

            //var mnDirectProductMapQuery = QueryHelper.GetMNDirectProductMapQuery();
            //var MN_Direct_Products_List = await dBHelper.FindAsync<MNDirectProductMapQueryModel>(mnDirectProductMapQuery);

            //var directPortfolioGeneralSettingQuery = QueryHelper.GetDirectPortfolioGeneralSettingQuery("IP Routing");
            //var portfolioSettingList = await dBHelper.FindAsync<DirectPortfolioGeneralSettingQueryModel>(directPortfolioGeneralSettingQuery);

            //var directCareCostPercentageQuery = QueryHelper.GetDirectCareCostPercentageQuery("Market Europe");
            //var careCostPercentList = await dBHelper.FindAsync<DirectCareCostPercentageQueryModel>(directCareCostPercentageQuery);

            //var sspSRSDefaultValuesQuery = QueryHelper.GetSSPSRSDefaultValuesQuery("IP Routing");
            //var sspSRSDefaultsList = await dBHelper.FindAsync<SSPSRSDefaultValuesQueryModel>(sspSRSDefaultValuesQuery);

            //Logger.LogDebug(JsonConvert.SerializeObject(dBHelper.GetDBStatistics()));
            Console.WriteLine(JsonConvert.SerializeObject(dBHelper.GetDBStatistics()));
        }

        public async Task OnPricingBatchAsync(BatchPriceRequest batchPriceRequest)
        {
            await Task.CompletedTask;
        }
    }
}
