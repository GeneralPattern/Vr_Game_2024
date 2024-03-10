using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitBehavior : MonoBehaviour
{

    [System.Serializable]
    public struct StringIdEvent
    {
        public string id;
        public UnityEvent onWaitFinished;
    }

    [System.Serializable]
    public struct SecondsAndStringIdEvent
    {
        public string id;
        public float seconds;
        public UnityEvent onWaitFinished;
    }

    [System.Serializable]
    public struct IntDataAndStringIdEvent
    {
        public string id;
        public IntData data;
        public UnityEvent onWaitFinished;
    }
    
    
    public List<SecondsAndStringIdEvent> endWaitForSeconds;
    public List<IntDataAndStringIdEvent> endWaitForZero;
    public List<StringIdEvent> endWaitForFixedUpdate;

    private float _secondsToWait;
    private int _waitAmount;
    private IntData _intData;
    private readonly WaitForSeconds _wfms = new WaitForSeconds(0.1f);
    private readonly WaitForSeconds _wfs = new WaitForSeconds(1);
    private readonly WaitForFixedUpdate _wffu = new WaitForFixedUpdate();

    public void StartWaitForSecondsEvent(string eventID)
    {
        var seconds = endWaitForSeconds.Find(x => x.id == eventID).seconds;
        StartCoroutine(WaitForSecondsEvent(seconds, eventID));
    }

    public void StartWaitForSecondsEvent(float seconds, string eventID)
    {
        StartCoroutine(WaitForSecondsEvent(seconds, eventID));
    }

    public void StartWaitForZeroIntDataEvent(string eventID)
    {
        var data = endWaitForZero.Find(x => x.id == eventID).data;
        StartCoroutine(WaitForZeroIntDataEvent(data, eventID));
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
        if (_secondsToWait <= 0) yield break;
        while (_secondsToWait > 0)
        {
            if (_secondsToWait > 0) {
                _secondsToWait--;
                yield return _wfs;
            } else
            {
                _secondsToWait -= 0.1f;
                yield return _wfms;
            }
        }

        foreach (var item in endWaitForSeconds)
        {
            if (item.id != eventID) continue;
            item.onWaitFinished.Invoke();
            break;
        }
    }

    private IEnumerator WaitForZeroIntDataEvent(IntData obj, string eventID)
    {
        _waitAmount = obj.value;
        if (_waitAmount <= 0) yield break;
        while (_waitAmount > 0)
        {
            _waitAmount = obj.value;
            yield return _wffu;
        }

        foreach (var item in endWaitForZero)
        {
            if (item.id != eventID) continue;
            item.onWaitFinished.Invoke();
            break;
        }
    }

    private IEnumerator WaitForFixedUpdateEvent(string eventID)
    {
        yield return _wffu;
        foreach (var item in endWaitForFixedUpdate)
        {
            if (item.id != eventID) continue;
            item.onWaitFinished.Invoke();
            break;
        }
    }
}