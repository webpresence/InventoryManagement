using AutoMapper;
using InventoryManagement.API.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.API.Mapping
{
    /// <summary>
    /// Represents the mapping profile for inventory items.
    /// </summary>
    public class InventoryItemProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryItemProfile"/> class.
        /// </summary>
        public InventoryItemProfile()
        {
            CreateMap<InventoryItem, InventoryItemReadDto>();
            CreateMap<InventoryItemCreateDto, InventoryItem>();
            CreateMap<InventoryItemUpdateDto, InventoryItem>().ReverseMap();
        }
    }
}
