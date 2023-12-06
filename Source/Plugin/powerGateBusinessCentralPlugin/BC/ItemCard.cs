using Newtonsoft.Json;

namespace powerGateBusinessCentralPlugin.BC
{
    public class ItemCardMin
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
    }

    public class ItemCard
    {
        [JsonProperty("@odata.etag")]
        public string odataetag { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public bool Blocked { get; set; }
        public string Type { get; set; }
        public string Base_Unit_of_Measure { get; set; }
        public string Last_Date_Modified { get; set; }
        public string GTIN { get; set; }
        public string Item_Category_Code { get; set; }
        public string Manufacturer_Code { get; set; }
        public string Service_Item_Group { get; set; }
        public bool Automatic_Ext_Texts { get; set; }
        public string Common_Item_No { get; set; }
        public string Purchasing_Code { get; set; }
        public string VariantMandatoryDefaultYes { get; set; }
        public string Shelf_No { get; set; }
        public bool Created_From_Nonstock_Item { get; set; }
        public string Search_Description { get; set; }
        public int Inventory { get; set; }
        public int Qty_on_Purch_Order { get; set; }
        public int Qty_on_Prod_Order { get; set; }
        public int Qty_on_Component_Lines { get; set; }
        public int Qty_on_Sales_Order { get; set; }
        public int Qty_on_Service_Order { get; set; }
        public int Qty_on_Job_Order { get; set; }
        public int Qty_on_Assembly_Order { get; set; }
        public int Qty_on_Asm_Component { get; set; }
        public string StockoutWarningDefaultYes { get; set; }
        public string PreventNegInventoryDefaultYes { get; set; }
        public double Net_Weight { get; set; }
        public double Gross_Weight { get; set; }
        public double Unit_Volume { get; set; }
        public string SAT_Item_Classification { get; set; }
        public string SAT_Hazardous_Material { get; set; }
        public string SAT_Packaging_Type { get; set; }
        public string Over_Receipt_Code { get; set; }
        public int Trans_Ord_Receipt_Qty { get; set; }
        public int Trans_Ord_Shipment_Qty { get; set; }
        public string Costing_Method { get; set; }
        public double Standard_Cost { get; set; }
        public double Unit_Cost { get; set; }
        public int Indirect_Cost_Percent { get; set; }
        public double Last_Direct_Cost { get; set; }
        public int Net_Invoiced_Qty { get; set; }
        public bool Cost_is_Adjusted { get; set; }
        public bool Cost_is_Posted_to_G_L { get; set; }
        public bool Inventory_Value_Zero { get; set; }
        public string SpecialPurchPriceListTxt { get; set; }
        public string SpecialPurchPricesAndDiscountsTxt { get; set; }
        public string Gen_Prod_Posting_Group { get; set; }
        public string VAT_Prod_Posting_Group { get; set; }
        public string Tax_Group_Code { get; set; }
        public string Inventory_Posting_Group { get; set; }
        public string Default_Deferral_Template_Code { get; set; }
        public string Tariff_No { get; set; }
        public string Country_Region_of_Origin_Code { get; set; }
        public double Unit_Price { get; set; }
        public double CalcUnitPriceExclVAT { get; set; }
        public bool Price_Includes_VAT { get; set; }
        public string Price_Profit_Calculation { get; set; }
        public double Profit_Percent { get; set; }
        public string SpecialSalesPriceListTxt { get; set; }
        public string SpecialPricesAndDiscountsTxt { get; set; }
        public bool Allow_Invoice_Disc { get; set; }
        public string Item_Disc_Group { get; set; }
        public string Sales_Unit_of_Measure { get; set; }
        public bool Sales_Blocked { get; set; }
        public string Application_Wksh_User_ID { get; set; }
        public string VAT_Bus_Posting_Gr_Price { get; set; }
        public string Replenishment_System { get; set; }
        public string Lead_Time_Calculation { get; set; }
        public string Vendor_No { get; set; }
        public string Vendor_Item_No { get; set; }
        public string Purch_Unit_of_Measure { get; set; }
        public bool Purchasing_Blocked { get; set; }
        public string Manufacturing_Policy { get; set; }
        public string Routing_No { get; set; }
        public string Production_BOM_No { get; set; }
        public double Rounding_Precision { get; set; }
        public string Flushing_Method { get; set; }
        public int Overhead_Rate { get; set; }
        public int Scrap_Percent { get; set; }
        public int Lot_Size { get; set; }
        public string Assembly_Policy { get; set; }
        public bool AssemblyBOM { get; set; }
        public string Reordering_Policy { get; set; }
        public string Reserve { get; set; }
        public string Order_Tracking_Policy { get; set; }
        public bool Stockkeeping_Unit_Exists { get; set; }
        public string Dampener_Period { get; set; }
        public int Dampener_Quantity { get; set; }
        public bool Critical { get; set; }
        public string Safety_Lead_Time { get; set; }
        public int Safety_Stock_Quantity { get; set; }
        public bool Include_Inventory { get; set; }
        public string Lot_Accumulation_Period { get; set; }
        public string Rescheduling_Period { get; set; }
        public int Reorder_Point { get; set; }
        public int Reorder_Quantity { get; set; }
        public int Maximum_Inventory { get; set; }
        public int Overflow_Level { get; set; }
        public string Time_Bucket { get; set; }
        public int Minimum_Order_Quantity { get; set; }
        public int Maximum_Order_Quantity { get; set; }
        public int Order_Multiple { get; set; }
        public string Item_Tracking_Code { get; set; }
        public string Serial_Nos { get; set; }
        public string Lot_Nos { get; set; }
        public string Expiration_Calculation { get; set; }
        public string Warehouse_Class_Code { get; set; }
        public string Special_Equipment_Code { get; set; }
        public string Put_away_Template_Code { get; set; }
        public string Put_away_Unit_of_Measure_Code { get; set; }
        public string Phys_Invt_Counting_Period_Code { get; set; }
        public string Last_Phys_Invt_Date { get; set; }
        public string Last_Counting_Period_Update { get; set; }
        public string Next_Counting_Start_Date { get; set; }
        public string Next_Counting_End_Date { get; set; }
        public string Identifier_Code { get; set; }
        public bool Use_Cross_Docking { get; set; }
        public string Global_Dimension_1_Filter { get; set; }
        public string Global_Dimension_2_Filter { get; set; }
        public string Location_Filter { get; set; }
        public string Drop_Shipment_Filter { get; set; }
        public string Variant_Filter { get; set; }
        public string Lot_No_Filter { get; set; }
        public string Serial_No_Filter { get; set; }
        public string Unit_of_Measure_Filter { get; set; }
        public string Package_No_Filter { get; set; }
        public string Date_Filter { get; set; }
    }
}
