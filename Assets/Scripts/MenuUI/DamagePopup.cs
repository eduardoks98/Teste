using TMPro;
using UnityEngine;

namespace Assets.Scripts.Uteis.Prefabs
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private float disappearTimeMax = .4f;
        [SerializeField] private int textCrit = 8;
        [SerializeField] private int textHit = 6;
        [SerializeField] private Color colorCrit;
        [SerializeField] private Color colorHit;

        private static int sortingOrder;
        private float disappearTimer;

        private TextMeshPro textMesh;
        private Color textColor;
        private Vector3 moveVector;


        private void Awake()
        {
            textMesh = transform.GetComponent<TextMeshPro>();
        }

        public void Setup(string info, bool isCriticalHit = false)
        {
            textMesh.SetText(info);
            textMesh.fontSize = isCriticalHit ? textCrit : textHit;
            textColor = isCriticalHit ? colorCrit : colorHit;
            textMesh.faceColor = textColor;
            textMesh.color = textColor;
            disappearTimer = disappearTimeMax;
            moveVector = new Vector3(1f, 5f) * 5f;
            sortingOrder++;
            textMesh.sortingOrder = sortingOrder;
        }

        private void FixedUpdate()
        {
            transform.position += moveVector * Time.fixedDeltaTime;
            moveVector -= moveVector * 8f * Time.fixedDeltaTime;

            if (disappearTimer > disappearTimeMax * .5f)
            {
                float increaseScaleAmount = 1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.fixedDeltaTime;
            }
            else
            {
                float decreaseScaleAmount = 1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.fixedDeltaTime;
            }
            disappearTimer -= Time.fixedDeltaTime;
            if (disappearTimer < 0)
            {
                float disappearSpeed = 5f;
                textColor.a -= disappearSpeed * Time.fixedDeltaTime;
                textMesh.color = textColor;
                if (textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}