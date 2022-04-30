using TMPro;
using UnityEngine;

public class DamagePopup : InteractiveObject
{
    private TextMeshPro textMeshPro;
    private static Quaternion rotationPopUp;
    private Transform cam;

    public static DamagePopup Create(Vector3 pos, int damagePoints, GameObject gameObject)
    {
        GameObject damagePopupTransform = Instantiate(gameObject, pos, Quaternion.AngleAxis(90, Vector3.up));

        //rotationPopUp = Quaternion.LookRotation(gameObject.transform.position - Camera.main.transform.position, Vector3.up);

        //GameObject damagePopupTransform = Instantiate(gameObject, pos, rotationPopUp);
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damagePoints);
        return damagePopup;
    }

    public void Setup(int damageAmount)
    {
        textMeshPro.SetText(damageAmount.ToString());
        cam = Camera.main.transform;
    }

    private void Awake()
    {
        textMeshPro = transform.GetComponent<TextMeshPro>();
        
    }
    public override void Interaction()
    {
        
    }

    private void Update()
    {
        float moveWindows = 20f;
        transform.position += new Vector3(0, moveWindows, 0)*Time.deltaTime;
        transform.LookAt(transform.position+ cam.forward);
        Destroy(gameObject, 1f);
    }
}
