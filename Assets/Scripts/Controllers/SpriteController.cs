using Assets.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(Animator))]
    public class SpriteController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector3 _flipSpriteOffset;
        [SerializeField] private bool _flipped;
        public Animator Animator { get => _animator; private set => _animator = value; }
        public Vector3 FlipSpriteOffset { get => _flipSpriteOffset; set => _flipSpriteOffset = value; }
        public bool Flipped { get => _flipped; set => _flipped = value; }

        [Header("Animator Variables")]
        [SerializeField] private bool _isMoving;
        public bool IsMoving { get => _isMoving; set => _isMoving = value; }
        public Action<bool> OnAttackTrigger;

        public virtual void AttackTrigger()
        {
            if (OnAttackTrigger != null)
                OnAttackTrigger(true);
        }

        public void OnValidate()
        {
            SetupRequireds();
        }
        public void OnEnable()
        {
            SetupRequireds();
        }

        private void SetupRequireds()
        {
            Animator = GetComponent<Animator>();
            // Base controller setado quando for instanciado pelo Base Controller do player/enemy
            if (!Animator) throw new System.Exception("Sem Animator, necessário para executar as animações do personagem");
        }
        public virtual void UpdateFlipCharacter(float horizontal)
        {
            // Debug.Log("UpdateFlipCharacter");

            if (horizontal > 0 && !Flipped) FlipCharacter();
            else if (horizontal < 0 && Flipped) FlipCharacter();
        }

        public void FlipCharacter()
        {
            Flipped = !Flipped;
            transform.localPosition = Flipped ? FlipSpriteOffset : Vector3.zero;

            Vector3 normalScale = transform.localScale;
            normalScale.x *= -1;
            transform.localScale = normalScale;
        }

        public void SetAttackSpeed(float value) => Animator.SetFloat(AnimatorParameterType.AttackSpeed, value);

        // Play Attack Animation
        public void TriggerAttackAnim() => Animator.SetTrigger(AnimatorParameterType.Attack);

        public void TriggerMovementAnim(bool value)
        {
            IsMoving = value;
            Animator.SetBool(AnimatorParameterType.IsMoving, value);
        }
    }
}
