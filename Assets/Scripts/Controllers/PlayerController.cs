using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PlayerController : Character
    {
        private void Awake()
        {
            Physics2D.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Enemys"), true);

        }
        public override void HealthChangeHandler(float value)
        {
            // Debug.Log("Setting Health value: " + value.ToString());

        }
        public override void OnEnable()
        {
            SpriteController.OnAttackTrigger += AttackTriggerHandler;
            base.OnEnable();
            CharacterStatMenu = GameObject.FindObjectOfType<CharacterStatMenu>();
            CharacterStatMenu.setup(this);
        }

        public override void OnDisable()
        {
            SpriteController.OnAttackTrigger -= AttackTriggerHandler;
            base.OnDisable();
        }
        void Update()
        {

            if (Input.GetMouseButton(0))
            {
                PerformAttack();
            }
            Target = GameManagerController.getClosestTarget(this.ID, transform, Attributes.BaseFieldView, EnemyLayer);
            if (Target) RangeFromTarget = Vector3.Distance(Target.transform.position, transform.position);


            // Gives a value between -1 and 1
            Horizontal = Attributes.CanMove ? Input.GetAxisRaw("Horizontal") : 0; // -1 is left
            Vertical = Attributes.CanMove ? Input.GetAxisRaw("Vertical") : 0; // -1 is down

            if (RangeFromTarget > Attributes.BaseAttackRange)
            {
                CloseToTarget = false;
            }
            else
            {
                CloseToTarget = true;
            }
        }



        public override void UpdateMovementAnimation()
        {
            SpriteController.TriggerMovementAnim(Horizontal != 0 || Vertical != 0);

            // Posição do mouse em relação ao personagem
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x > transform.position.x && !SpriteController.Flipped) SpriteController.FlipCharacter();
            else if (mousePos.x < transform.position.x && SpriteController.Flipped) SpriteController.FlipCharacter();
        }


        public void AttackTriggerHandler(bool value)
        {
            if (!Attributes.CanAttack)
            {
                // POSSIVEL FAZER OPÇÃO PARA ATACAR MAIS DE UM ALVO OU APENAS UM UNICO
                // FAZER BOOLEAN QUE MARCA SE O PLAYER PODE OU NAO ATACAR MAIS DE UM ALVO
                // Debug.Log("Enemy Damaged");
                Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(TargetAttackPoint.position, Attributes.BaseAttackRange, EnemyLayer);
                foreach (Collider2D enemy in hitEnemys)
                {
                    Character enemyController = enemy.GetComponent<Character>();
                    if (enemyController != null)
                        enemyController.OnTakeDamageTrigger(Attributes.BasePhysicalDamage, Attributes.BaseMagicDamage);
                    // IDamageable isDamageable = enemy.GetComponent<IDamageable>();
                    // if (isDamageable != null) isDamageable.TakeDamage(AtributesController.BasePhysicalDamage, AtributesController.BaseMagicDamage);


                }
                WindupCoroutine = StartCoroutine(Attributes.IWindupDelay());
            }
        }



    }

}