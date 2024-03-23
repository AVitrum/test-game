using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    
    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (var i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        if (ContainsItem(itemToAdd, out var invSlots))
        {
            foreach (var slot in invSlots.Where(slot => slot.RoomLeftInStack(amountToAdd)))
            {
                slot.AddToStack(amountToAdd);
                OnInventorySlotChanged?.Invoke(slot);
                return true;
            }
        }

        if (!HasFreeSlot(out var freeSlot)) return false;
        
        freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
        OnInventorySlotChanged?.Invoke(freeSlot);
        return true;
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(item => item.ItemData == itemToAdd).ToList();
        Debug.Log(invSlot.Count);
        return invSlot != null;
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot != null;
    }
}
