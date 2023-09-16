using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service;

public static class Extentions
{
    public static InventoryItemDto AsDtos(this InventoryItem item){
        return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
    }
} 