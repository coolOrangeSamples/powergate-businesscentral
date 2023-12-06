using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using powerGateBusinessCentralPlugin.BC;
using powerGateBusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace powerGateBusinessCentralPlugin
{
    [DataServiceKey(nameof(Number))]
    [DataServiceEntity]
    public class Item
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public double Weight { get; set; }
        public string Material { get; set; }
        public double Price { get; set; } // readonly
        public int Stock { get; set; } // readonly
        public bool MakeBuy { get; set; } // readonly
        public bool Blocked { get; set; } // readonly
        public string Supplier { get; set; } // readonly
        public string Thumbnail { get; set; }
        public string ThinClientLink { get; set; }
        public string ThickClientLink { get; set; }
    }

    public class Items : ServiceMethod<Item>
    {
        public override IEnumerable<Item> Query(IExpression<Item> expression)
        {
            if (expression.IsSimpleWhereToken())
            {
                var number = (string)expression.GetWhereValueByName(nameof(Item.Number));
                return new List<Item> { GetItemByNumber(number, true) };
            }

            return GetAllItems();
        }

        public static Item GetItemByNumber(string number, bool includeThumbnail = false)
        {
            var bcItem = BusinessCentralApi.Instance.GetItem(number);
            var bcItemCard = BusinessCentralApi.Instance.GetItemCard(number);
            if (bcItem == null || bcItemCard == null)
                return new Item();

            var bcAttributes = BusinessCentralApi.Instance.GetItemAttributes(number);
            var bcLinks = BusinessCentralApi.Instance.GetItemLinks(number);

            var item = ComposeItem(bcItem, bcItemCard, bcAttributes, bcLinks);
            if (includeThumbnail)
            {
                var picture = BusinessCentralApi.Instance.GetItemPicture(bcItem);
                item.Thumbnail = picture;
            }

            return item;
        }

        private static IEnumerable<Item> GetAllItems()
        {
            var items = new List<Item>();

            var bcItems = BusinessCentralApi.Instance.GetItems();
            var bcItemCards = BusinessCentralApi.Instance.GetItemCards();
            var bcAttributes = BusinessCentralApi.Instance.GetItemAttributes();
            var bcLinks = BusinessCentralApi.Instance.GetItemLinks();

            foreach (var bcItem in bcItems)
            {
                var bcItemCard = bcItemCards.SingleOrDefault(i => i.No.Equals(bcItem.number));
                if (bcItemCard == null)
                    continue;

                items.Add(ComposeItem(bcItem, bcItemCard, bcAttributes, bcLinks));
            }

            return items;
        }

        private static Item ComposeItem(BC.Item bcItem, BC.ItemCard bcItemCard, List<BC.Attribute> bcAttributes, List<BC.Link> bcLinks)
        {
            return new Item
            {
                Number = bcItem.number,
                Title = bcItem.displayName,
                Description = bcAttributes.SingleOrDefault(l => l.itemNumber.Equals(bcItem.number) && l.attribute.Equals(WebService.Config.ItemAttributeDescription))?.value,
                UnitOfMeasure = bcItem.baseUnitOfMeasureCode,
                Weight = bcItemCard.Net_Weight,
                Material = bcAttributes.SingleOrDefault(l => l.itemNumber.Equals(bcItem.number) && l.attribute.Equals(WebService.Config.ItemAttributeMaterial))?.value,
                Price = bcItem.unitPrice,
                Stock = bcItem.inventory,
                MakeBuy = bcItemCard.Gen_Prod_Posting_Group != WebService.Config.GeneralProductPostingGroupMakeIndicator,
                Blocked = bcItemCard.Blocked,
                Supplier = WebService.Vendors.SingleOrDefault(v => v.number.Equals(bcItemCard.Vendor_No))?.displayName,
                ThinClientLink = bcLinks.SingleOrDefault(l => l.itemNumber.Equals(bcItem.number) && l.description.Equals(WebService.Config.ItemLinkThinClient))?.url,
                ThickClientLink = bcLinks.SingleOrDefault(l => l.itemNumber.Equals(bcItem.number) && l.description.Equals(WebService.Config.ItemLinkThickClient))?.url
            };
        }

        public override void Update(Item entity)
        {
            var bcItemCard = new ItemCard
            {
                No = entity.Number,
                Description = entity.Title,
                Base_Unit_of_Measure = entity.UnitOfMeasure,
                Net_Weight = entity.Weight
            };
            bcItemCard = BusinessCentralApi.Instance.UpdateItemCard(bcItemCard);

            BusinessCentralApi.Instance.SetItemPicture(bcItemCard.No, entity.Thumbnail);
            BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, WebService.Config.ItemAttributeDescription, entity.Description);
            BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, WebService.Config.ItemAttributeMaterial, entity.Material);
            BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThinClientLink, WebService.Config.ItemLinkThinClient);
            BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThickClientLink, WebService.Config.ItemLinkThickClient);
        }

        public override void Create(Item entity)
        {
            var bcItemCard = new ItemCard
            {
                No = entity.Number,
                Description = entity.Title,
                Base_Unit_of_Measure = entity.UnitOfMeasure,
                Net_Weight = entity.Weight
            };
            bcItemCard = BusinessCentralApi.Instance.CreateItemCard(bcItemCard);

            BusinessCentralApi.Instance.SetItemPicture(bcItemCard.No, entity.Thumbnail);
            BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, WebService.Config.ItemAttributeDescription, entity.Description);
            BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, WebService.Config.ItemAttributeMaterial, entity.Material);
            BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThinClientLink, WebService.Config.ItemLinkThinClient);
            BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThickClientLink, WebService.Config.ItemLinkThickClient);
        }

        public override void Delete(Item entity)
        {
            throw new NotSupportedException();
        }
    }
}