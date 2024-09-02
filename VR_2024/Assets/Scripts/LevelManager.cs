using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelNumber;
    public Vector3 levelPosition;
    public bool Unlocked;

    private void Start()
    {
        levelPosition = transform.position;
    }

    public void LevelActive()
    {
        gameObject.SetActive(true);
    }
    
    public void ExitLevel()
    {
        transform.position = levelPosition;
        gameObject.SetActive(false);
    }
    
    public void UnlockLevel()
    {
        Unlocked = true;
    }
    
    public void LockLevel()
    {
        Unlocked = false;
    }
}