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
    private List<ITickable> _tickables;
    private bool _active;
    
    private Coroutine _tickRoutine;

    private void Awake()
    {
        if (Root == null)
        {
            Root = this;
        }
        
        _tickables = new List<ITickable>();
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
        while (_active)
        {
            TickIndex++;
            
            foreach (var tickable in _tickables)
            {
                tickable.Tick(TickIndex, TickBalanceValue, TickTime);
            }
            yield return new WaitForSeconds(TickTime);
        }
    }

    public void RegisterTickable(ITickable tickable)
    {
        _tickables.Add(tickable);
    }

    public void RemoveTickable(ITickable tickable)
    {
        _tickables.Remove(tickable);
    }
}
