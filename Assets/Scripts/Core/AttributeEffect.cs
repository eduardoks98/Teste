using System;
using Assets.Scripts.Scriptables;

namespace Assets.Scripts.Core
{
    [Serializable]
    public class AttributeEffect
    {
        public AttributeData attributeData;
        public float percentage;
        public AttributeEffect(AttributeData _attributeData, float perc)
        {
            attributeData = _attributeData;
            percentage = perc;
        }
    }
}
