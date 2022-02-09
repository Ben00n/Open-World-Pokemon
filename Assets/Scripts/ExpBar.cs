using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBar : MonoBehaviour
{
    [SerializeField] GameObject exp;

    public void SetEXP(float expNormalized)
    {
        exp.transform.localScale = new Vector3(expNormalized, 1f);
    }
}
