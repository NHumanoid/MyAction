using UnityEngine;

namespace MyAction 
{
    public class Follow : MonoBehaviour
    {
        public Transform target; // ���� ���
        public Vector3 offset; // ������
        private void Update()
        {
            transform.position = target.position + offset; // ����� ��ġ�� �������� ���� ī�޶� ��ġ ����
        }
    }
}

