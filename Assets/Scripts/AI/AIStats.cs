using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Stats")]
public class AIStats : ScriptableObject
{
    public int health = 100;
    public float speed = 10.0f;
    public float turnSpeed = 10.0f;

    public LayerMask attackLayerMask;
}
