using System;
using _Project.Scripts.Engine;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhotonObjective : GameplayModule
{
    public PositionalControl TargetControl;
    public Bounds ObjectiveBounds;
    
    public Vector2 ActiveObjectivePosition;

    public Color DefaultColor;
    public Color OnTargetColor;

    public Vector2 RespawnTimeRangeInSeconds;
    
    public MeshRenderer[] ReticuleRenderer;

    public ObjectiveData[] Objectives;
    
    public bool ObjectiveActive;
    public float Density;
    
    private PhotonManager _photonManager;
    private ObjectiveData _currentObjective;
    private int _currentObjectiveIndex = -1;
    [SerializeField]
    private float _currentAccumulatedValue;
    [SerializeField]
    private bool _targetOnObjective;

    private TimerInfo _currentTimer;

    private void Start()
    {
        _photonManager = FindAnyObjectByType<PhotonManager>();
        TargetControl.OnPositionChanged += OnMovedTarget;
        
        OnModuleReset();
    }

    public void Init()
    {
        foreach (var ren in ReticuleRenderer)
        {
            ren.enabled = true;
        }
        
        _currentTimer = TickableTimer.Start(Random.Range(RespawnTimeRangeInSeconds.x, RespawnTimeRangeInSeconds.y), OnClick_GenerateObjective);
    }

    public void GenerateObjective(ObjectiveData data)
    {
       var x = Random.Range(ObjectiveBounds.min.x, ObjectiveBounds.max.x);
       var y = Random.Range(ObjectiveBounds.min.y, ObjectiveBounds.max.y);
       
       ActiveObjectivePosition = new Vector2((int) (x % 5) * 5, (int) (y % 5) * 5);
       _currentAccumulatedValue = 0;
       _currentObjective = data;

       _photonManager.PhotonsFinished += OnReceivePhoton;
       
       TargetControl.SetHealthFill(0);
       
       ObjectiveActive = true;
       
       SimLogger.Log($"New Simulation Object detected! Coordinates are ({ActiveObjectivePosition.x},{ActiveObjectivePosition.y}).");
    }

    public void OnMovedTarget(Vector2 newPos)
    {
        _targetOnObjective = ObjectiveActive && newPos == ActiveObjectivePosition;
        SetColor(_targetOnObjective ? OnTargetColor : DefaultColor);
        if (_targetOnObjective)
        {
            SetColor(OnTargetColor);
            TargetControl.SetHealthFill(_currentAccumulatedValue/_currentObjective.ObjectiveValue);
        }
        else
        {
            SetColor(DefaultColor);
            TargetControl.SetHealthFill(0);
        }
    }

    public void OnReceivePhoton(int count)
    {
        if (!_targetOnObjective)
            return;
        
        _currentAccumulatedValue += count * Density;
        
        TargetControl.SetHealthFill(_currentAccumulatedValue/_currentObjective.ObjectiveValue);
        
        if (_currentAccumulatedValue > _currentObjective.ObjectiveValue)
        {
            FinishObjective();   
        }
    }

    private void FinishObjective()
    {
        //  Play effect
        _photonManager.PhotonsFinished -= OnReceivePhoton;
        ObjectiveActive = false;
        SetColor(DefaultColor);
        TargetControl.SetHealthFill(1);
        
        // Add x score
        int x = 0;
        
        SimLogger.Log($"Simulation Object finished. Found {x} score.");
        
        _currentTimer = TickableTimer.Start(Random.Range(RespawnTimeRangeInSeconds.x, RespawnTimeRangeInSeconds.y), OnClick_GenerateObjective);
    }

    private void SetColor(Color color)
    {
        foreach (var ren in ReticuleRenderer)
        {
            ren.material.color = color;
        }
    }

    public override void OnModuleEnable()
    {
        base.OnModuleEnable();
        Init();
    }

    public override void OnModuleReset()
    {
        foreach (var ren in ReticuleRenderer)
        {
            ren.enabled = false;
            ren.material.color = DefaultColor;
        }
        
        _currentTimer?.Cancel();
        
        TargetControl.OnModuleReset();
    }

    public void OnClick_GenerateObjective()
    {
        _currentObjectiveIndex++;
        GenerateObjective(Objectives[_currentObjectiveIndex]);
    }
}

[System.Serializable]
public class ObjectiveData
{
    public float ObjectiveValue;
}
