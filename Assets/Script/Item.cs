using UnityEngine;
namespace MyAction 
{
    public class Item : MonoBehaviour
    {
        public enum Type { Ammo, Coin, Grenade, Heart, Weapon }
        public Type type;
        public int value;

        private void Update()
        {
            // Rotate the item for a simple animation effect
            transform.Rotate(Vector3.up * 25 * Time.deltaTime);
        }
    }

}




