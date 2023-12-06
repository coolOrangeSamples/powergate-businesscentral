using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using powerGateBusinessCentralPlugin.BC;
using powerGateBusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace powerGateBusinessCentralPlugin
{
    [DataServiceKey(nameof(ParentNumber), nameof(Position), nameof(ChildNumber))]
    [DataServiceEntity]
    public class BomRow
    {
        public string ParentNumber { get; set; }
        public string ChildNumber { get; set; }
        public int Position { get; set; }
        public double Quantity { get; set; }
        public bool IsRawMaterial { get; set; }
        public Item Item { get; set; }
    }

    public class BomRows : ServiceMethod<BomRow>
    {
        public override IEnumerable<BomRow> Query(IExpression<BomRow> expression)
        {
            if (expression.IsSimpleWhereToken())
            {
                var parentNumber = (string)expression.GetWhereValueByName(nameof(BomRow.ParentNumber));
                var position = (int)expression.GetWhereValueByName(nameof(BomRow.Position));
                var childNumber = (string)expression.GetWhereValueByName(nameof(BomRow.ChildNumber));

                var bomRow = BusinessCentralApi.Instance.GetBomRow(parentNumber, position, childNumber);
                if (bomRow == null)
                    return new List<BomRow>();

                var entity = new BomRow
                {
                    ParentNumber = bomRow.Production_BOM_No,
                    ChildNumber = bomRow.No,
                    Position = bomRow.Line_No,
                    Quantity = (double)bomRow.Quantity_per,
                    Item = Items.GetItemByNumber(bomRow.No),
                    IsRawMaterial = bomRow.Routing_Link_Code == "RAW"
                };

                return new List<BomRow> { entity };
            }

            throw new NotSupportedException();
        }

        public override void Update(BomRow entity)
        {
            var item = BusinessCentralApi.Instance.GetItemCardMin(entity.ChildNumber);
            var bomRow = new ProdBOMLine
            {
                Production_BOM_No = entity.ParentNumber,
                Line_No = entity.Position,
                No = entity.ChildNumber,
                Description = item.Description,
                Quantity_per = (decimal)entity.Quantity,
                Routing_Link_Code = entity.IsRawMaterial ? "RAW" : null
            };
            BusinessCentralApi.Instance.UpdateBomRow(bomRow);
        }

        public override void Create(BomRow entity)
        {
            var item = BusinessCentralApi.Instance.GetItemCardMin(entity.ChildNumber);
            var bomRow = new ProdBOMLine
            {
                Production_BOM_No = entity.ParentNumber,
                Line_No = entity.Position,
                No = entity.ChildNumber,
                Description = item.Description,
                Quantity_per = (decimal)entity.Quantity,
                Routing_Link_Code = entity.IsRawMaterial ? "RAW" : null
            };
            BusinessCentralApi.Instance.CreateBomRow(bomRow);
        }

        public override void Delete(BomRow entity)
        {
            var bomRow = new ProdBOMLine
            {
                Production_BOM_No = entity.ParentNumber,
                Line_No = entity.Position,
                No = entity.ChildNumber
            };
            BusinessCentralApi.Instance.DeleteBomRow(bomRow);
        }
    }
}