using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent initializeSceneEvent, startGameEvent, gameOverEvent, restartGameEvent;
    
    private void Start()
    {
        InitializeScene();
    }
    
    public void InitializeScene()
    {
        initializeSceneEvent.Invoke();
    }

    public void StartGame()
    {
        startGameEvent.Invoke();
    }

    public void GameOver()
    {
        gameOverEvent.Invoke();
    }

    public void RestartGame()
    {
        restartGameEvent.Invoke();
    }
}
