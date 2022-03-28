using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningPointGizmo : MonoBehaviour
{
    [SerializeField] Color colour;

    private void OnDrawGizmos()
    {
        Gizmos.color = colour;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
