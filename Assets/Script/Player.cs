using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyAction
{
    public class Player : MonoBehaviour
    {
        public float speed = 15.0f;

        public GameObject[] weapons;
        public bool[] hasWeapons;
        public GameObject[] grenades;
        public int ammo; // �Ѿ�
        public int coin; // ����
        public int health; // ü��
        public int hasGrenades; // ����ź

        public int maxHealth; // �ִ� ü��
        public int maxHasGrenades; // �ִ� ����ź
        public int maxAmmo; // �ִ� �Ѿ�
        public int maxCoin; // �ִ� ����

        private float hAxis;
        private float vAxis;

        private bool wDown;
        private bool jDown;
        private bool iDown;
        private bool fDown;

        bool sDown1;
        bool sDown2;
        bool sDown3;

        private bool isJump;
        private bool isDodge;
        private bool isSwap;
        private bool isFireReady = true;

        Vector3 moveVec;
        Vector3 dodgeVec;

        Rigidbody rigid;
        [SerializeField]
        GameObject nearObject;
        [SerializeField]
        Weapon equipWeapon;
        Animator anim;
        [SerializeField]
        private int equipWeaponIndex = -1;
        [SerializeField]
        float fireDelay;
        void Awake()
        {
            // GetComponentInChildren : �ڽ� ������Ʈ���� ������Ʈ�� ã��
            anim = GetComponentInChildren<Animator>();
            rigid = GetComponent<Rigidbody>();
        }
        void Update()
        {
            GetInput();
            Move();
            Turn();
            Jump();
            Dodge();
            Interation();
            Swap();
            Attack();
        }

        void GetInput() 
        {
            hAxis = Input.GetAxisRaw("Horizontal"); // ������ �Է�
            vAxis = Input.GetAxisRaw("Vertical"); // ������ �Է�
            wDown = Input.GetButton("Walk"); // �ȱ� ��ư �Է�
            jDown = Input.GetButtonDown("Jump"); // ���� ��ư �Է�
            fDown = Input.GetButton("Fire1"); // ���� ��ư �Է�
            iDown = Input.GetButtonDown("Interation"); // ���� ��ư �Է�
            sDown1 = Input.GetButtonDown("Swap1"); // ���� ��ü ��ư �Է�
            sDown2 = Input.GetButtonDown("Swap2"); // ���� ��ü ��ư �Է�
            sDown3 = Input.GetButtonDown("Swap3"); // ���� ��ü ��ư �Է�
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; // �̵� ����

            if (isDodge) { moveVec = dodgeVec; }
            if (isSwap || !isFireReady) { moveVec = Vector3.zero; }
            transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime; // �̵�
            // ���� SetBool�� ���� �ִϸ��̼� �Ķ���͸� ����
            anim.SetBool("IsRun", moveVec != Vector3.zero); // �̵� �ִϸ��̼� ���
            // �����ְų� �������� ���� �� �ִϸ��̼� ����
            anim.SetBool("IsWalk", wDown); // �ȱ� �ִϸ��̼� ���
        }

        void Turn()
        {
            // LookAt : ������Ʈ�� Ư�� ��ġ�� �ٶ󺸵��� ����
            transform.LookAt(transform.position + moveVec); // �ٶ󺸴� ���� ����
        }

        void Jump()
        {
            if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
            {
                rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // ���� �� �߰�\
                anim.SetBool("IsJump", true);
                anim.SetTrigger("DoJump");
                isJump = true; // ���� �ִϸ��̼� ���
            }
        }

        void Attack() 
        {
            if (equipWeapon == null)
                return;

            fireDelay += Time.deltaTime;
            isFireReady = equipWeapon.rate < fireDelay;

            if (fDown && isFireReady && !isDodge && !isSwap)
            {
                equipWeapon.Use();
                anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "DoSwing" : "DoShot");
                fireDelay = 0;
            }
        }

        void Dodge() //ȸ�ǰ��� �Լ��Դϴ�.
        {
            if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
            {
                dodgeVec = moveVec;
                speed *= 2;
                anim.SetTrigger("DoDodge");
                isDodge = true; // ���� �ִϸ��̼� ���

                Invoke("DodgeOut", 0.5f);
            }
        }

        void DodgeOut()
        {
            speed *= 0.5f;
            isDodge = false;
        }

        void Swap()
        {
            if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) { return; }
            if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) { return; }
            if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) { return; }


            int weaponIndex = -1; // ���� �ε��� �ʱ�ȭ
            if (sDown1) weaponIndex = 0;
            if (sDown2) weaponIndex = 1;
            if (sDown3) weaponIndex = 2;

            if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) 
            {
                if (equipWeapon != null) { equipWeapon.gameObject.SetActive(false); } // ��ȿ���� ���� �ε����� ����, ���� ���� ��Ȱ��ȭ
                equipWeaponIndex = weaponIndex;
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true); // ���� ���� ��Ȱ��ȭ

                anim.SetTrigger("DoSwap");
                isSwap = true; // ���� ��ü �ִϸ��̼� ���
                Invoke("SwapOut", 0.4f);
            }
        }

        void SwapOut()
        {
            isSwap = false;
        }

        void Interation()
        {
            if (iDown && nearObject != null && !isJump && !isDodge) 
            {
                if(nearObject.tag == "Weapon")
                {
                    Debug.Log("���� ȹ��");
                    Item item = nearObject.GetComponent<Item>();
                    int weaponIndex = item.value; // ���� �ε��� ���
                    hasWeapons[weaponIndex] = true; // ���� ȹ��

                    Destroy(nearObject); // ������ ������Ʈ ����
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                anim.SetBool("IsJump", false); // ���� ������ ���� �ִϸ��̼� ����
                isJump = false; // ���� ������ ���� ���� ����
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Item") 
            {
                Item item = other.GetComponent<Item>();
                switch (item.type)
                {
                    case Item.Type.Ammo:
                        ammo += item.value;
                        if (ammo > maxAmmo) { ammo = maxAmmo; }
                        break;
                    case Item.Type.Coin:
                        coin += item.value;
                        if (coin > maxCoin) { coin = maxCoin; }
                        break;
                    case Item.Type.Grenade:
                        grenades[hasGrenades].SetActive(true);
                        hasGrenades += item.value;
                        if (hasGrenades > maxHasGrenades) { hasGrenades = maxHasGrenades; }
                        break;
                    case Item.Type.Heart:
                        health += item.value;
                        if (health > maxHealth) { health = maxHealth; }
                        break;
                }
                Destroy(other.gameObject); // ������ ������Ʈ ����
            }
        }
        void OnTriggerStay(Collider other)
        {
            if (other.tag == "Weapon") { nearObject = other.gameObject; print("������"); }
            
        }
        void OnTriggerExit(Collider other) 
        {
            if (other.tag == "Weapon") { nearObject = null; print("������"); }
        }
    }
}
