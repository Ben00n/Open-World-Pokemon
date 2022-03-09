using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TxtLookAtCamera : MonoBehaviour
{
    public TextMeshPro thisText;

    private void LateUpdate()
    {
        thisText.transform.LookAt(Camera.main.transform);
        thisText.transform.Rotate(0, 180, 0);
    }
}
