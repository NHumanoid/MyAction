using UnityEngine;

namespace MyAction 
{
    public class Bullet : MonoBehaviour
    {
        public int damage; // Damage value

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                Destroy(gameObject, 3);
            }
            else if (collision.gameObject.tag == "Wall") 
            {
                Destroy(gameObject);
            }
        }
    }

}
