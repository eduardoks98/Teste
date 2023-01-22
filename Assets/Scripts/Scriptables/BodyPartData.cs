using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;
using Assets.Scripts.Core;

namespace Assets.Scripts.Scriptables
{
    [CreateAssetMenu]
    public class BodyPartData : ScriptableObject
    {
        public string displayName;
        public BodyType bodyType;
        public GameObject prefab;

        public List<AttributeEffect> effects = new List<AttributeEffect>();
        public Dictionary<AttributeEffect, float> itemDictionary = new Dictionary<AttributeEffect, float>();

        public float CalculatePercetage(AttributeEffect attributeEffect, InventoryItem inventoryItem)
        {
            return AtributesCalcs.CalcTypeStack(attributeEffect, inventoryItem);
        }

    }
}