using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using powerGateBusinessCentralPlugin.BC;
using powerGateBusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace powerGateBusinessCentralPlugin
{
    [DataServiceKey(nameof(Number))]
    [DataServiceEntity]
    public class BomHeader
    {
        public string Number { get; set; }
        public Item Item { get; set; }
        public List<BomRow> Children { get; set; }

        public BomHeader()
        {
            Children = new List<BomRow>();
        }
    }

    public class BomHeaders : ServiceMethod<BomHeader>
    {
        public override IEnumerable<BomHeader> Query(IExpression<BomHeader> expression)
        {
            if (expression.IsSimpleWhereToken())
            {
                var number = (string)expression.GetWhereValueByName(nameof(BomHeader.Number));

                var bomRow = BusinessCentralApi.Instance.GetBomHeaderAndRows(number);
                if (bomRow == null)
                    return new List<BomHeader>();

                var entity = new BomHeader
                {
                    Number = bomRow.No,
                    Item = Items.GetItemByNumber(bomRow.No),
                    Children = new List<BomRow>()
                };
                foreach (var line in bomRow.ProductionBOMsProdBOMLine)
                {
                    var row = new BomRow
                    {
                        ParentNumber = line.Production_BOM_No,
                        ChildNumber = line.No,
                        Position = line.Line_No,
                        Quantity = (double)line.Quantity_per,
                        Item = Items.GetItemByNumber(line.No),
                        IsRawMaterial = line.Routing_Link_Code == "RAW"
                    };

                    entity.Children.Add(row);
                }

                return new List<BomHeader> { entity };
            }

            throw new NotSupportedException();
        }

        public override void Update(BomHeader entity)
        {
            var item = BusinessCentralApi.Instance.GetItemCard(entity.Number);
            var bomHeader = new ProductionBOM
            {
                No = entity.Number,
                Description = item.Description,
                Unit_of_Measure_Code = item.Base_Unit_of_Measure
            };
            BusinessCentralApi.Instance.UpdateBomHeader(bomHeader);
            BusinessCentralApi.Instance.UpdateItemCardProductionBom(item.No);
        }

        public override void Create(BomHeader entity)
        {
            var item = BusinessCentralApi.Instance.GetItemCard(entity.Number);
            var bomHeader = new ProductionBOM
            {
                No = entity.Number,
                Description = item.Description,
                Unit_of_Measure_Code = item.Base_Unit_of_Measure
            };
            BusinessCentralApi.Instance.CreateBomHeader(bomHeader);
            BusinessCentralApi.Instance.UpdateItemCardProductionBom(item.No);
        }

        public override void Delete(BomHeader entity)
        {
            throw new NotSupportedException();
        }
    }
}