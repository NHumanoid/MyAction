using UnityEngine;

namespace MyAction
{

    public class OrBit : MonoBehaviour
    {
        public Transform target;
        public float orbitSpeed = 10f;
        Vector3 offset;

        private void Start()
        {
            offset = transform.position - target.position;
        }

        private void Update()
        {
            transform.position = target.position + offset;
            transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
            offset = transform.position - target.position;
        }
    }
}

