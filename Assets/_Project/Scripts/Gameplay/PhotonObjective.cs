using UnityEngine;
using Random = UnityEngine.Random;

public class PhotonObjective : BaseGameplayModule
{
    public PositionalControl TargetControl;
    public Bounds ObjectiveBounds;
    
    public Vector2 ActiveObjectivePosition;

    public Color DefaultColor;
    public Color OnTargetColor;
    
    public MeshRenderer[] ReticuleRenderer;

    public ObjectiveData[] Objectives;
    
    public bool ObjectiveActive;
    public float Density;
    
    private PhotonManager _photonManager;
    private ObjectiveData _currentObjective;
    private int _currentObjectiveIndex;
    private float _currentAccumulatedValue;
    private bool _targetOnObjective;

    private void Start()
    {
        _photonManager = FindAnyObjectByType<PhotonManager>();
        
        foreach (var ren in ReticuleRenderer)
        {
            ren.enabled = false;
            ren.material.color = DefaultColor;
        }
    }

    public void Init()
    {
        foreach (var ren in ReticuleRenderer)
        {
            ren.enabled = true;
        }
    }

    public void GenerateObjective(ObjectiveData data)
    {
       var x = Random.Range(ObjectiveBounds.min.x, ObjectiveBounds.max.x);
       var y = Random.Range(ObjectiveBounds.min.y, ObjectiveBounds.max.y);
       
       ActiveObjectivePosition = new Vector2((int) (x % 5) * 5, (int) (y % 5) * 5);
       _currentAccumulatedValue = 0;
       _currentObjective = data;

       _photonManager.PhotonsFinished += OnReceivePhoton;
       TargetControl.OnPositionChanged += OnMovedTarget;
       
       ObjectiveActive = true;
       
       SimLogger.Log($"New Simulation Object detected! Coordinates are ({ActiveObjectivePosition.x},{ActiveObjectivePosition.y}).");
    }

    public void OnMovedTarget(Vector2 newPos)
    {
        _targetOnObjective = ObjectiveActive && newPos == ActiveObjectivePosition;
        SetColor(_targetOnObjective ? OnTargetColor : DefaultColor);
    }

    public void OnReceivePhoton(int count)
    {
        if (!_targetOnObjective)
            return;
        
        _currentAccumulatedValue += count * Density;
        if (_currentAccumulatedValue > _currentObjective.ObjectiveValue)
        {
            FinishObjective();   
        }
    }

    private void FinishObjective()
    {
        //  Play effect
        _photonManager.PhotonsFinished -= OnReceivePhoton;
        TargetControl.OnPositionChanged -= OnMovedTarget;
        ObjectiveActive = false;
        SetColor(DefaultColor);
        
        // Add x score
        int x = 0;
        
        SimLogger.Log($"Simulation Object finished. Found {x} score.");
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
