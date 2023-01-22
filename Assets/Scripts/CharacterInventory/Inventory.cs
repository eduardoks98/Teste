using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Scriptables;

public class Inventory : MonoBehaviour
{

    [SerializeField] private string _playerID;
    public List<InventoryItem> invetory = new List<InventoryItem>();
    public Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();

    public Action<ItemData, InventoryItem> OnAddItem;
    public Action<ItemData, InventoryItem> OnRemoveItem;

    public string PlayerID { get => _playerID; set => _playerID = value; }

    private void OnEnable()
    {
        Item.OnItemCollected += Add;
    }

    private void OnDisable()
    {
        Item.OnItemCollected -= Add;

    }

    public void Add(ItemData itemData, string playerID)
    {
        if (playerID != PlayerID) return;
        InventoryItem inventoryItem = null; ;
        // Verifica se o item já existe no invetario
        if (itemDictionary.TryGetValue(itemData, out InventoryItem invItem))
        {
            inventoryItem = invItem;
            inventoryItem.AddToStack();
        }
        // Se não existir criamos o mesmo
        else
        {
            inventoryItem = new InventoryItem(itemData);
            invetory.Add(inventoryItem);
            itemDictionary.Add(itemData, inventoryItem);
        }
        if (inventoryItem != null)
            OnAddItem?.Invoke(itemData, inventoryItem);

    }

    public void Remove(ItemData itemData, string playerID)
    {
        if (playerID != PlayerID) return;
        InventoryItem inventoryItem = null;
        // Verifica se o item já existe no invetario
        if (itemDictionary.TryGetValue(itemData, out InventoryItem invItem))
        {
            inventoryItem = invItem;
            inventoryItem.RemoveFromStack();
            if (inventoryItem.stackSize == 0)
            {
                invetory.Remove(inventoryItem);
                itemDictionary.Remove(itemData);
            }
        }
        if (inventoryItem != null)
            OnRemoveItem?.Invoke(itemData, inventoryItem);

    }



}
