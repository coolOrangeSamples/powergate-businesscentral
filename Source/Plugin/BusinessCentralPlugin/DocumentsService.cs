using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using BusinessCentralPlugin.Helper;
using powerGateServer.SDK;

namespace BusinessCentralPlugin
{
    [DataServiceKey("Number", "FileName")]
    [DataServiceEntity]
    public class Document : Streamable
    {
        public string Number { get; set; }

        public override string GetContentType()
        {
            return ContentTypes.Application.Pdf;
        }
    }

    public class Documents : ServiceMethod<Document>, IStreamableServiceMethod<Document>
    {
        public override IEnumerable<Document> Query(IExpression<Document> expression)
        {
            var results = new List<Document>();

            if (expression.Where.Any(b => b.PropertyName.Equals("Number")))
            {
                var number = (string)expression.GetWhereValueByName(nameof(Item.Number));
                var documents = Task.Run(async () => await BusinessCentralApi.Instance.GetDocuments(number)).Result;
                foreach (var document in documents)
                {
                    results.Add(new Document
                    {
                        Number = number,
                        FileName = document.fileName
                    });
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            return results;
        }

        public override async void Create(Document entity)
        {
            await BusinessCentralApi.Instance.CreateDocument(entity.Number, entity.FileName);
        }

        public override void Update(Document entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(Document entity)
        {

        }

        public IStream Download(Document entity)
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");

            var bytes = Task.Run(async () => await BusinessCentralApi.Instance.DownloadDocument(entity.Number, entity.FileName)).Result;

            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = $"filename={entity.FileName}";

            return new powerGateServer.SDK.Streams.ByteStream(bytes);
        }

        public async void Upload(Document entity, IStream stream)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                await stream.Source.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            await BusinessCentralApi.Instance.UploadDocument(entity.Number, entity.FileName, bytes);
        }

        public void DeleteStream(Document entity)
        {
            throw new NotImplementedException();
        }
    }
}