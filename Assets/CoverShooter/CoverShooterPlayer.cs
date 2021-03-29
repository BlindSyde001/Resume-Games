using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoverShooterPlayer : MonoBehaviour
{
    // VARIABLES
    #region PLAYER VARIABLES
    private Camera cam;
    private CharacterController cc;
    private Transform coverToAttach;

    public GameObject bullet;
    public Transform gunpoint;
    public Transform gun;

    public float rotationSpeed = 2f;
    public float smoothSpeed = 2f;
    public float smoothFactor = 1f;

    public TextMeshProUGUI ammoCounter;
    public int currentAmmo = 40;
    public int maxAmmo = 40;
    public float fireRate;
    private float timeBetweenShots;
    private bool _lock = false;

    private float speed = 8f;
    private float currentMoveH;
    private float currentMoveV;

    private Vector3 camOffSet;
    public Vector3 moveDirection;

    private bool lAim;
    private Transform currentShoulder;
    public Transform rShoulder;
    public Transform lShoulder;

    public float CamAdjustX;
    public float CamAdjustY;
    #endregion
    private float enemySpawnTime = 0;
    public List<Transform> spawnLocations;
    public GameObject enemyToSpawn;
    public Collider[] coversInRange;

    public int hitCounter;
    public TextMeshProUGUI hitCounterUI;

    private bool _MoveLock;

    // UPDATES
    private void Start()
    {
        Cursor.visible = false;
        cam = FindObjectOfType<Camera>();
        cc = this.GetComponent<CharacterController>();
        camOffSet = cam.transform.position - transform.position;
    }
    private void Update()
    {
        hitCounterUI.text = hitCounter.ToString();
        enemySpawnTime += Time.deltaTime;
        if (enemySpawnTime >= 2.5f)
        {
            SpawnEnemy();
            enemySpawnTime = 0;
        }
        ammoCounter.text = currentAmmo + " / " + maxAmmo;
        ShootGun();
    }
    private void FixedUpdate()
    {
        // Move When Pressing button down
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            currentMoveH = Input.GetAxisRaw("Horizontal");
            currentMoveV = Input.GetAxisRaw("Vertical");
        } // Stop moving
        else
        {
            currentMoveH = 0;
            currentMoveV = 0;
        }
        if (Input.GetKey(KeyCode.E))
        {
            TakeCover();
        }
        if (_MoveLock)
            return;
        PlayerMovement();
    }
    private void LateUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 30, 0.05f);
        } else 
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, 0.05f);
        }
        // Spin Player using Camera's Y rotation
        transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);

        // Set cam behind a shoulder and spin cam
        currentShoulder = lAim ? lShoulder : rShoulder;
        thirdPersonCameraY();

    }

    // METHODS

    #region PLAYER METHODS
    private void thirdPersonCameraY()
    {
        // Create Rotations around the Y Axis and X Axis
        Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y"), Vector3.right);
        Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up);

        // Create the offset of the rotation and position into a new position for camera
        camOffSet = camTurnAngleX * camTurnAngleY * camOffSet;
        Vector3 newPos = currentShoulder.position + camOffSet;

        // Do the Movement, Look at the Player Character
        cam.transform.position = Vector3.Slerp(cam.transform.position, newPos, smoothFactor);
        cam.transform.LookAt(currentShoulder);

        // Point Direction of Gun to the front
        gun.transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x + CamAdjustX,
                                                  cam.transform.rotation.eulerAngles.y + CamAdjustY,
                                                  0);
    }
    private void PlayerMovement()
    {
        moveDirection = new Vector3(currentMoveH, 0 , currentMoveV);
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);        // Direction is camera facing
        moveDirection.y = 0f;
        moveDirection.Normalize();        // Number from Input, normalized to -1, 1 so you don't accelerate to infinity

        // Move from Input
        cc.Move(moveDirection * speed * Time.deltaTime);
    }
    private void TakeCover()
    {
        // Find the covers in range
       coversInRange = Physics.OverlapSphere(transform.position, 5f);
       float closestCover = 0;

        //  Find the nearest cover to player
        for(int i = 0; i < coversInRange.Length; i++)
        {
            if (coversInRange[i].CompareTag("Cover"))
            {
                float currentCover = Vector3.Distance(transform.position, coversInRange[i].transform.position);
                if (currentCover < closestCover || closestCover == 0)
                {
                    closestCover = currentCover;
                    coverToAttach = coversInRange[i].transform;
                }
            }
        }
        // Attach to the cover
        transform.position = Vector3.Lerp(transform.position,
                                          coverToAttach.GetComponentInChildren<Collider>().gameObject.transform.position,
                                          0.05f);
        // Enable movement within cover
        // Move out of cover
        // Swap from one cover to the next
    }
    private void ShootGun()
    {
        // Reload when clip empty
        if (currentAmmo <= 0 && !_lock)
        {
            _lock = true;
            StartCoroutine(Reload());
        }
        
        // Time Between Shots from Fire Rate
        timeBetweenShots += Time.deltaTime; 

        // Shoot Bullet
        if(Input.GetMouseButton(0) && timeBetweenShots >= fireRate && currentAmmo > 0)
        {
            GameObject intBullet = Instantiate<GameObject>(bullet, gunpoint.position, gunpoint.rotation);
            //Rigidbody intBulletRB = intBullet.GetComponent<Rigidbody>();
            //intBullet.GetComponent<Rigidbody>().AddForce(gunpoint.forward * 100, ForceMode.Impulse);
            timeBetweenShots = 0;
            currentAmmo--;
        }

    }
    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(3f);
        currentAmmo = maxAmmo;
        _lock = false;
    }
    #endregion

    private void SpawnEnemy()
    {
        Transform a = spawnLocations[Random.Range(0, spawnLocations.Count)];
        Instantiate<GameObject>(enemyToSpawn, a.position, a.rotation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
