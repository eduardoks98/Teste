using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Scriptables;
using UnityEngine;

namespace Scripts.CharacterCreation
{

    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField] private List<BodyPartData> _availableCharacterParts = new List<BodyPartData>();

        [SerializeField] private List<BodyPartData> _availableHeadParts = new List<BodyPartData>();
        [SerializeField] private List<BodyPartData> _availableBodyParts = new List<BodyPartData>();
        [SerializeField] private List<BodyPartData> _availableArmRightParts = new List<BodyPartData>();
        [SerializeField] private List<BodyPartData> _availableArmLeftParts = new List<BodyPartData>();
        [SerializeField] private List<BodyPartData> _availableLegRightParts = new List<BodyPartData>();
        [SerializeField] private List<BodyPartData> _availableLegLeftParts = new List<BodyPartData>();

        [SerializeField] private List<CharacterPart> _character = new List<CharacterPart>();

        [SerializeField] private GameObject _characterCreationMenu;
        [SerializeField] private GameObject _sliderSelectionPF;



        public List<BodyPartData> AvailableCharacterParts
        {
            get => _availableCharacterParts; set
            {
                _availableCharacterParts = value;


                AssingCharacterParts();
            }
        }

        public List<BodyPartData> AvailableHeadParts { get => _availableHeadParts; set => _availableHeadParts = value; }
        public List<BodyPartData> AvailableBodyParts { get => _availableBodyParts; set => _availableBodyParts = value; }
        public List<BodyPartData> AvailableArmRightParts { get => _availableArmRightParts; set => _availableArmRightParts = value; }
        public List<BodyPartData> AvailableArmLeftParts { get => _availableArmLeftParts; set => _availableArmLeftParts = value; }
        public List<BodyPartData> AvailableLegRightParts { get => _availableLegRightParts; set => _availableLegRightParts = value; }
        public List<BodyPartData> AvailableLegLeftParts { get => _availableLegLeftParts; set => _availableLegLeftParts = value; }
        public List<CharacterPart> Character { get => _character; set => _character = value; }
        public GameObject CharacterCreationMenu { get => _characterCreationMenu; set => _characterCreationMenu = value; }
        public GameObject SliderSelectionPF { get => _sliderSelectionPF; set => _sliderSelectionPF = value; }

        public void AssingCharacterParts(bool immediate = false)
        {
            AvailableHeadParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.Head));
            AvailableBodyParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.Torso));
            AvailableArmLeftParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.LeftArm));
            AvailableArmRightParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.RightArm));
            AvailableLegRightParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.RightLeg));
            AvailableLegLeftParts = new List<BodyPartData>(AvailableCharacterParts.FindAll(match => match.bodyType == BodyType.LeftLeg));

            Character = new List<CharacterPart>(FindObjectsOfType<CharacterPart>());

            if (!CharacterCreationMenu) return;
            DestroyChildren(CharacterCreationMenu, immediate);

            if (!SliderSelectionPF) return;
            InstantiateSelectionSlider(BodyType.Head);
            InstantiateSelectionSlider(BodyType.Torso);
            InstantiateSelectionSlider(BodyType.RightArm);
            InstantiateSelectionSlider(BodyType.LeftArm);
            InstantiateSelectionSlider(BodyType.RightLeg);
            InstantiateSelectionSlider(BodyType.LeftLeg);

        }

        private void InstantiateSelectionSlider(BodyType type)
        {
            GameObject instance = Instantiate(SliderSelectionPF, CharacterCreationMenu.transform);
            ChangeOutfitSlider changeOutfitSlider = instance.GetComponent<ChangeOutfitSlider>();
            changeOutfitSlider.BodyType = type;
            switch (type)
            {
                case BodyType.Head:
                    changeOutfitSlider.SetTitle("Head");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableHeadParts.Count);
                    // AvailableHeadParts.FindIndex();
                    break;
                case BodyType.Torso:
                    changeOutfitSlider.SetTitle("Body");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableBodyParts.Count);
                    break;
                case BodyType.RightArm:
                    changeOutfitSlider.SetTitle("Right Arm");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableArmRightParts.Count);
                    break;
                case BodyType.LeftArm:
                    changeOutfitSlider.SetTitle("Left Arm");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableArmLeftParts.Count);
                    break;
                case BodyType.RightLeg:
                    changeOutfitSlider.SetTitle("Right Leg");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableLegRightParts.Count);
                    break;
                case BodyType.LeftLeg:
                    changeOutfitSlider.SetTitle("Left Leg");
                    changeOutfitSlider.OnSliderChange += OnSliderChange;
                    changeOutfitSlider.SetSliderRange(AvailableLegLeftParts.Count);
                    break;
            }

        }


        public List<BodyPartData> FindBodyPartList(BodyType bodyType)
        {
            switch (bodyType)
            {
                case BodyType.Head:
                    return AvailableHeadParts;
                case BodyType.Torso:
                    return AvailableBodyParts;
                case BodyType.RightArm:
                    return AvailableArmRightParts;
                case BodyType.LeftArm:
                    return AvailableArmLeftParts;
                case BodyType.RightLeg:
                    return AvailableLegRightParts;
                case BodyType.LeftLeg:
                    return AvailableLegLeftParts;
            }

            throw new System.Exception("Body type nao encontrado");
        }

        public void OnSliderChange(int value, BodyType bodyType, ChangeOutfitSlider outfit)
        {
            CharacterPart characterPart = Character.Find(match => match.BodyType == bodyType);
            List<BodyPartData> list = FindBodyPartList(bodyType);
            if (value > list.Count) return;
            GameObject pf = list[value - 1].prefab;
            if (!pf)
            {
                characterPart.SpriteRenderer.sprite = null;
                outfit.SetDescription("");
                return;
            }
            outfit.SetDescription(list[value - 1].displayName);
            SpriteRenderer spriteRenderer = pf.GetComponent<SpriteRenderer>();

            characterPart.SpriteRenderer.sprite = spriteRenderer.sprite;
            characterPart.SpriteRenderer.flipX = spriteRenderer.flipX;
            characterPart.SpriteRenderer.flipY = spriteRenderer.flipY;

        }


        private void Awake()
        {
            // Debug.Log("ON AWAKE");
            // AssingCharacterParts();
        }
        private void OnEnable()
        {
            // Debug.Log("ON ENABLE");
            AssingCharacterParts();
        }
        private void OnValidate()
        {
            // Debug.Log("ON VALIDATE");
            AssingCharacterParts(true);
        }

        public void DestroyChildren(GameObject t, bool immediate = false)
        {
            // Debug.Log("BEFORE: " + t.transform.childCount);
            int i = 0;

            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[t.transform.childCount];

            //Find all child obj and store to that array
            foreach (Transform child in t.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                // if (immediate) DestroyImmediate(child.gameObject);
                // if (Application.isPlaying) Destroy(child.gameObject);
                // else DestroyImmediate(child.gameObject);
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(child.gameObject);
                };
            }

            // Debug.Log("AFTER: " + t.transform.childCount);
        }
    }
}
