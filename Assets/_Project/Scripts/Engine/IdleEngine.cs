using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEngine : MonoBehaviour
{
    public static IdleEngine Root;
    
    public float TickTime = 0.05f;
    public float TickBalanceValue = 1;
    public uint TickIndex;

    public bool Debug_LogTickableRegistration;
    
    private List<ITickable> _tickables;
    private List<ITickable> _removeTickablesOnFinishTick;
    private bool _active;
    
    private Coroutine _tickRoutine;

    private void Awake()
    {
        if (Root == null)
        {
            Root = this;
        }
        
        _tickables = new List<ITickable>();
        _removeTickablesOnFinishTick = new List<ITickable>();
    }

    void Start()
    {
        StartEngine();
    }

    public void StartEngine()
    {
        _active = true;
        _tickRoutine = StartCoroutine(TickRoutine());
    }

    private IEnumerator TickRoutine()
    {
        var waiter = new WaitForSeconds(TickTime);
        while (_active)
        {
            TickIndex++;

            for (int i = 0; i < _tickables.Count; i++)
            {
                _tickables[i]?.Tick(TickIndex, TickBalanceValue, TickTime);
            }
            yield return waiter;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void RegisterTickable(ITickable tickable)
    {
        if (Debug_LogTickableRegistration)
            Debug.Log("Registered Tickable " + tickable.GetType());
        
        
        _tickables.Add(tickable);
    }

    public void RemoveTickable(ITickable tickable)
    {
        var i = _tickables.IndexOf(tickable);
        _tickables[i] = null;
    }
}
