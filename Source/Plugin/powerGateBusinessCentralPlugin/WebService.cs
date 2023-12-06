using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using powerGateBusinessCentralPlugin.BC;
using powerGateBusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace powerGateBusinessCentralPlugin
{
    [WebServiceData("coolOrange", "BusinessCentral")]
    public class WebService : powerGateServer.SDK.WebService
    {
        public WebService()
        {
            AddMethod(new Items());
            AddMethod(new BomHeaders());
            AddMethod(new BomRows());
            AddMethod(new Documents());
        }

        static WebService()
        {
            ReadConfiguration();
        }

        private static void ReadConfiguration()
        {
            var configFullName = Assembly.GetExecutingAssembly().Location + ".config";
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFullName };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var loginSection = configuration.GetSection("BusinessCentral.Login") as AppSettingsSection;
            if (loginSection == null)
                throw new ConfigurationErrorsException(
                    "BusinessCentral.Login section is missing in powerGateBusinessCentralPlugin.dll.config");

            Config.BaseUrl = loginSection.Settings["BaseUrl"].Value;
            Config.Company = loginSection.Settings["Company"].Value;
            Config.Username = loginSection.Settings["Username"].Value;
            Config.Password = loginSection.Settings["Password"].Value;

            var settingsSection = configuration.GetSection("BusinessCentral.Settings") as AppSettingsSection;
            if (settingsSection == null)
                throw new ConfigurationErrorsException(
                    "BusinessCentral.Settings section is missing in powerGateBusinessCentralPlugin.dll.config");

            Config.DefaultItemType = settingsSection.Settings["Default_Item_Type"].Value;
            Config.DefaultItemCategoryCode = settingsSection.Settings["Default_Item_Category_Code"].Value;
            Config.DefaultInventoryPostingGroup = settingsSection.Settings["Default_Inventory_Posting_Group"].Value;
            Config.DefaultGeneralProductPostingGroup = settingsSection.Settings["Default_General_Product_Posting_Group"].Value;
            Config.GeneralProductPostingGroupMakeIndicator = settingsSection.Settings["General_Product_Posting_Group_Make_Indicator"].Value;
            Config.ItemAttributeDescription = settingsSection.Settings["Item_Attribute_Description"].Value;
            Config.ItemAttributeMaterial = settingsSection.Settings["Item_Attribute_Material"].Value;
            Config.ItemLinkThinClient = settingsSection.Settings["Item_Link_ThinClient"].Value;
            Config.ItemLinkThickClient = settingsSection.Settings["Item_Link_ThickClient"].Value;
        }

        public static List<Lookup> UnitsOfMeasures;
        public static List<Vendor> Vendors;

        public static void InitializeBusinessCentral()
        {
            UnitsOfMeasures = BusinessCentralApi.Instance.GetUnitsOfMeasures();
            Vendors = BusinessCentralApi.Instance.GetVendors();


            var itemAttributes = BusinessCentralApi.Instance.GetItemAttributesDefinitions();
            if (!itemAttributes.Any(i => i.Name.Equals(Config.ItemAttributeDescription)))
                throw new ConfigurationErrorsException($"Item_Attribute_Description cannot be '{Config.ItemAttributeDescription}'");

            if (string.IsNullOrEmpty(Config.ItemAttributeDescription))
                throw new ConfigurationErrorsException($"Item_Attribute_Description cannot be '{Config.ItemAttributeDescription}'");

            if (!itemAttributes.Any(i => i.Name.Equals(Config.ItemAttributeMaterial)))
                throw new ConfigurationErrorsException($"Item_Attribute_Description cannot be '{Config.ItemAttributeMaterial}'");

            if (string.IsNullOrEmpty(Config.ItemAttributeMaterial))
                throw new ConfigurationErrorsException($"Item_Attribute_Material cannot be '{Config.ItemAttributeMaterial }'");

            if (string.IsNullOrEmpty(Config.ItemLinkThinClient))
                throw new ConfigurationErrorsException($"Item_Link_ThinClient cannot be '{Config.ItemLinkThinClient}'");

            if (string.IsNullOrEmpty(Config.ItemLinkThickClient))
                throw new ConfigurationErrorsException($"Item_Link_ThickClient cannot be '{Config.ItemLinkThickClient}'");

            if (!new[] { "Inventory", "Service", "Non-Inventory"}.Contains(Config.DefaultItemType))
                throw new ConfigurationErrorsException($"Default_Item_Type cannot be '{Config.DefaultItemType}'");

            var itemCategories = BusinessCentralApi.Instance.GetItemCategories();
            if (!itemCategories.Any(i => i.code.Equals(Config.DefaultItemCategoryCode)))
                throw new ConfigurationErrorsException($"Default_Item_Category_Code cannot be '{Config.DefaultItemCategoryCode}'");

            var inventoryPostingGroups = BusinessCentralApi.Instance.GetInventoryPostingGroups();
            if (!inventoryPostingGroups.Any(i => i.code.Equals(Config.DefaultInventoryPostingGroup)))
                throw new ConfigurationErrorsException($"Default_Inventory_Posting_Group cannot be '{Config.DefaultInventoryPostingGroup}'");

            var generalProductPostingGroups = BusinessCentralApi.Instance.GetGeneralProductPostingGroups();
            if (!generalProductPostingGroups.Any(i => i.code.Equals(Config.DefaultGeneralProductPostingGroup)))
                throw new ConfigurationErrorsException($"Default_General_Product_Posting_Group cannot be '{Config.DefaultGeneralProductPostingGroup}'");

            if (!generalProductPostingGroups.Any(i => i.code.Equals(Config.GeneralProductPostingGroupMakeIndicator)))
                throw new ConfigurationErrorsException($"General_Product_Posting_Group_Make_Indicator cannot be '{Config.GeneralProductPostingGroupMakeIndicator}'");
        }

        public static class Config
        {
            public static string BaseUrl;
            public static string Company;
            public static string Username;
            public static string Password;
            public static string DefaultItemType;
            public static string DefaultItemCategoryCode;
            public static string DefaultInventoryPostingGroup;
            public static string DefaultGeneralProductPostingGroup;
            public static string GeneralProductPostingGroupMakeIndicator;
            public static string ItemAttributeDescription;
            public static string ItemAttributeMaterial;
            public static string ItemLinkThinClient;
            public static string ItemLinkThickClient;
        }
    }
}