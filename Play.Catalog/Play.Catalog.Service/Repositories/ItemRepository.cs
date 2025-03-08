using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Common.MongoDB;

namespace Play.Catalog.Repositories
{

    public class ItemRepository : MongoRepository<Item>
    {
        private readonly IMongoCollection<ItemUpload> dbCollection;
        private readonly FilterDefinitionBuilder<ItemUpload> filterBuilder = Builders<ItemUpload>.Filter;
        public ItemRepository(IMongoDatabase database): base(database, "items")
        {
            dbCollection = database.GetCollection<ItemUpload>("vwItemUpload");
        }

        public async Task<IReadOnlyCollection<ItemUpload>> GetAllItemUploadAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<ItemUpload> GetItemUploadAsync(Guid id)
        {
            FilterDefinition<ItemUpload> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}