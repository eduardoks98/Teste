using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Scriptables;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Core
{
    public class Attributes : MonoBehaviour
    {
        [Header("Current Values")]
        [SerializeField] private float _attackSpeed;
        [SerializeField] private float _attackSpeedDelay;
        [SerializeField] private float _windupDelay;
        [SerializeField] private float _health;
        [SerializeField] private float _dodgeRate;
        public Action<float> OnAttackSpeedChange;
        public Action<float> OnHealthChange;
        public Action<float> OnDodgeRateChange;
        public float AttackSpeed
        {
            get => _attackSpeed; set
            {
                _attackSpeed = value;
                OnAttackSpeedChange?.Invoke(value);
            }
        }
        public float AttackSpeedDelay { get => _attackSpeedDelay; set => _attackSpeedDelay = value; }
        public float WindupDelay { get => _windupDelay; set => _windupDelay = value; }
        public float Health
        {
            get => _health; set
            {
                _health = value;
                OnHealthChange?.Invoke(value);
            }
        }
        public float DodgeRate
        {
            get => _dodgeRate; set
            {
                _dodgeRate = value;
                OnDodgeRateChange?.Invoke(value);
            }
        }


        [Header("Base Values")]
        [SerializeField] private float _baseAttackSpeed;
        [SerializeField] private float _baseAttackRange;
        [SerializeField] private float _baseFieldView;
        [SerializeField] private float _baseMoveSpeed;
        [SerializeField] private float _baseWindup;
        [SerializeField] private float _baseHealth;
        [SerializeField] private float _baseArmorResist;
        [SerializeField] private float _baseMagicResist;
        [SerializeField] private float _basePhysicalDamage;
        [SerializeField] private float _baseMagicDamage;
        [SerializeField] private float _baseDodgeChance;
        public Action<float> OnAttackRangeChange;

        public float BaseAttackSpeed
        {
            get => _baseAttackSpeed; set
            {
                _baseAttackSpeed = value;
                SetupAttackSpeed();
            }
        }

        public float BaseAttackRange
        {
            get => _baseAttackRange; set
            {
                _baseAttackRange = value;
                OnAttackRangeChange?.Invoke(_baseAttackRange);
            }
        }
        public float BaseFieldView { get => _baseFieldView; set => _baseFieldView = value; }

        public float BaseWindup
        {
            get => _baseWindup; set
            {
                _baseWindup = value;
                SetupAttackSpeed();
            }
        }
        public float BaseMoveSpeed { get => _baseMoveSpeed; set => _baseMoveSpeed = value; }

        public float BaseHealth
        {
            get => _baseHealth; set
            {
                _baseHealth = value;
                SetupHealth();
            }
        }

        public float BaseArmorResist { get => _baseArmorResist; set => _baseArmorResist = value; }
        public float BaseMagicResist { get => _baseMagicResist; set => _baseMagicResist = value; }

        public float BasePhysicalDamage { get => _basePhysicalDamage; set => _basePhysicalDamage = value; }
        public float BaseMagicDamage { get => _baseMagicDamage; set => _baseMagicDamage = value; }
        public float BaseDodgeChance
        {
            get => _baseDodgeChance; set
            {
                _baseDodgeChance = value;
                SetupDodgeChance();
            }
        }

        [Header("Aditional modifiers Values")]
        [SerializeField] public List<ItemAttribute> AttackSpeedAditionalList = new List<ItemAttribute>();
        [SerializeField] public List<ItemAttribute> DodgeChanceAditionalList = new List<ItemAttribute>();
        private Dictionary<ItemData, ItemAttribute> _adicionalAttackSpeed = new Dictionary<ItemData, ItemAttribute>();
        private Dictionary<ItemData, ItemAttribute> _adicionalDodgeChance = new Dictionary<ItemData, ItemAttribute>();
        public Dictionary<ItemData, ItemAttribute> AdicionalAttackSpeed { get => _adicionalDodgeChance; set => _adicionalDodgeChance = value; }
        public Dictionary<ItemData, ItemAttribute> AdicionalDodgeChance { get => _adicionalDodgeChance; set => _adicionalDodgeChance = value; }

        [Header("Other Values")]
        [SerializeField] private GameObject _rangeBullet;

        protected GameObject RangeBullet { get => _rangeBullet; set => _rangeBullet = value; }



        [Header("States values")]
        [SerializeField] private bool _canAttack;
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _canDamage;
        [SerializeField] private bool _isRanged = false;
        public bool CanAttack { get => _canAttack; private set => _canAttack = value; }
        public bool CanMove { get => _canMove; private set => _canMove = value; }
        public bool CanDamage { get => _canDamage; set => _canDamage = value; }
        public bool IsRanged { get => _isRanged; set => _isRanged = value; }

        private void Awake()
        {
            CanMove = true;
            CanAttack = true;
            CanDamage = true;
        }
        public IEnumerator IAttackSpeedDelay()
        {
            CanAttack = false;
            yield return new WaitForSeconds(AttackSpeedDelay);
            CanDamage = true;
            CanAttack = true;
        }
        public IEnumerator IWindupDelay()
        {
            // Debug.Log(gameObject.name + " Stop Moving...");
            CanMove = false;
            yield return new WaitForSecondsRealtime(WindupDelay);
            CanMove = true;
        }


        public void SetupAttackSpeed()
        {
            AttackSpeed = AtributesCalcs.CalcBaseLinearStat(_baseAttackSpeed, AttackSpeedAditionalList);
            AttackSpeedDelay = AtributesCalcs.CalcAttackSpeedDelay(_attackSpeed);
            WindupDelay = AtributesCalcs.CalcWindupDelay(_baseWindup, _baseAttackSpeed, _attackSpeed);

        }

        public void SetupDodgeChance()
        {
            DodgeRate = AtributesCalcs.CalcBaseHyperbolicStat(_baseDodgeChance, DodgeChanceAditionalList);
        }

        public bool DodgeTrigger()
        {
            float rand = UnityEngine.Random.Range(0, 100);
            return rand <= (DodgeRate * 100);

        }

        public void SetupHealth()
        {
            Health = _baseHealth;
        }

        private void OnValidate()
        {
            SetupAttackSpeed();
            SetupHealth();

            SetupDodgeChance();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, BaseFieldView);
        }


        public void AddItemHandler(ItemData itemData, InventoryItem inventoryItem)
        {
            foreach (var effect in itemData.effects)
            {
                float calcValue = AtributesCalcs.CalcTypeStack(effect, inventoryItem);
                ItemAttribute atributeItemAditional = new ItemAttribute(itemData, calcValue);

                // Debug.Log(effect.attributeData.type);
                switch (effect.attributeData.type)
                {
                    case AttributeType.AttackSpeed:
                        AtributesCalcs.ItemHandler(itemData, atributeItemAditional, AttackSpeedAditionalList, AdicionalAttackSpeed);
                        SetupAttackSpeed();
                        break;

                    case AttributeType.DodgeChance:
                        AtributesCalcs.ItemHandler(itemData, atributeItemAditional, DodgeChanceAditionalList, AdicionalDodgeChance);
                        SetupDodgeChance();
                        break;
                    default:
                        throw new Exception("Atributo: " + effect.attributeData.type + " não cadastrado!");
                }
            }
        }

        public void getAttributeValueByAttributeType(AttributeType attributeType, out float value, ref Action<float> actionHandler)
        {
            switch (attributeType)
            {
                case AttributeType.Health:
                    value = Health;
                    OnHealthChange += actionHandler;
                    break;
                case AttributeType.ArmorResist:
                    value = BaseArmorResist;
                    // actionHandler = null;
                    break;
                case AttributeType.AttackRange:
                    value = BaseAttackRange;
                    OnAttackRangeChange += actionHandler;
                    break;
                case AttributeType.AttackSpeed:
                    value = AttackSpeed;
                    OnAttackSpeedChange += actionHandler;
                    break;
                case AttributeType.DodgeChance:
                    value = DodgeRate;
                    OnDodgeRateChange += actionHandler;
                    break;
                case AttributeType.MagicDamage:
                    value = BaseMagicDamage;
                    // actionHandler = null;
                    break;
                case AttributeType.MagicResist:
                    value = BaseMagicResist;
                    // actionHandler = null;
                    break;
                case AttributeType.MoveSpeed:
                    value = BaseMoveSpeed;
                    // actionHandler = null;
                    break;
                case AttributeType.PhysicalDamage:
                    value = BasePhysicalDamage;
                    // actionHandler = null;
                    break;
                case AttributeType.Tenacity:
                    value = 0;
                    // actionHandler = null;
                    break;
                default:
                    value = 0;
                    // actionHandler = null;
                    break;
            }

        }

    }
}
