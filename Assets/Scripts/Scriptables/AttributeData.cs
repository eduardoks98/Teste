
using UnityEngine;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Scriptables
{
    [CreateAssetMenu]
    public class AttributeData : ScriptableObject
    {
        public CalcType calcType;
        public AttributeType type;
    }
}