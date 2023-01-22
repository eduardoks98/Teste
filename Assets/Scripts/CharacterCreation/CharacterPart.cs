
using Assets.Scripts.Enums;
using UnityEngine;

namespace Scripts.CharacterCreation
{

    public class CharacterPart : MonoBehaviour
    {

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private BodyType _bodyType;

        public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
        public BodyType BodyType { get => _bodyType; set => _bodyType = value; }

        private void OnEnable()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();

        }
    }
}
