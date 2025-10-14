using System.Collections;
using UnityEngine;

namespace MyAction
{
    public class Weapon : MonoBehaviour
    {
        public enum Type { Melee, Range };
        public Type type;// Weapon type
        public int damage; // Damage value
        public float rate; // Attack rate
        public BoxCollider meleeArea;
        public TrailRenderer trailEffect; // Swing effect

        public Transform bulletPos;
        public GameObject bullet;
        public Transform bulletCasePos;
        public GameObject bulletCase;
        public void Use()
        {
            if (type == Type.Melee) 
            { 
                StopCoroutine(Swing());
                StartCoroutine(Swing()); 
            }
            else if (type == Type.Range)
            {
                StartCoroutine(Shot());
            }
        }

        IEnumerator Swing() //코루틴 함수
        {
            yield return new WaitForSeconds(0.1f);
            meleeArea.enabled = true;
            trailEffect.enabled = true;

            yield return new WaitForSeconds(0.3f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(0.3f);
            trailEffect.enabled = false;
        }

        IEnumerator Shot()
        {
            GameObject intanBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody rigidBullet = intanBullet.GetComponent<Rigidbody>();
            rigidBullet.linearVelocity = bulletPos.forward * 50;

            yield return null;

            GameObject intanCase = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
            Rigidbody caseRigid = intanCase.GetComponent<Rigidbody>();
            Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + bulletCasePos.up * Random.Range(2, 3);
            caseRigid.AddForce(caseVec, ForceMode.Impulse);
            caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        }

    }
}
