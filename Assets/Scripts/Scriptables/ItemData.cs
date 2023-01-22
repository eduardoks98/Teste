using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Core;

namespace Assets.Scripts.Scriptables
{
    [CreateAssetMenu]
    public class ItemData : ScriptableObject
    {
        public string displayName;
        public Sprite sprite;

        public List<AttributeEffect> effects = new List<AttributeEffect>();
        public Dictionary<AttributeEffect, float> itemDictionary = new Dictionary<AttributeEffect, float>();


    }
}
