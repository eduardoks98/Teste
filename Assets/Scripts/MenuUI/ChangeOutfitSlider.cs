using System;
using Assets.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOutfitSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bodyPartTitleText;
    [SerializeField] private TextMeshProUGUI _bodyPartDescriptionText;
    [SerializeField] private Slider _bodyPartSlider;
    [SerializeField] private BodyType _bodyType;

    public Action<int, BodyType, ChangeOutfitSlider> OnSliderChange;

    public TextMeshProUGUI BodyPartTitleText { get => _bodyPartTitleText; private set => _bodyPartTitleText = value; }
    public TextMeshProUGUI BodyParDescriptionText { get => _bodyPartDescriptionText; private set => _bodyPartDescriptionText = value; }
    public Slider BodyPartSlider { get => _bodyPartSlider; set => _bodyPartSlider = value; }
    public BodyType BodyType { get => _bodyType; set => _bodyType = value; }

    public void SetTitle(string value)
    {
        BodyPartTitleText.text = value;
    }
    public void SetDescription(string value)
    {
        BodyParDescriptionText.text = value;
    }

    public void SetSliderRange(int max)
    {
        BodyPartSlider.value = 1;
        BodyPartSlider.minValue = 1;
        BodyPartSlider.maxValue = max;
    }

    private void SetSliderValue(){

    }

    private void OnEnable()
    {
        BodyPartSlider = GetComponentInChildren<Slider>();
        BodyPartSlider.onValueChanged.AddListener(delegate { ChangeSliderHandler(); });
    }

    private void OnDisable()
    {
        BodyPartSlider.onValueChanged.RemoveListener(delegate { ChangeSliderHandler(); });

    }

    public virtual void ChangeSliderHandler()
    {
        // Debug.Log("Slider Change");
        OnSliderChange?.Invoke((int)BodyPartSlider.value, BodyType, this);
    }
}
