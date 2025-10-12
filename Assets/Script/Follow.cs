using UnityEngine;

namespace MyAction 
{
    public class Follow : MonoBehaviour
    {
        public Transform target; // 따라갈 대상
        public Vector3 offset; // 오프셋
        private void Update()
        {
            transform.position = target.position + offset; // 대상의 위치에 오프셋을 더해 카메라 위치 설정
        }
    }
}

