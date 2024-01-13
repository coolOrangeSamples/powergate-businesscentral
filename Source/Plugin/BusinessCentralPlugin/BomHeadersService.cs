using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Threading.Tasks;
using BusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace BusinessCentralPlugin
{
    using ProdBOMLine = BusinessCentral.ProdBOMLine;
    using ProductionBOM = BusinessCentral.ProductionBOM;

    [DataServiceKey(nameof(Number))]
    [DataServiceEntity]
    public class BomHeader
    {
        public string Number { get; set; }
        public Item Item { get; set; }
        public List<BomRow> Children { get; set; } = new List<BomRow>();
    }

    public class BomHeaders : ServiceMethod<BomHeader>
    {
        public override IEnumerable<BomHeader> Query(IExpression<BomHeader> expression)
        {
            if (expression.IsSimpleWhereToken())
            {
                var number = (string)expression.GetWhereValueByName(nameof(BomHeader.Number));

                var bomRowTask = BusinessCentralApi.Instance.GetBomHeaderAndRows(number);
                var itemTask = Items.GetItemByNumberAsync(number);
                Task.WaitAll(bomRowTask, itemTask);

                var bomRow = bomRowTask.Result;
                if (bomRow == null)
                    return new List<BomHeader>();

                var entity = new BomHeader
                {
                    Number = bomRow.No,
                    Item = itemTask.Result,
                    Children = new List<BomRow>()
                };

                var bag = new ConcurrentBag<BomRow>();
                async void BomRowCreation(ProdBOMLine line)
                {
                    var row = new BomRow
                    {
                        ParentNumber = line.Production_BOM_No,
                        ChildNumber = line.No,
                        Position = line.Line_No,
                        Quantity = (double)line.Quantity_per,
                        Item = await Items.GetItemByNumberAsync(line.No),
                        IsRawMaterial = line.Routing_Link_Code == Configuration.RoutingLinkRawMaterial
                    };
                    bag.Add(row);
                }
                Parallel.ForEach(bomRow.ProductionBOMsProdBOMLine, BomRowCreation);
                entity.Children.AddRange(bag.ToArray());

                return new List<BomHeader> { entity };
            }

            throw new NotSupportedException();
        }

        public override async void Update(BomHeader entity)
        {
            var item = await BusinessCentralApi.Instance.GetItemCard(entity.Number);
            var bomHeader = new ProductionBOM
            {
                No = entity.Number,
                Description = item.Description,
                Unit_of_Measure_Code = item.Base_Unit_of_Measure
            };
            var tasks = new List<Task>
            {
                BusinessCentralApi.Instance.UpdateBomHeader(bomHeader),
                BusinessCentralApi.Instance.UpdateItemCardProductionBom(item.No)
            };
            Task.WaitAll(tasks.ToArray());
        }

        public override async void Create(BomHeader entity)
        {
            var item = await BusinessCentralApi.Instance.GetItemCard(entity.Number);
            var bomHeader = new ProductionBOM
            {
                No = entity.Number,
                Description = item.Description,
                Unit_of_Measure_Code = item.Base_Unit_of_Measure
            };
            var tasks = new List<Task>
            {
                BusinessCentralApi.Instance.CreateBomHeader(bomHeader),
                BusinessCentralApi.Instance.UpdateItemCardProductionBom(item.No)
            };
            Task.WaitAll(tasks.ToArray());
        }

        public override void Delete(BomHeader entity)
        {
            throw new NotSupportedException();
        }
    }
}