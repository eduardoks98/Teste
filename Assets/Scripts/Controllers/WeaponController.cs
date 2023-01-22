using Assets.Scripts.Core;
using System.Collections;
using UnityEngine;
namespace Assets.Scripts.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Manual presets")]
        // Prefab da bala
        [SerializeField] private GameObject _pfBullet;
        // Posição que a bala sairá
        [SerializeField] private Transform _firePoint;
        public GameObject PfBullet { get => _pfBullet; private set => _pfBullet = value; }
        public Transform FirePoint { get => _firePoint; set => _firePoint = value; }

        [Header("Automatic")]
        [SerializeField] private Character _baseControllerReference;
        public Character BaseControllerReference { get => _baseControllerReference; set => _baseControllerReference = value; }


        private void Awake()
        {
            BaseControllerReference = GetComponentInParent<Character>();
            if (BaseControllerReference == null) throw new System.Exception("BaseController não encontrado para iniciar a weapon");
        }

        public void Fire()
        {
            if (BaseControllerReference.Target == null) return;
            // Instancia a bala
            GameObject bullet = Instantiate(PfBullet, FirePoint.position, FirePoint.rotation);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.fireArgs = new BulletController.OnFireEventArgs { physicalDamage = BaseControllerReference.Attributes.BasePhysicalDamage, magicDamage = BaseControllerReference.Attributes.BaseMagicDamage };
            // Adiciona a force para bala ir em direção ao inimigo
            bullet.GetComponent<Rigidbody2D>().AddForce(FirePoint.up * BaseControllerReference.Attributes.AttackSpeed * 50f, ForceMode2D.Impulse);
            // Ignora a colisao com o personagem que está atirando
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), BaseControllerReference.GetComponent<Collider2D>(), true);
            // Ignora a colisao com a layer do personagem
            Physics2D.IgnoreLayerCollision(bullet.layer, BaseControllerReference.gameObject.layer, true);
        }

        private void FixedUpdate()
        {
            if (BaseControllerReference.Target != null) AimDirection(BaseControllerReference.Target);
        }


        private void AimDirection(GameObject target)
        {
            float angle = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 180f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 9999f);
        }
        private void OnDrawGizmos()
        {
            // Desenha um circulo no local de onde sairá o tiro
            Gizmos.DrawSphere(FirePoint.position, .1f);
        }


    }
}
