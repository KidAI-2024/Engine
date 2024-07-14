/*
Purpose of the Script
This script manages the behavior of a pistol weapon in a Unity game. It handles shooting mechanics, reloading, ammo management, 
visual and audio effects, and interactions with enemy objects. 
The script includes cooldowns for shooting and reloading to ensure realistic weapon behavior and prevent rapid firing or reloading.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survival
{
    // This class handles the functionality of a pistol weapon in the game
    public class Pistol : MonoBehaviour
    {
        // Public Variables
        public int maxAmmoInMag = 10;       // Maximum ammo capacity in the magazine
        public int maxAmmoInStorage = 100;   // Maximum ammo capacity in the storage
        public float shootCooldown = 0.2f;  // Cooldown time between shots
        public float reloadCooldown = 0.5f;  // Cooldown time between reloads
        private float switchCooldown = 0.5f;  // Cooldown time between shots
        public float shootRange = 100f;     // Range of the raycast

        public ParticleSystem impactEffect; // Particle effect for impact

        public int currentAmmoInMag;       // Current ammo in the magazine
        public int currentAmmoInStorage;   // Current ammo in the storage
        public int damager; // Damage dealt by the pistol
        public bool canShoot = true;       // Flag to check if shooting is allowed
        public bool canSwitch = true;      // Flag to check if switching weapons is allowed
        private bool isReloading = false;   // Flag to check if reloading is in progress
        private float shootTimer;           // Timer for shoot cooldown

        public Transform cartridgeEjectionPoint; // Ejection point of the cartridge
        public GameObject cartridgePrefab; // Prefab of the cartridge
        public float cartridgeEjectionForce = 5f; // Force applied to the cartridge

        public Text ammoText; // UI Text to display current ammo in magazine
        public Text storageText; // UI Text to display current ammo in storage

        public Animator gun; // Animator component for the gun
        public ParticleSystem muzzleFlash; // Particle system for muzzle flash effect
        public GameObject muzzleFlashLight; // Light object for muzzle flash
        public AudioSource shoot; // Audio source for shooting sound

        // Initialization
        void Start()
        {
            currentAmmoInMag = maxAmmoInMag;
            currentAmmoInStorage = maxAmmoInStorage;
            canSwitch = true;
            muzzleFlashLight.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            // Clamp the ammo values to ensure they are within valid ranges
            currentAmmoInMag = Mathf.Clamp(currentAmmoInMag, 0, maxAmmoInMag);
            currentAmmoInStorage = Mathf.Clamp(currentAmmoInStorage, 0, maxAmmoInStorage);
            ammoText.text = "Current Ammo: " + currentAmmoInMag.ToString();
            storageText.text = "Current Storage: " + currentAmmoInStorage.ToString();

            // Check for shoot input
            if (Input.GetButtonDown("Fire1") && canShoot && !isReloading)
            {
                switchCooldown = shootCooldown;
                Shoot();
            }

            // Check for reload input
            if (Input.GetKeyDown(KeyCode.R))
            {
                switchCooldown = reloadCooldown;
                Reload();
            }

            // Update the shoot timer
            if (shootTimer > 0f)
            {
                shootTimer -= Time.deltaTime;
            }
        }

        // Handle the shooting action
        void Shoot()
        {
            // Check if there is ammo in the magazine
            if (currentAmmoInMag > 0 && shootTimer <= 0f)
            {
                canSwitch = false;
                shoot.Play();
                muzzleFlash.Play();
                muzzleFlashLight.SetActive(true);
                gun.SetBool("shoot", true);

                // Perform the shoot action
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootRange))
                {
                    // Check if the hit object has the "Enemy" tag
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        // Get the EnemyHealth component from the hit object
                        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();

                        // Check if the enemy has the EnemyHealth component
                        if (enemyHealth != null)
                        {
                            // Apply damage to the enemy
                            enemyHealth.TakeDamage(damager); // Apply damage to the enemy
                        }
                    }

                    // Instantiate impact effect at the hit point
                    Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }

                // Instantiate the empty cartridge
                GameObject cartridge = Instantiate(cartridgePrefab, cartridgeEjectionPoint.position, cartridgeEjectionPoint.rotation);
                Rigidbody cartridgeRigidbody = cartridge.GetComponent<Rigidbody>();

                // Apply force to eject the cartridge
                cartridgeRigidbody.AddForce(cartridgeEjectionPoint.right * cartridgeEjectionForce, ForceMode.Impulse);

                StartCoroutine(endAnimations());
                StartCoroutine(endLight());
                StartCoroutine(canswitchshoot());

                switchCooldown -= Time.deltaTime;

                // Reduce ammo count
                currentAmmoInMag--;

                // Start the shoot cooldown
                shootTimer = shootCooldown;
            }
            else
            {
                // Out of ammo in the magazine or shoot on cooldown
                Debug.Log("Cannot shoot");
            }
        }

        // Handle the reloading action
        void Reload()
        {
            switchCooldown -= Time.deltaTime;
            // Check if already reloading or out of ammo in the storage
            if (isReloading || currentAmmoInStorage <= 0)
                return;

            // Calculate the number of bullets to reload
            int bulletsToReload = maxAmmoInMag - currentAmmoInMag;

            // Check if there is enough ammo in the storage for reloading
            if (bulletsToReload > 0)
            {
                gun.SetBool("reload", true);
                StartCoroutine(endAnimations());

                // Determine the actual number of bullets to reload based on available ammo
                int bulletsAvailable = Mathf.Min(bulletsToReload, currentAmmoInStorage);

                // Update ammo counts
                currentAmmoInMag += bulletsAvailable;
                currentAmmoInStorage -= bulletsAvailable;

                Debug.Log("Reloaded " + bulletsAvailable + " bullets");

                // Start the reload cooldown
                StartCoroutine(ReloadCooldown());
            }
            else
            {
                Debug.Log("Cannot reload");
            }
        }

        // Handle the cooldown for reloading
        IEnumerator ReloadCooldown()
        {
            isReloading = true;
            canShoot = false;
            canSwitch = false;

            yield return new WaitForSeconds(reloadCooldown);

            isReloading = false;
            canShoot = true;
            canSwitch = true;
        }

        // End shooting and reloading animations
        IEnumerator endAnimations()
        {
            yield return new WaitForSeconds(.1f);
            gun.SetBool("shoot", false);
            gun.SetBool("reload", false);
        }

        // Turn off the muzzle flash light
        IEnumerator endLight()
        {
            yield return new WaitForSeconds(.1f);
            muzzleFlashLight.SetActive(false);
        }

        // Allow switching and shooting after cooldown
        IEnumerator canswitchshoot()
        {
            yield return new WaitForSeconds(shootCooldown);
            canSwitch = true;
        }
    }
}
