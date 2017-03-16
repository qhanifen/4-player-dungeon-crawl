using UnityEngine;

public abstract class ProjectilePath : ScriptableObject
{
    public abstract Vector3 GetPosition(Vector3 position, Vector3 forwardVector, float time);
}
