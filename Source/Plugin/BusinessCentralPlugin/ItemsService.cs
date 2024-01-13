using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace BusinessCentralPlugin
{
    using Attribute = BusinessCentral.Attribute;
    using ItemCard = BusinessCentral.ItemCard;
    using Link = BusinessCentral.Link;

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
                return new List<Item> { Task.Run(async () => await GetItemByNumberAsync(number, true)).Result };
            }

            //TODO: Implement filter
            return GetAllItems();
        }

        public static async Task<Item> GetItemByNumberAsync(string number, bool includeThumbnail = false)
        {
            var bcItemCardTask = BusinessCentralApi.Instance.GetItemCard(number);
            var bcAttributesTask = BusinessCentralApi.Instance.GetItemAttributes(number);
            var bcLinksTask = BusinessCentralApi.Instance.GetItemLinks(number);
            Task.WaitAll(bcItemCardTask, bcAttributesTask, bcLinksTask);

            var bcItemCard = bcItemCardTask.Result;
            var bcAttributes = bcAttributesTask.Result;
            var bcLinks = bcLinksTask.Result;

            if (bcItemCard == null)
                return new Item();

            var item = ComposeItem(bcItemCard, bcAttributes, bcLinks);
            if (includeThumbnail)
            {
                var picture = await BusinessCentralApi.Instance.GetItemPicture(number);
                item.Thumbnail = picture;
            }

            return item;
        }

        private static IEnumerable<Item> GetAllItems()
        {
            var items = new List<Item>();

            var bcItemCardsTask = BusinessCentralApi.Instance.GetItemCards();
            var bcAttributesTask = BusinessCentralApi.Instance.GetItemAttributes();
            var bcLinksTask = BusinessCentralApi.Instance.GetItemLinks();
            Task.WaitAll(bcItemCardsTask, bcAttributesTask, bcLinksTask);

            var bcItemCards = bcItemCardsTask.Result;
            var bcAttributes = bcAttributesTask.Result;
            var bcLinks = bcLinksTask.Result;

            foreach (var bcItemCard in bcItemCards)
                items.Add(ComposeItem(bcItemCard, bcAttributes, bcLinks));

            return items;
        }

        private static Item ComposeItem(ItemCard bcItemCard, List<Attribute> bcAttributes, List<Link> bcLinks)
        {
            return new Item
            {
                Number = bcItemCard.No,
                Title = bcItemCard.Description,
                Description = bcAttributes.SingleOrDefault(l => l.itemNumber.Equals(bcItemCard.No) && l.attribute.Equals(Configuration.ItemAttributeDescription))?.value,
                UnitOfMeasure = bcItemCard.Base_Unit_of_Measure,
                Weight = bcItemCard.Net_Weight,
                Material = bcAttributes.SingleOrDefault(l => l.itemNumber.Equals(bcItemCard.No) && l.attribute.Equals(Configuration.ItemAttributeMaterial))?.value,
                Price = bcItemCard.Unit_Price,
                Stock = bcItemCard.Inventory,
                MakeBuy = bcItemCard.Gen_Prod_Posting_Group != Configuration.GeneralProductPostingGroupMakeIndicator,
                Blocked = bcItemCard.Blocked,
                Supplier = Configuration.Vendors.SingleOrDefault(v => v.number.Equals(bcItemCard.Vendor_No))?.displayName,
                ThinClientLink = bcLinks.SingleOrDefault(l => l.itemNumber.Equals(bcItemCard.No) && l.description.Equals(Configuration.ItemLinkThinClient))?.url,
                ThickClientLink = bcLinks.SingleOrDefault(l => l.itemNumber.Equals(bcItemCard.No) && l.description.Equals(Configuration.ItemLinkThickClient))?.url
            };
        }

        public override async void Update(Item entity)
        {
            var bcItemCard = new ItemCard
            {
                No = entity.Number,
                Description = entity.Title,
                Base_Unit_of_Measure = entity.UnitOfMeasure,
                Net_Weight = entity.Weight
            };
            bcItemCard = await BusinessCentralApi.Instance.UpdateItemCard(bcItemCard);

            var tasks = new List<Task>
            {
                BusinessCentralApi.Instance.SetItemPicture(bcItemCard.No, entity.Thumbnail),
                BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, Configuration.ItemAttributeDescription, entity.Description),
                BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, Configuration.ItemAttributeMaterial, entity.Material),
                BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThinClientLink, Configuration.ItemLinkThinClient),
                BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThickClientLink, Configuration.ItemLinkThickClient),
            };
            Task.WaitAll(tasks.ToArray());
        }

        public override async void Create(Item entity)
        {
            var bcItemCard = new ItemCard
            {
                No = entity.Number,
                Description = entity.Title,
                Base_Unit_of_Measure = entity.UnitOfMeasure,
                Net_Weight = entity.Weight
            };
            bcItemCard = await BusinessCentralApi.Instance.CreateItemCard(bcItemCard);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var tasks = new List<Task>
            {
                BusinessCentralApi.Instance.SetItemPicture(bcItemCard.No, entity.Thumbnail),
                BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, Configuration.ItemAttributeDescription, entity.Description),
                BusinessCentralApi.Instance.SetItemAttribute(bcItemCard.No, Configuration.ItemAttributeMaterial, entity.Material),
                BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThinClientLink, Configuration.ItemLinkThinClient),
                BusinessCentralApi.Instance.SetItemLink(bcItemCard.No, entity.ThickClientLink, Configuration.ItemLinkThickClient)
            };
            Task.WaitAll(tasks.ToArray());
        }

        public override void Delete(Item entity)
        {
            throw new NotSupportedException();
        }
    }
}