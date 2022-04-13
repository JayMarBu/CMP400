using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningPointGizmo : MonoBehaviour
{
    [SerializeField] public Color colour;
    [SerializeField] bool isArrow = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = colour;
        if(isArrow)
        {
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
