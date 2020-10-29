using UnityEngine;

public class PaddyTest : MonoBehaviour
{
    private GameObject orb;

    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private int numberOfChildren = 40;
    [SerializeField] [Range (0.0f, 0.4f)] private float bottomTrim = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        var toRemoveFromBottom = (int)(numberOfChildren * bottomTrim);
        var list = Placement.GetPointsOnSphere(numberOfChildren + toRemoveFromBottom, 1.4f);
        list = Placement.RotatePoints(list);
        list = Placement.RemoveFromBottom(list, toRemoveFromBottom);

        orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orb.transform.position = new Vector3(0, 2f, 0);
        orb.transform.localScale = Vector3.one * 2f;

        foreach (var point in list) {
            var suborb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            suborb.transform.SetParent(orb.transform);
            suborb.transform.localPosition = point;
            suborb.transform.localScale = Vector3.one * 0.2f;
        }

    }

    private void Update() {
        orb.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

}
