using AutoMapper;
using InventoryManagement.API.DTOs;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing inventory items.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryItemsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryItemsController"/> class with specified inventory service and mapper.
        /// </summary>
        /// <param name="inventoryService">The service for inventory operations.</param>
        /// <param name="mapper">The mapper for object transformations.</param>
        public InventoryItemsController(IInventoryService inventoryService, IMapper mapper)
        {
            _inventoryService = inventoryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all inventory items.
        /// </summary>
        /// <returns>A list of inventory items.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemReadDto>>> GetAllInventoryItems()
        {
            var items = await _inventoryService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<InventoryItemReadDto>>(items));
        }

        /// <summary>
        /// Retrieves a specific inventory item by ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item.</param>
        /// <returns>The inventory item if found; otherwise, NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItemReadDto>> GetInventoryItem(long id)
        {
            var item = await _inventoryService.GetByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<InventoryItemReadDto>(item));
        }

        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        /// <param name="createDto">The inventory item data transfer object to create the item.</param>
        /// <returns>The created inventory item.</returns>
        [HttpPost]
        public async Task<ActionResult<InventoryItemReadDto>> CreateInventoryItem(InventoryItemCreateDto createDto)
        {
            var inventoryItem = _mapper.Map<InventoryItem>(createDto);
            var newItem = await _inventoryService.AddAsync(inventoryItem);
            var readDto = _mapper.Map<InventoryItemReadDto>(newItem);

            return CreatedAtAction(nameof(GetInventoryItem), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates a specific inventory item by ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item to update.</param>
        /// <param name="updateDto">The inventory item data transfer object with updated fields.</param>
        /// <returns>An IActionResult indicating the outcome of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(long id, InventoryItemUpdateDto updateDto)
        {
            if (!InventoryItemExists(id))
            {
                return NotFound();
            }

            var inventoryItem = await _inventoryService.GetByIdAsync(id);
            _mapper.Map(updateDto, inventoryItem);
            await _inventoryService.UpdateAsync(inventoryItem);

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific inventory item by ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item to delete.</param>
        /// <returns>An IActionResult indicating the outcome of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(long id)
        {
            if (!InventoryItemExists(id))
            {
                return NotFound();
            }

            await _inventoryService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Checks if an inventory item exists by ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item.</param>
        /// <returns>true if the inventory item exists; otherwise, false.</returns>
        private bool InventoryItemExists(long id)
        {
            var item = _inventoryService.GetByIdAsync(id).Result;
            return item != null;
        }
    }
}
