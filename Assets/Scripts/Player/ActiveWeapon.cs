using StarterAssets;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO startingWeaponSO;
    [SerializeField] CinemachineCamera playerFollowCamera;
    [SerializeField] Camera weaponCamrea;
    [SerializeField] GameObject zoomVignette;
    [SerializeField] TMP_Text ammoText;

    WeaponSO currentWaponSO;
    FirstPersonController firstPersonController;
    StarterAssetsInputs starterAssetsInputs;
    Animator animator;
    Weapon currentWeapon;

    const string SHOOT_STRING = "Shoot";

    float timeSinceLastShot = 0f;
    float defaultFOV;
    float defaultRotationSpeed;
    int currentAmmo;

    private void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        firstPersonController = GetComponentInParent<FirstPersonController>();
        animator = GetComponent<Animator>();
        defaultFOV = playerFollowCamera.Lens.FieldOfView;
        defaultRotationSpeed = firstPersonController.RotationSpeed; 
    }

    private void Start()
    {
        SwitchWeapon(startingWeaponSO);
        AdjustAmmo(currentWaponSO.MagazineSize);
    }

    void Update()
    {
        HandleShoot();
        HandleZoom();
    }

    public void AdjustAmmo(int amount)
    {
        currentAmmo += amount;
        if(currentAmmo > currentWaponSO.MagazineSize)
        {
            currentAmmo = currentWaponSO.MagazineSize;
        }
        ammoText.text = currentAmmo.ToString("D2");
    }

    void HandleShoot()
    {
        timeSinceLastShot += Time.deltaTime;
        if (!starterAssetsInputs.shoot) return;
        if(timeSinceLastShot >= currentWaponSO.FireRate && currentAmmo > 0)
        {
            currentWeapon.Shoot(currentWaponSO);
            animator.Play(SHOOT_STRING, 0, 0f);
            timeSinceLastShot = 0f;
            AdjustAmmo(-1);
        }
        if (!currentWaponSO.IsAutomatic)
        {
            starterAssetsInputs.ShootInput(false);
        }
    }    
    void HandleZoom()
    {
        if(!currentWaponSO.CanZoom) return;
        if (starterAssetsInputs.zoom)
        {
            playerFollowCamera.Lens.FieldOfView = currentWaponSO.ZoomAmount;
            weaponCamrea.fieldOfView = currentWaponSO.ZoomAmount;
            zoomVignette.SetActive(true);
            firstPersonController.ChangeRotationSpeed(currentWaponSO.ZoomRotationSpeed);
        }
        else 
        {
            playerFollowCamera.Lens.FieldOfView = defaultFOV;
            weaponCamrea.fieldOfView = defaultFOV;
            zoomVignette.SetActive(false);
            firstPersonController.ChangeRotationSpeed(defaultRotationSpeed);
        }
    }

    public void SwitchWeapon(WeaponSO weaponSO)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }
        Weapon newWeapon = Instantiate(weaponSO.WeaponPrefab, transform).GetComponent<Weapon>();
        currentWeapon = newWeapon;
        this.currentWaponSO = weaponSO;
        AdjustAmmo(currentWaponSO.MagazineSize);
    }

}
