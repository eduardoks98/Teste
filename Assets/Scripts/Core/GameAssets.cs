using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets i
        {
            get
            {
                if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
                return _i;
            }
        }
        [Header("Popups")]
        public Transform pfDamagePopup;

        public ParticleSystem psPlainHit;
        public ParticleSystem psBloodHit;
        public ParticleSystem psSoulDie;
        public GameObject arvore;

    }
}