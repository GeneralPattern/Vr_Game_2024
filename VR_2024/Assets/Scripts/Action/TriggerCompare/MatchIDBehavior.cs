using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchIDBehavior : IDBehavior
{
    [System.Serializable]
    public struct PossibleMatch
    {
        public ID id;
        public UnityEvent triggerEvent;
    }

    private WaitForFixedUpdate _wffu = new WaitForFixedUpdate();
    public List<PossibleMatch> triggerEnterMatches;
    private ID _otherIdObj;

    private void OnTriggerEnter(Collider other)
    {
        IDBehavior idBehavior = other.GetComponent<IDBehavior>();
        if (idBehavior == null) return;
        _otherIdObj = idBehavior.idObj;
        StartCoroutine(CheckId(_otherIdObj, triggerEnterMatches));
    }
    
    private IEnumerator CheckId(ID nameId, List<PossibleMatch> possibleMatches)
    {
        bool noMatch = true;
        _otherIdObj = nameId;
        foreach (var obj in possibleMatches)
        {
            if (_otherIdObj != obj.id) continue;
            noMatch = false;
            obj.triggerEvent.Invoke();
            yield return _wffu;
        }
        if (noMatch) 
            Debug.Log($"No match found on: {this}");
    }
}