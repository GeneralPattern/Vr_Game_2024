using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct StringIdEvent
{
    public string id;
    public UnityEvent unityEvent;
}

public class WaitBehavior : MonoBehaviour
{
    public List<StringIdEvent> endWaitForSeconds;
    public List<StringIdEvent> endWaitForZero;
    public List<StringIdEvent> endWaitForFixedUpdate;

    private float _secondsToWait;
    private int _waitAmount;
    private IntData _intData;
    private WaitForSeconds _wfs = new WaitForSeconds(1);
    private WaitForFixedUpdate _wffu = new WaitForFixedUpdate();
    
    public void SetSecondsToWait(float seconds){ _secondsToWait = seconds; }
    public void SetWaitForZeroAmount(IntData amount){ _intData = amount; }

    public void StartWaitForSecondsEvent(string eventID)
    {
        if (_secondsToWait <= 0) return;
        StartCoroutine(WaitForSecondsEvent(_secondsToWait, eventID));
    }

    public void StartWaitForSecondsEvent(int seconds, string eventID)
    {
        StartCoroutine(WaitForSecondsEvent(seconds, eventID));
    }

    public void StartWaitForZeroIntDataEvent(string eventID)
    {
        if (_intData == null) return;
        StartCoroutine(WaitForZeroIntDataEvent(_intData, eventID));
    }

    public void StartWaitForZeroIntDataEvent(IntData data, string eventID)
    {
        StartCoroutine(WaitForZeroIntDataEvent(data, eventID));
    }

    public void StartWaitForFixedUpdateEvent(string eventID)
    {
        StartCoroutine(WaitForFixedUpdateEvent(eventID));
    }

    private IEnumerator WaitForSecondsEvent(float num, string eventID)
    {
        _secondsToWait = num;
        while (_secondsToWait > 0)
        {
            _secondsToWait--;
            yield return _wfs;
        }

        foreach (var StringIdEvent in endWaitForSeconds)
        {
            if (StringIdEvent.id == eventID)
            {
                StringIdEvent.unityEvent.Invoke();
                break;
            }
        }
    }

    private IEnumerator WaitForZeroIntDataEvent(IntData obj, string eventID)
    {
        _waitAmount = obj.value;
        while (_waitAmount > 0)
        {
            _waitAmount = obj.value;
            yield return _wffu;
        }

        foreach (var StringIdEvent in endWaitForZero)
        {
            if (StringIdEvent.id == eventID)
            {
                StringIdEvent.unityEvent.Invoke();
                break;
            }
        }
    }

    private IEnumerator WaitForFixedUpdateEvent(string eventID)
    {
        yield return _wffu;
        foreach (var StringIdEvent in endWaitForFixedUpdate)
        {
            if (StringIdEvent.id == eventID)
            {
                StringIdEvent.unityEvent.Invoke();
                break;
            }
        }
    }
}