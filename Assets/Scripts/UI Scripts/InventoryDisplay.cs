using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
      [SerializeField] private MouseItemData mouseInventoryItem;
      private PlayerInput playerInput;
      
      protected InventorySystem inventorySystem;
      protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary;
      
      public InventorySystem InventorySystem => inventorySystem;
      public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

      protected virtual void Start()
      {
            playerInput = new PlayerInput();
            playerInput.Enable();
      }

      private void Update()
      {
            if (Keyboard.current.leftAltKey.ReadValue() != 0 ||
                mouseInventoryItem.AssignedInventorySlot.ItemData == null) return;
            inventorySystem.AddToInventory(mouseInventoryItem.AssignedInventorySlot.ItemData,
                  mouseInventoryItem.AssignedInventorySlot.StackSize);
            mouseInventoryItem.ClearSlot();
      }

      public abstract void AssignSlot(InventorySystem invToDisplay);

      protected virtual void UpdateSlot(InventorySlot updatedSlot)
      {
            foreach (var slot in SlotDictionary.Where(slot => slot.Value == updatedSlot))
            {
                  slot.Key.UpdateUISlot(updatedSlot);
            }
      }

      public void SlotClicked(InventorySlot_UI clickedUISlot)
      {
            var isTap = Keyboard.current.tabKey.isPressed;
            
            if (playerInput.Player.Showmouse.ReadValue<float>() == 0) return;

            if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
                mouseInventoryItem.AssignedInventorySlot.ItemData == null)
            {
                  if (isTap && clickedUISlot.AssignedInventorySlot.SplitStack(out var halfStackSlot))
                  {
                        mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                        clickedUISlot.UpdateUISlot();
                        return;
                  }

                  mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                  clickedUISlot.ClearSlot();
                  return;

            }

            if (clickedUISlot.AssignedInventorySlot.ItemData == null &&
                mouseInventoryItem.AssignedInventorySlot.ItemData != null)
            {
                  clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                  clickedUISlot.UpdateUISlot();

                  mouseInventoryItem.ClearSlot();
                  return;
            }

            if (clickedUISlot.AssignedInventorySlot.ItemData != null &&
                mouseInventoryItem.AssignedInventorySlot.ItemData != null)
            {
                  var isSameItem = clickedUISlot.AssignedInventorySlot.ItemData ==
                                 mouseInventoryItem.AssignedInventorySlot.ItemData;
                  if (isSameItem
                      && clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot
                            .StackSize))
                  {
                        clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                        clickedUISlot.UpdateUISlot();

                        mouseInventoryItem.ClearSlot();
                        return;
                  }

                  if (isSameItem &&
                      !clickedUISlot.AssignedInventorySlot.RoomLeftInStack(mouseInventoryItem.AssignedInventorySlot
                            .StackSize, out var leftInStack))
                  {
                        if (leftInStack < 1) SwapSlots(clickedUISlot);
                        else
                        {
                              var remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                              clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                              clickedUISlot.UpdateUISlot();

                              var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData,
                                    remainingOnMouse);
                              mouseInventoryItem.ClearSlot();
                              mouseInventoryItem.UpdateMouseSlot(newItem);
                        }
                  }
                  else if (!isSameItem)
                  {
                        SwapSlots(clickedUISlot);
                  }
            }
      }

      private void SwapSlots(InventorySlot_UI clickedUISlot)
      {
            var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData,
                  mouseInventoryItem.AssignedInventorySlot.StackSize);
            mouseInventoryItem.ClearSlot();
            
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
            
            clickedUISlot.ClearSlot();
            clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
            clickedUISlot.UpdateUISlot();
      }
}
