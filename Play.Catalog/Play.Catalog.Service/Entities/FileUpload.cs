using System;
using Play.Common;

namespace Play.Catalog.Service.Entities
{
    public class FileUpload : IEntity
    {
        public Guid Id { get; set; }

        public string Uri { get; set; }

        public string Path { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}