using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace fireAttackVFXNameSpace
{


    public class fireBallScript : MonoBehaviour
    {
        private bool GotHit = false;

        public VisualEffect vfxPrefab;
        public GameObject objectToDisable;

        public Rigidbody rb;
        public float speed = 2f;
        public float maxSpeed = 3f;
        public float acceleration = 2f;
        public float rotationSpeed = 100f;

        [Header("Damage")]
        public float damage = 25f;
        public Transform dragonRespawnPoint;


        void Start()
        {
            rb = this.GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (speed < maxSpeed)
            {
                speed += acceleration * Time.fixedDeltaTime;
                speed = Mathf.Min(speed, maxSpeed);
            }

            rb.AddForce(transform.forward * speed);  // use forward instead of Vector3.right
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (GotHit) return;
            GotHit = true;

            // 1. Deal damage to player if hit
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);
                }
            }

            // 2. Play VFX
            if (vfxPrefab != null)
            {
                VisualEffect vfxInstance = Instantiate(vfxPrefab, collision.contacts[0].point, Quaternion.identity);
                vfxInstance.SendEvent("OnPlay");
                Destroy(vfxInstance.gameObject, 1f);
            }

            // 3. Destroy fireball
            Destroy(gameObject);
        }
    

    
    }
}
