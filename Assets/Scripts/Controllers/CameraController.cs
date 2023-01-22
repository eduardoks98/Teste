using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {

        public float dampTime = 0.15f;
        private Vector3 velocity = Vector3.zero;
        public Transform target;
        // Update is called once per frame
        void Update()
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController) target = playerController.gameObject.transform;
            if (!target)
            {
                Character baseController = FindObjectOfType<Character>();
                if (baseController) target = baseController.gameObject.transform;
            }
            else
            {
                Vector3 destination = new Vector3(target.position.x, target.position.y, -300);
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            }



        }
    }
}