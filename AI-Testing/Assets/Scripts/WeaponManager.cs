using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject weaponsPrefab;
    public Transform rightHandTransform;
    public Transform leftHandTransform;

    private GameObject equippedWeapons;
    private Transform swordTransform;
    private Transform shieldTransform;

    void Start()
    {
        Debug.Log("WeaponManager Start method called");
        EquipWeapons();
    }

    void EquipWeapons()
    {
        if (weaponsPrefab != null && rightHandTransform != null && leftHandTransform != null)
        {
            equippedWeapons = Instantiate(weaponsPrefab, transform);
            swordTransform = equippedWeapons.transform.Find("Sword");
            shieldTransform = equippedWeapons.transform.Find("Shield");

            if (swordTransform != null && shieldTransform != null)
            {
                swordTransform.SetParent(rightHandTransform, false);
                shieldTransform.SetParent(leftHandTransform, false);

                // Adjust positions, rotations, and scales
                AdjustWeaponTransforms();

                Debug.Log("Weapons equipped successfully");
            }
            else
            {
                Debug.LogError("Sword or Shield not found in the weapons prefab!");
            }
        }
        else
        {
            Debug.LogError("Weapons prefab or hand transforms are missing!");
        }
    }

    void Update()
    {
        // Continuously adjust weapon transforms to maintain positions
        AdjustWeaponTransforms();
    }

    void AdjustWeaponTransforms()
    {
        if (swordTransform != null && shieldTransform != null)
        {
            // Adjust sword
            swordTransform.localPosition = new Vector3(0.05f, -0.05f, -0.1f); // Adjust these values as needed
            swordTransform.localRotation = Quaternion.Euler(-40, 0, 0); // Adjust these values as needed
            swordTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f); // Adjust scale as needed

            // Adjust shield
            shieldTransform.localPosition = Vector3.zero;
            shieldTransform.localRotation = Quaternion.Euler(0, 90, 180); // Adjust these values as needed
            shieldTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Adjust scale as needed
        }
    }
}