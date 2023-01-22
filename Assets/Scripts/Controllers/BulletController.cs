using Assets.Scripts.Core;
using System;
using System.Collections;
using UnityEngine;
namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BulletController : MonoBehaviour
    {

        public class OnFireEventArgs : EventArgs
        {
            public float physicalDamage;
            public float magicDamage;
        }

        [Header("Manual presets")]
        [SerializeField] private ParticleSystem _psHit;
        public ParticleSystem PsHit { get => _psHit; private set => _psHit = value; }

        [Header("Automatic")]
        [SerializeField] private Rigidbody2D _rb2d;
        public Rigidbody2D Rb2d { get => _rb2d; set => _rb2d = value; }

        public OnFireEventArgs fireArgs;

        public void OnValidate()
        {
            SetupRequireds();
        }
        public void OnEnable()
        {
            SetupRequireds();
        }
        // Start is called before the first frame update
        void Start()
        {

            StartCoroutine(DestroyBullet(gameObject));


        }
        private void SetupRequireds()
        {
            Rb2d = GetComponent<Rigidbody2D>();
            if (!Rb2d)
            {
                Rb2d = gameObject.AddComponent<Rigidbody2D>();
                Rb2d.gravityScale = 0;
                Rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            Character baseController = other.gameObject.GetComponent<Character>();

            // Efeito de colisão
            GameManagerController.PlayHitParticle(PsHit, transform.position);


            // Retorna e destroy caso nao seja
            if (!baseController)
            {
                Destroy(this.gameObject);
                return;
            }

            //Aplica o dano e destroy bullet
            AttackDamage(baseController);
            Destroy(this.gameObject);
        }

        IEnumerator DestroyBullet(GameObject bullet)
        {

            yield return new WaitForSeconds(5);
            Destroy(bullet);
        }

        public void AttackDamage(Character baseController)
        {
            baseController.OnTakeDamageTrigger(fireArgs.physicalDamage, fireArgs.magicDamage);
        }
    }
}
