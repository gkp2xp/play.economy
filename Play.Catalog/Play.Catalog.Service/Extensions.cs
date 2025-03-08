using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using System;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this ItemUpload item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.FileId == Guid.Empty ? null: item.FileId.ToString(), item.Uri, item.CreatedDate);
        }
    }
}