using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float DestroyTime = 3;

    private void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
