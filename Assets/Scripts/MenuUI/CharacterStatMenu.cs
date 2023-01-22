using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using UnityEngine;

public class CharacterStatMenu : MonoBehaviour
{
    [SerializeField] List<CharacterStatProperty> _characterStatProperties = new List<CharacterStatProperty>();
    public List<CharacterStatProperty> CharacterStatProperties { get => _characterStatProperties; set => _characterStatProperties = value; }

    // private void OnEnable()
    // {
    //     setup();
    // }

    // private void OnValidate()
    // {
    //     setup();
    // }

    public void setup(PlayerController playerController)
    {
        CharacterStatProperties = new List<CharacterStatProperty>(gameObject.GetComponentsInChildren<CharacterStatProperty>());
        assingStats(playerController);
    }

    private void assingStats(PlayerController playerController)
    {
        foreach (CharacterStatProperty characterStatProperty in CharacterStatProperties)
        {
            // out reference to value
            float attributeValue;
            // Create action to assing on change triggers of atttributes
            Action<float> attributeActionHandler = (float value) => characterStatProperty.InputStatValue = value.ToString("F2");
            // Call method to return te attributeValue and assing the Actions
            playerController.Attributes.getAttributeValueByAttributeType(characterStatProperty.AttributeType, out attributeValue, ref attributeActionHandler);
            // Trigger the custom actionHandler
            attributeActionHandler(attributeValue);


        }
    }

}
