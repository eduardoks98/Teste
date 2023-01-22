using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(Attributes))]
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(ManageSpriteOrder))]
    public class Character : UniqueMonoBehaviour, IDamageable
    {
        public bool _DEBUG = false;
        const float MOVE_LIMITER = 0.7f;

        [Header("Required Components")]
        private Rigidbody2D _rb2d;
        private Attributes _attributesController;
        private SpriteController _spriteController;
        private Inventory _inventory;
        private CharacterStatMenu _characterStatMenu;

        public Rigidbody2D Rb2d { get => _rb2d; private set => _rb2d = value; }
        public Attributes Attributes { get => _attributesController; private set => _attributesController = value; }
        public SpriteController SpriteController { get => _spriteController; private set => _spriteController = value; }
        public Inventory Inventory { get => _inventory; private set => _inventory = value; }
        public CharacterStatMenu CharacterStatMenu { get => _characterStatMenu; set => _characterStatMenu = value; }

        [Header("Movement Debug")]
        [Range(-1, 1)] private float _horizontal;
        [Range(-1, 1)] private float _vertical;
        private Vector2 _velocity;
        private bool _closeToTarget;
        protected float Horizontal { get => _horizontal; set { _horizontal = value > 1 ? 1 : (value < -1 ? -1 : value); } }
        protected float Vertical { get => _vertical; set { _vertical = value > 1 ? 1 : (value < -1 ? -1 : value); } }
        protected Vector2 Velocity { get => _velocity; set => _velocity = value; }
        protected bool CloseToTarget { get => _closeToTarget; set => _closeToTarget = value; }



        [Header("Other Variables")]
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Transform _targetAttackPoint;

        private GameObject _target;
        private float _rangeFromTarget;


        public LayerMask EnemyLayer { get => _enemyLayer; set => _enemyLayer = value; }
        public Transform TargetAttackPoint { get => _targetAttackPoint; set => _targetAttackPoint = value; }
        public GameObject Target
        {
            get => _target; set
            {
                _target = value;
                OnChangeTarget?.Invoke(value);
            }
        }
        public float RangeFromTarget { get => _rangeFromTarget; set => _rangeFromTarget = value; }

        Coroutine _windupCoroutine;
        Coroutine _attackSpeedCoroutine;
        public Coroutine WindupCoroutine { get => _windupCoroutine; set => _windupCoroutine = value; }
        public Coroutine AttackSpeedCoroutine { get => _attackSpeedCoroutine; set => _attackSpeedCoroutine = value; }

        // ACTIONS
        public Action<float, float> OnTakeDamageTrigger;
        public Action<GameObject> OnChangeTarget;


        public  new virtual void OnValidate()
        {
            base.OnValidate();
            SetupRequireds();
        }
        public new virtual void OnEnable()
        {
            base.OnEnable();
            SetupRequireds();
            OnTakeDamageTrigger += OnTakeDamage;

            Inventory.OnAddItem += Attributes.AddItemHandler;
            Attributes.OnAttackSpeedChange += AttackSpeedChangeHandler;
            Attributes.OnHealthChange += HealthChangeHandler;
            Attributes.OnAttackRangeChange += AttackRangeChangeHandler;

        }

        public virtual void OnDisable()
        {
            OnTakeDamageTrigger -= OnTakeDamage;

            Inventory.OnAddItem -= Attributes.AddItemHandler;
            Attributes.OnAttackSpeedChange -= AttackSpeedChangeHandler;
            Attributes.OnHealthChange -= HealthChangeHandler;
            Attributes.OnAttackRangeChange -= AttackRangeChangeHandler;
        }


        private void AttackSpeedChangeHandler(float value) => SpriteController.SetAttackSpeed(value);

        public Action<bool> BeforeDieTrigger;

        public virtual void HealthChangeHandler(float value)
        {
            // Debug.Log("Setting Health value: " + value.ToString());
            if (!IsAlive())
            {
                BeforeDieTrigger?.Invoke(IsAlive());
                Destroy(gameObject);
            }
        }


        private void AttackRangeChangeHandler(float value)
        {
            // Debug.Log("Setting AttackRange value: " + value.ToString());

        }
        private void Awake()
        {
            CloseToTarget = false;
        }

        public virtual void Start()
        {

            if (Attributes.BaseMoveSpeed == 0) throw new System.Exception("Velocidade do player não pode ser zero!");
            if (Attributes.AttackSpeed == 0) throw new System.Exception("Cooldown para ataque do player não pode ser zero!");
            Attributes.SetupAttackSpeed();
            Attributes.SetupHealth();
            RangeFromTarget = 0;
        }

        void FixedUpdate()
        {
            UpdateMovementAnimation();
            UpdateMovement();
        }

        private void SetupRequireds()
        {
            Rb2d = GetComponent<Rigidbody2D>();
            if (!Rb2d)
            {
                Rb2d = gameObject.AddComponent<Rigidbody2D>();
                Rb2d.gravityScale = 0;
                Rb2d.mass = 999;
                Rb2d.freezeRotation = true;
                Rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            Attributes = GetComponent<Attributes>();

            SpriteController = GetComponentInChildren<SpriteController>();
            if (!Attributes) throw new System.Exception("Necessário incluir SpriteController na Sprite para trocar o lado quando precisar!");

            Inventory = GetComponent<Inventory>();
            Inventory.PlayerID = ID;


        }
        public virtual void UpdateMovementAnimation()
        {
            TriggerMovementAnim();
            UpdateFlipCharacter();
        }

        public virtual void TriggerMovementAnim()
        {
            SpriteController.TriggerMovementAnim(Horizontal != 0 || Vertical != 0);
        }
        public virtual void UpdateFlipCharacter()
        {
            SpriteController.UpdateFlipCharacter(Horizontal);
        }

        private void UpdateMovement()
        {
            if (Horizontal != 0 && Vertical != 0) // Check for diagonal movement
            {
                // limit movement speed diagonally, so you move at 70% speed
                Horizontal *= MOVE_LIMITER;
                Vertical *= MOVE_LIMITER;
            }
            // Debug.Log(Horizontal * MoveSpeed + "   " + Vertical * MoveSpeed);

            Rb2d.velocity = new Vector2(Horizontal * Attributes.BaseMoveSpeed, Vertical * Attributes.BaseMoveSpeed);
            //FOR DEBUG
            Velocity = Rb2d.velocity;
        }

        public virtual void PerformAttack()
        {
            if (Attributes.CanAttack)
            {
                // Play Attack Animation
                SpriteController.TriggerAttackAnim();

                // Start a contagem de tempo para atacar novamente
                AttackSpeedCoroutine = StartCoroutine(Attributes.IAttackSpeedDelay());
            }
        }



        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(TargetAttackPoint.position, Attributes.BaseAttackRange);
        }

        public virtual float TakeDamage(float physicalDamage, float magicDamage)
        {
            float totalDamage = AtributesCalcs.CalcDamageTaken(Attributes.Health, physicalDamage, magicDamage, Attributes.BaseArmorResist, Attributes.BaseMagicResist);
            Attributes.Health -= totalDamage;
            return totalDamage;
        }

        public virtual void OnTakeDamage(float physicalDamage, float magicDamage)
        {

            bool dodged = Attributes.DodgeTrigger();
            float totalDamage = 0;
            if (!dodged)
            {
                totalDamage = TakeDamage(physicalDamage, magicDamage);
                DebugLog(gameObject.name + " TAKE DAMAGE OF: " + totalDamage);
                GameManagerController.PlayHitParticle(GameAssets.i.psPlainHit, transform.position);
                GameManagerController.PlayHitParticle(GameAssets.i.psBloodHit, transform.position);
            }

            GameManagerController.DamagePopup(transform.position, dodged ? "Bloqueou" : totalDamage.ToString("F2"), false);


        }

        public void AttackDamage(Character target)
        {
            if (!CloseToTarget)
            {
                GameManagerController.DamagePopup(target.transform.position, "Esquivou", false);
            }
            else if (target)
            {
                target.Target = this.gameObject;
                target.OnTakeDamageTrigger(Attributes.BasePhysicalDamage, Attributes.BaseMagicDamage);
            }
        }


        public bool IsAlive()
        {
            return Attributes.Health > 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ICollectable collectable = other.GetComponent<ICollectable>();
            if (collectable != null) collectable.Collect(ID);
        }


        public void DebugLog(string message)
        {
            if (!_DEBUG) return;
            Debug.Log(message);
        }


    }
}
