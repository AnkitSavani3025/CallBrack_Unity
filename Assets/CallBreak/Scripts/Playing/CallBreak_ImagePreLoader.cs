using UnityEngine;

public class CallBreak_ImagePreLoader : MonoBehaviour
{
    void Update()
    {
        if (this.enabled)
            transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z - 5f));
    }
}
