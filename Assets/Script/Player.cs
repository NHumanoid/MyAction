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
        public int ammo; // 총알
        public int coin; // 코인
        public int health; // 체력
        public int hasGrenades; // 수류탄

        public int maxHealth; // 최대 체력
        public int maxHasGrenades; // 최대 수류탄
        public int maxAmmo; // 최대 총알
        public int maxCoin; // 최대 코인

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
            // GetComponentInChildren : 자식 오브젝트에서 컴포넌트를 찾음
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
            hAxis = Input.GetAxisRaw("Horizontal"); // 가로축 입력
            vAxis = Input.GetAxisRaw("Vertical"); // 세로축 입력
            wDown = Input.GetButton("Walk"); // 걷기 버튼 입력
            jDown = Input.GetButtonDown("Jump"); // 점프 버튼 입력
            fDown = Input.GetButton("Fire1"); // 공격 버튼 입력
            iDown = Input.GetButtonDown("Interation"); // 점프 버튼 입력
            sDown1 = Input.GetButtonDown("Swap1"); // 무기 교체 버튼 입력
            sDown2 = Input.GetButtonDown("Swap2"); // 무기 교체 버튼 입력
            sDown3 = Input.GetButtonDown("Swap3"); // 무기 교체 버튼 입력
        }

        void Move()
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 이동 벡터

            if (isDodge) { moveVec = dodgeVec; }
            if (isSwap || !isFireReady) { moveVec = Vector3.zero; }
            transform.position += moveVec * speed * (wDown ? 0.5f : 1f) * Time.deltaTime; // 이동
            // 먼저 SetBool을 통해 애니메이션 파라미터를 설정
            anim.SetBool("IsRun", moveVec != Vector3.zero); // 이동 애니메이션 재생
            // 멈춰있거나 움직이지 않을 때 애니메이션 멈춤
            anim.SetBool("IsWalk", wDown); // 걷기 애니메이션 재생
        }

        void Turn()
        {
            // LookAt : 오브젝트가 특정 위치를 바라보도록 설정
            transform.LookAt(transform.position + moveVec); // 바라보는 방향 설정
        }

        void Jump()
        {
            if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
            {
                rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // 점프 힘 추가\
                anim.SetBool("IsJump", true);
                anim.SetTrigger("DoJump");
                isJump = true; // 점프 애니메이션 재생
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

        void Dodge() //회피관련 함수입니다.
        {
            if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
            {
                dodgeVec = moveVec;
                speed *= 2;
                anim.SetTrigger("DoDodge");
                isDodge = true; // 점프 애니메이션 재생

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


            int weaponIndex = -1; // 무기 인덱스 초기화
            if (sDown1) weaponIndex = 0;
            if (sDown2) weaponIndex = 1;
            if (sDown3) weaponIndex = 2;

            if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge) 
            {
                if (equipWeapon != null) { equipWeapon.gameObject.SetActive(false); } // 유효하지 않은 인덱스면 종료, 현재 무기 비활성화
                equipWeaponIndex = weaponIndex;
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true); // 현재 무기 비활성화

                anim.SetTrigger("DoSwap");
                isSwap = true; // 무기 교체 애니메이션 재생
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
                    Debug.Log("무기 획득");
                    Item item = nearObject.GetComponent<Item>();
                    int weaponIndex = item.value; // 무기 인덱스 계산
                    hasWeapons[weaponIndex] = true; // 무기 획득

                    Destroy(nearObject); // 아이템 오브젝트 제거
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Floor")
            {
                anim.SetBool("IsJump", false); // 땅에 닿으면 점프 애니메이션 해제
                isJump = false; // 땅에 닿으면 점프 상태 해제
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
                Destroy(other.gameObject); // 아이템 오브젝트 제거
            }
        }
        void OnTriggerStay(Collider other)
        {
            if (other.tag == "Weapon") { nearObject = other.gameObject; print("들어왔음"); }
            
        }
        void OnTriggerExit(Collider other) 
        {
            if (other.tag == "Weapon") { nearObject = null; print("나갔음"); }
        }
    }
}
