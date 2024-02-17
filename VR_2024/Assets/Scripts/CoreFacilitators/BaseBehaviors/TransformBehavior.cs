using UnityEngine;

public class TransformBehavior : MonoBehaviour
{
    public void SetPosition(Vector3Data newPosition)
    {
        // Debug.Log("RESETTING CHARACTER TO: " +  newPosition.value);
        transform.position = newPosition.value;
    }
    
    public void SetPosition(CharacterData data)
    {
        // Debug.Log("RESETTING CHARACTER TO SPAWN POSITION: " + data.spawnPosition);
        transform.position = data.spawnPosition;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
