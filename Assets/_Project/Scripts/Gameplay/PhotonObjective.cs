using UnityEngine;

public class PhotonObjective : MonoBehaviour
{
    public Bounds ObjectiveBounds;
    
    public Vector2 ActiveObjectivePosition;
    public Transform TargetTransform;

    public Color DefaultColor;
    public Color OnTargetColor;
    
    public MeshRenderer[] ReticuleRenderer;

    private ObjectiveData _currentObjective;
    private float _currentAccumulatedValue;
    private bool _targetOnObjective;
    
    public void GenerateObjective(ObjectiveData data)
    {
       var x = Random.Range(ObjectiveBounds.min.x, ObjectiveBounds.max.x);
       var y = Random.Range(ObjectiveBounds.min.y, ObjectiveBounds.max.y);
       
       ActiveObjectivePosition = new Vector2((int)x, (int)y);
       _currentAccumulatedValue = 0;
       _currentObjective = data;
    }

    public void OnMovedTarget(Vector2 newPos)
    {
        _targetOnObjective = newPos == ActiveObjectivePosition;
        SetColor(_targetOnObjective ? OnTargetColor : DefaultColor);
    }

    public void OnReceivePhoton(int count, float density)
    {
        if (!_targetOnObjective)
            return;
        
        _currentAccumulatedValue += count * density;
        if (_currentAccumulatedValue > _currentObjective.ObjectiveValue)
        {
            
        }
    }

    private void FinishObjective()
    {
        
    }

    private void SetColor(Color color)
    {
        foreach (var ren in ReticuleRenderer)
        {
            ren.material.color = color;
        }
    }
}

[System.Serializable]
public class ObjectiveData
{
    public float ObjectiveValue;
}
