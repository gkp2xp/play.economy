using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Repositories;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemRepository itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(ItemRepository itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await itemsRepository.GetAllItemUploadAsync())
                        .Select(item => item.AsDto());

            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetItemUploadAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        // POST /items
        [HttpPost]
        [Authorize(Policies.Write)]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                FileId = createItemDto.FileId,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);
            var itemUpload = (await itemsRepository.GetItemUploadAsync(item.Id)).AsDto();

            await publishEndpoint.Publish(new CatalogItemCreated(
                item.Id, 
                item.Name, 
                item.Description,
                item.Price));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, itemUpload);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        [Authorize(Policies.Write)]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;
            existingItem.FileId = updateItemDto.FileId;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(
                existingItem.Id, 
                existingItem.Name, 
                existingItem.Description,
                existingItem.Price));

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        //[Authorize(Policies.Write)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}