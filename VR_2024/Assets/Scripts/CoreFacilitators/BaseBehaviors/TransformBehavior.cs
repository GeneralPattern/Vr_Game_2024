using Unity.VisualScripting;
using UnityEngine;

public class TransformBehavior : MonoBehaviour
{
    public bool setToStartTransform;
    
    public Vector3 setPosition;
    public Quaternion setRotation;
    
    private Transform _transform;
    private Vector3 _startTransformPosition;
    private Quaternion _startTransformRotation;
    
    private void Start()
    {
        _transform = transform;
        _startTransformPosition = _transform.position;
        _startTransformRotation = _transform.rotation;
    }
    
    public void SetPosition() { transform.position = setPosition; }
    public void SetPosition(Vector3 newPosition) { transform.position = newPosition; }
    public void SetPosition(Vector3Data newPosition) { transform.position = newPosition.value; }
    public void SetPosition(CharacterData data) { transform.position = data.spawnPosition; }
    
    public void SetRotation() { transform.rotation = setRotation; }
    public void SetRotation(Vector3 newPosition) { transform.position = newPosition; }
    public void SetRotation(Vector3Data newPosition) { transform.position = newPosition.value; }
    
    public void ResetPosition() { transform.position = _startTransformPosition; }
    public void ResetRotation() { transform.rotation = _startTransformRotation; }

    public void ResetTransform()
    {
        var thisTransform = transform;
        thisTransform.position = GetResetPosition();
        thisTransform.rotation = GetResetRotation();
    }

    public Vector3 GetPosition() { return transform.position; }
    private Vector3 GetResetPosition() { return setToStartTransform ? _startTransformPosition : setPosition; }
    private Quaternion GetResetRotation() { return setToStartTransform ? _startTransformRotation : setRotation; }
}
