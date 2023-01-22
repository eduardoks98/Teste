using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Scriptables;
using Assets.Scripts.Enums;
using Assets.Scripts.Core;

public static class AtributesCalcs
{
    public static float CalcBaseLinearStat(float baseStats, List<ItemAttribute> adicionalStats)
    {
        float somaAdicional = sumList(adicionalStats);
        if (somaAdicional == 0) return baseStats;
        return (baseStats <= 0 ? 0 : baseStats) * (somaAdicional);
    }

    public static float CalcBaseHyperbolicStat(float baseStats, List<ItemAttribute> adicionalStats)
    {
        float somaAdicional = sumList(adicionalStats);
        if (somaAdicional == 0) return baseStats;
        return baseStats + (somaAdicional);
    }

    public static float CalcAttackSpeedDelay(float finalAttackSpeed) => (1 / finalAttackSpeed);

    public static float CalcWindupDelay(float baseWindup, float baseAttackSpeed, float finalAttackSpeed)
    {
        float windupPercent = (baseWindup / 100);
        float bWindupTime = (1 / baseAttackSpeed) * windupPercent;
        float cAttackTime = AtributesCalcs.CalcAttackSpeedDelay(finalAttackSpeed);
        return bWindupTime + ((cAttackTime * windupPercent) - bWindupTime);
    }

    public static float CalcMoveSpeed(float baseMoveSpeed)
    {
        return baseMoveSpeed;
    }

    public static float CalcDamageTaken(float currentHealth, float physicalDamage, float magicDamage, float armorResist, float magicResist)
    {
        float pDamage = armorResist >= 0 ? physicalDamage * (100 / (100 + armorResist)) : physicalDamage * (2 - (100 / (100 - armorResist)));
        float mDamage = magicResist >= 0 ? magicDamage * (100 / (100 + magicResist)) : magicDamage * (2 - (100 / (100 - magicResist)));
        // Debug.Log(pDamage + " " + mDamage);
        return pDamage + mDamage;
    }

    public static float CalcHyperbolicStack(float percentage, float quantity) => (1 - 1 / (AtributesCalcs.CalcLinearStack(percentage, quantity)));
    public static float CalcLinearStack(float percentage, float quantity) => (1 + (percentage / 100) * quantity);
    public static float CalcExponentialStack(float percentage, float quantity) => (Mathf.Pow(percentage, quantity));
    public static float CalcSpecialStack(float percentage, float quantity) => (Mathf.Pow(percentage, quantity));

    public static float CalcTypeStack(AttributeEffect effect, InventoryItem item)
    {
        switch (effect.attributeData.calcType)
        {
            case CalcType.Linear:
                return AtributesCalcs.CalcLinearStack(effect.percentage, item.stackSize);
            case CalcType.Hyperbolic:
                return AtributesCalcs.CalcHyperbolicStack(effect.percentage, item.stackSize);
            case CalcType.Special:
                return AtributesCalcs.CalcSpecialStack(effect.percentage, item.stackSize);
            default:
                throw new System.Exception("Tipo de calculo inexistente: " + effect.attributeData.calcType);
        }
    }

    public static float sumList(List<ItemAttribute> list)
    {
        float result = 0;
        foreach (ItemAttribute item in list)
            result += item.percentage;

        return result;
    }

    public static float sumDictionary(Dictionary<ItemData, ItemAttribute> list)
    {
        float result = 0;
        foreach (KeyValuePair<ItemData, ItemAttribute> item in list)
            result += item.Value.percentage;

        return result;
    }


    public static void ItemHandler(ItemData itemData, ItemAttribute atributeItemAditional, List<ItemAttribute> list, Dictionary<ItemData, ItemAttribute> dictionary)
    {
        if (dictionary.TryGetValue(itemData, out ItemAttribute adicionalAttribute))
        {
            // Remove os atributos
            list.Remove(adicionalAttribute);
            dictionary.Remove(itemData);
            // Adiciona novamente recalculado
            list.Add(atributeItemAditional);
            dictionary.Add(itemData, atributeItemAditional);
        }
        else
        {
            // Adicional valores
            list.Add(atributeItemAditional);
            dictionary.Add(itemData, atributeItemAditional);
        }

    }
}
