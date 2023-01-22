using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Scriptables
{
    [CreateAssetMenu]
    public class EnemyData : ScriptableObject
    {
        public string displayName;
        public float spawnChance;
        public GameObject enemyPrefab;

        public List<AttributeEffect> effects = new List<AttributeEffect>();
        public Dictionary<AttributeEffect, float> itemDictionary = new Dictionary<AttributeEffect, float>();

        public float CalculatePercetage(AttributeEffect attributeEffect, InventoryItem inventoryItem)
        {
            return AtributesCalcs.CalcTypeStack(attributeEffect, inventoryItem);
        }

    }
}