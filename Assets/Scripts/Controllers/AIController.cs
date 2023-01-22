using System;
using Assets.Scripts.Core;
using Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(AIPath))]
    [RequireComponent(typeof(AIDestinationSetter))]
    [RequireComponent(typeof(DynamicGridObstacle))]
    public class AIController : Character
    {


        private Seeker _seeker;
        private AIPath _aiPath;
        private AIDestinationSetter _aiDestinationSetter;

        private GameObject _retreatTarget;

        protected Seeker Seeker { get => _seeker; set => _seeker = value; }
        protected AIPath AiPath { get => _aiPath; set => _aiPath = value; }
        protected AIDestinationSetter AiDestinationSetter { get => _aiDestinationSetter; set => _aiDestinationSetter = value; }
        protected GameObject RetreatTarget { get => _retreatTarget; set => _retreatTarget = value; }

        private void Awake()
        {
            AiDestinationSetter = GetComponent<AIDestinationSetter>();
            AiPath = GetComponent<AIPath>();
            Seeker = GetComponent<Seeker>();

            if (!Seeker) throw new System.Exception("Seeker não setado para o character");
            // Ignora colisao com terreno
            Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Enviroment"), true);

        }

        public override void Start()
        {
            base.Start();
            AiPath.maxSpeed = Attributes.BaseMoveSpeed;
            AiPath.maxAcceleration = Attributes.BaseMoveSpeed;

            RetreatTarget = new GameObject();
            RetreatTarget.transform.position = transform.position;
            RetreatTarget.name = "TargetFrom: " + gameObject.name;
        }

        public void BeforeDieTriggerHandler(bool alive)
        {
            if (!alive)
            {
                GameManagerController.PlayHitParticle(GameAssets.i.psSoulDie, transform.position);
                Destroy(RetreatTarget);
            }
        }

        public override void OnEnable()
        {
            BeforeDieTrigger += BeforeDieTriggerHandler;
            base.OnEnable();
            SpriteController.OnAttackTrigger += AttackTriggerHandler;
        }

        public override void OnDisable()
        {
            SpriteController.OnAttackTrigger -= AttackTriggerHandler;
            BeforeDieTrigger -= BeforeDieTriggerHandler;
            base.OnDisable();
        }


        private void Update()
        {

            // if (!Target) Target = GameManagerController.getClosestTarget(this.ID, transform, Attributes.BaseFieldView, EnemyLayer);

            // if (Target) AiDestinationSetter.target = Target.transform;

            ATargetChase();

        }

        public void ATargetChase()
        {
            float startAttackOnRange = Attributes.IsRanged ? (Attributes.BaseAttackRange / 2) : (Attributes.BaseAttackRange / 1.2f);
            if (!Target)
            {
                // Procura o alvo mais proximo
                Target = GameManagerController.getClosestTarget(this.ID, transform, Attributes.BaseFieldView, EnemyLayer);
                AiDestinationSetter.target = Target ? Target.transform : null;
                if (!Target)
                {
                    CloseToTarget = false;
                    Seeker.OnDestroy();
                }

                Horizontal = 0;
                Vertical = 0;

                // Fica idle
                SpriteController.TriggerMovementAnim(false);
                return;
            }
            else
            {
                // Atualiza a distancia ate o alvo
                RangeFromTarget = Vector3.Distance(Target.transform.position, transform.position);
            }

            // Persegue o inimigo 
            if (RangeFromTarget > Attributes.BaseAttackRange)
            {
                CloseToTarget = false;

                // Gives a value between -1 and 1
                Horizontal = Attributes.CanMove ? Target.transform.position.x - transform.position.x : 0; // -1 is left
                Vertical = Attributes.CanMove ? Target.transform.position.y - transform.position.y : 0; // -1 is down

                AiPath.canMove = Attributes.CanMove;

                // AI IRA MOVER O PLAYER
                //AiDestinationSetter.target = RetreatTarget.transform;

                // AI IRA MOVER O PLAYER
                AiDestinationSetter.target = Attributes.CanMove ? Target.transform : null;
            }
            // Se o inimigo se aproximar muito começa a recuar
            else if (RangeFromTarget < (startAttackOnRange) && !Attributes.CanAttack)
            {
                Vector3 position = Attributes.CanMove ? Vector2.MoveTowards(transform.position, Target.transform.position, -(Attributes.BaseMoveSpeed)) : transform.position;

                // Gives a value between -1 and 1
                Horizontal = Attributes.CanMove ? (position.x - transform.position.x) : 0; // -1 is left
                Vertical = Attributes.CanMove ? (position.y - transform.position.y) : 0; // -1 is down

                RetreatTarget.transform.position = position;

                AiPath.canMove = Attributes.CanMove;

                // AI IRA MOVER O PLAYER
                AiDestinationSetter.target = Attributes.CanMove ? RetreatTarget.transform : null;

                // Se for possível se mover acionamos a animação de movimento
                SpriteController.TriggerMovementAnim(Attributes.CanMove ? true : false);
            }
            else
            {
                // Chegamos perto o suficiente do inimigo para atacar
                CloseToTarget = true;

                AiPath.canMove = Attributes.CanMove;

                // Desativa animação de movimento
                SpriteController.TriggerMovementAnim(Attributes.CanMove);
                // Ataca o inimigo
                PerformAttack();
            }
        }

        public void BasicTargetChase()
        {
            float startAttackOnRange = Attributes.IsRanged ? (Attributes.BaseAttackRange / 2) : (Attributes.BaseAttackRange / 1.2f);
            if (!Target)
            {

                 DebugLog("SEM Target");
                // Procura o alvo mais proximo
                Target = GameManagerController.getClosestTarget(this.ID, transform, Attributes.BaseFieldView, EnemyLayer);
                if (Target) DebugLog("COM Target: " + Target.gameObject.name + " Procurando layer: " + EnemyLayer.value + " Current Layer:" + gameObject.layer);

                Horizontal = 0;
                Vertical = 0;
                // Fica idle
                SpriteController.TriggerMovementAnim(false);
                return;
            }
            else
            {
                 DebugLog("COM Target E CALCULANDO DISTANCE");

                // Atualiza a distancia ate o alvo
                RangeFromTarget = Vector3.Distance(Target.transform.position, transform.position);
            }

            // Persegue o inimigo 
            if (RangeFromTarget > Attributes.BaseAttackRange)
            {
                CloseToTarget = false;

                // Gives a value between -1 and 1
                Horizontal = Attributes.CanMove ? Target.transform.position.x - transform.position.x : 0; // -1 is left
                Vertical = Attributes.CanMove ? Target.transform.position.y - transform.position.y : 0; // -1 is down
            }
            // Se o inimigo se aproximar muito começa a recuar
            else if (RangeFromTarget < (startAttackOnRange) && !Attributes.CanAttack)
            {
                Vector3 position = Attributes.CanMove ? Vector2.MoveTowards(transform.position, Target.transform.position, -(Attributes.BaseMoveSpeed)) : transform.position;

                // Gives a value between -1 and 1
                Horizontal = Attributes.CanMove ? (position.x - transform.position.x) : 0; // -1 is left
                Vertical = Attributes.CanMove ? (position.y - transform.position.y) : 0; // -1 is down

                // Se for possível se mover acionamos a animação de movimento
                SpriteController.TriggerMovementAnim(Attributes.CanMove ? true : false);
            }
            else
            {
                // Chegamos perto o suficiente do inimigo para atacar
                CloseToTarget = true;
                // Desativa animação de movimento
                SpriteController.TriggerMovementAnim(false);
                // Ataca o inimigo
                PerformAttack();
            }
        }

        public override void UpdateFlipCharacter()
        {
            if (!Target) base.UpdateFlipCharacter();
            else
            {
                // Se o target estiver á direta do personagem e o mesmo não estiver chamamos o método para inverter a sprite de lado
                if (Target.transform.position.x > transform.position.x && !SpriteController.Flipped)
                {
                    SpriteController.FlipCharacter();
                }
                else if (Target.transform.position.x < transform.position.x && SpriteController.Flipped)
                {
                    SpriteController.FlipCharacter();
                }
            }

        }


        public void AttackTriggerHandler(bool value)
        {

            if (!Target) { return; }


            if (!Attributes.CanAttack)
            {

                // Se o personagem for ranged
                if (Attributes.IsRanged)
                {
                    // Procura arma do personagem
                    WeaponController weapon = GetComponentInChildren<WeaponController>();
                    if (weapon == null) throw new System.Exception("Arma não encontrada para o personagem: " + gameObject.name);

                    // Chama o métod para atirar, onde o mesmo irá chamar o AttackDamage();
                    weapon.Fire();
                }
                // Se não for ranged
                else
                {
                    // POSSIVEL FAZER OPÇÃO PARA ATACAR MAIS DE UM ALVO
                    // Ataca o alvo mais proximo
                    Character enemy = Target.GetComponent<Character>();
                    if (enemy != null) AttackDamage(enemy);
                }

                // Para por um curto periodo de tempo
                if (WindupCoroutine != null) StopCoroutine(WindupCoroutine);
                WindupCoroutine = StartCoroutine(Attributes.IWindupDelay());
            }
        }



        public override void PerformAttack()
        {
            if (Attributes.CanAttack)
            {


                // Executa a animação de ataque e inicia o cooldown do mesmo
                base.PerformAttack();
            }
        }

        public override void OnTakeDamage(float physicalDamage, float magicDamage)
        {


            //Reseta movimentação 
            if (WindupCoroutine != null) StopCoroutine(WindupCoroutine);
            WindupCoroutine = StartCoroutine(Attributes.IWindupDelay());

            // Leva o dano
            base.OnTakeDamage(physicalDamage, magicDamage);

        }


        private void OnDrawGizmos()
        {

            if (!TargetAttackPoint) throw new Exception("Character ranged e não foi setado targetAttackPoint: " + gameObject.name);
            Gizmos.DrawWireSphere(TargetAttackPoint.position, Attributes.BaseAttackRange);
        }
    }

}