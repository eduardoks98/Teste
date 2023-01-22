using System;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MyEvent : UnityEvent<string> { }

public class CharacterStatProperty : MonoBehaviour
{
    public MyEvent OnEvent;
    [SerializeField] private string _inputStatName;
    [SerializeField] private string _inputStatValue;
    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _statValue;

    [SerializeField] private AttributeType _attributeType;

    public TextMeshProUGUI StatValue { get => _statValue; set => _statValue = value; }
    public TextMeshProUGUI StatName { get => _statName; set => _statName = value; }
    public string InputStatValue
    {
        get => _inputStatValue; set
        {
            // Debug.Log("Stat Value CHanged");

            _inputStatValue = value;
            OnInputValueChange?.Invoke(value);
            // OnEvent.Invoke(value);
        }
    }
    public string InputStatName
    {
        get => _inputStatName; set
        {
            // Debug.Log("Stat Name CHanged");
            _inputStatName = value;
            OnInputNameChange?.Invoke(value);
            // OnEvent.Invoke(value);
        }
    }

    public AttributeType AttributeType { get => _attributeType; set => _attributeType = value; }

    public Action<string> OnInputNameChange;
    public Action<string> OnInputValueChange;


    public void SetName(string value)
    {
        // Debug.Log("SetNAme trigger");
        StatName.text = value;
    }
    public void SetValue(string value)
    {
        // Debug.Log("SetVALUE trigger");
        StatValue.text = value;
    }

    private void OnEnable()
    {
        OnInputNameChange += SetName;
        OnInputValueChange += SetValue;
        // OnEvent.AddListener(SetName);
    }
    private void OnDisable()
    {
        OnInputNameChange -= SetName;
        OnInputValueChange -= SetValue;
    }
    private void OnValidate()
    {
        OnEvent.Invoke(InputStatName);
        OnEvent.Invoke(InputStatValue);
        SetName(InputStatName);
        SetValue(InputStatValue);

        // _inputStatName = StatName.text;
        // _inputStatValue = StatValue.text;
    }

}
