using UnityEngine;

public class DamageSpawn: InteractiveObject
{
    [SerializeField]
    private Transform pfDamagePopup;

    public override void Interaction()
    {
        
    }

    private void Start()
    {
       Transform damagePopupTransform = Instantiate(pfDamagePopup, Vector3.zero, Quaternion.identity);
       DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
       damagePopup.Setup(100);

    }


}