using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhotonManager : MonoBehaviour
{
    public Mesh photonMesh;
    public Material photonMaterial;
    public int MaxPhotonCount = 100000;

    public Transform CenterOrigin;
    public Transform CenterTarget;
    public Vector2 SpawnRangeFromCenter;
    public float Speed;
    public float Scale;
    
    public int CurrentPhotonCount;

    public Action<int> PhotonsFinished;

    public bool Render;
    
    private NativeArray<Matrix4x4> _matrices;
    private NativeArray<Matrix4x4> _results;
    private Matrix4x4[] _renderResults;
    
    private NativeArray<int> _photonFlags;
    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _direction;
    private NativeArray<(Vector3, Vector3)> _photonAddQueue;
    private NativeArray<int> _activeCount;
    private NativeArray<int> _finishedCount;
    private Vector3[] _positionsBuffer;
    private int _randIndex;
    private Vector3 _targetCache;
    private RenderParams _renderParams;

    private bool _receivedFirstPhoton;
    private int _photonAddPointer;
    
    void Start()
    {
        _matrices = new NativeArray<Matrix4x4>(MaxPhotonCount, Allocator.Persistent);
        _positions = new NativeArray<Vector3>(MaxPhotonCount, Allocator.Persistent);
        _direction = new NativeArray<Vector3>(MaxPhotonCount, Allocator.Persistent);
        _photonFlags = new NativeArray<int>(MaxPhotonCount, Allocator.Persistent);
        _results = new NativeArray<Matrix4x4>(MaxPhotonCount, Allocator.Persistent);
        _renderResults = new Matrix4x4[MaxPhotonCount];
        _photonAddQueue = new NativeArray<(Vector3, Vector3)>(MaxPhotonCount/100, Allocator.Persistent);
        _activeCount = new NativeArray<int>(1, Allocator.Persistent);
        _finishedCount = new NativeArray<int>(1, Allocator.Persistent);
        
        _renderParams = new RenderParams(photonMaterial)
        {
            receiveShadows = false,
            shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
            layer = LayerMask.NameToLayer("Simulation")
        };

        FillInPosition();

        for (int i = 0; i < MaxPhotonCount; i++)
        {
            _photonFlags[i] = 0;
            _positions[i] = DequeueRandomPosition();
            _direction[i] = (CenterTarget.position - _positions[i]).normalized * Speed;
        }
    }

    public void AddPhoton(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _photonAddQueue[_photonAddPointer] = (DequeueRandomPosition(true), (CenterTarget.position - DequeueRandomPosition()).normalized * Speed);
            _photonAddPointer++;
        }
        _receivedFirstPhoton = true;
    }
    
    public void Tick(float deltaTime)
    {
        if (!_receivedFirstPhoton)
            return;
        
        var job = new MatrixFilterJob
        {
            Positions = _positions,
            Directions = _direction,
            Flags = _photonFlags,
            InputMatrices = _matrices,
            OutputMatrices = _results,
            PhotonQueue = _photonAddQueue,
            ActivePhotonCount = _activeCount,
            FinishedPhotonCount = _finishedCount,
            RecalculateDirection = _targetCache != CenterTarget.position,
            Scale = Scale,
            MaxPhotonCount = MaxPhotonCount,
            PhotonAddCount = _photonAddPointer + 1,
            Target = CenterTarget.position,
            DeltaTime = deltaTime,
            Speed = Speed
        };

        job.Run();
        
        CurrentPhotonCount = _activeCount[0];
        if(_finishedCount[0] > 0)
            PhotonsFinished?.Invoke(_finishedCount[0]);
        
        _targetCache = CenterTarget.position;
        _photonAddPointer = 0;
    }

    private void Update()
    {
        if(Render && CurrentPhotonCount > 0)
            Graphics.RenderMeshInstanced(_renderParams, photonMesh, 0, _results, CurrentPhotonCount);
    }

    private Vector3 GetRandomPointFromCenterWithRange()
    {
        var range = Random.Range(SpawnRangeFromCenter.x, SpawnRangeFromCenter.y);
        return (CenterOrigin.position + (Vector3)Random.insideUnitCircle - CenterOrigin.position).normalized * range;
    }

    private void FillInPosition()
    {
        _positionsBuffer = new Vector3[MaxPhotonCount*100];
        for (int i = 0; i < _positionsBuffer.Length; i++)
        {
            _positionsBuffer[i] = GetRandomPointFromCenterWithRange();
            _positionsBuffer[i].z = CenterOrigin.position.z;
        }
    }

    private Vector3 DequeueRandomPosition(bool lookUpOnly = false)
    {
        if(_randIndex + 1 >= _positionsBuffer.Length)
            _randIndex = 0;
        return _positionsBuffer[lookUpOnly ? _randIndex : _randIndex++];
    }
}

[BurstCompile]
public struct MatrixFilterJob : IJob
{
    public NativeArray<Vector3> Positions;
    public NativeArray<Vector3> Directions;
    public NativeArray<int> Flags;
    public NativeArray<Matrix4x4> InputMatrices;
    public NativeArray<Matrix4x4> OutputMatrices;
    public NativeArray<(Vector3, Vector3)> PhotonQueue;
    public NativeArray<int> ActivePhotonCount;
    public NativeArray<int> FinishedPhotonCount;
    public bool RecalculateDirection;
    
    [ReadOnly] public float Scale;
    [ReadOnly] public int MaxPhotonCount;
    [ReadOnly] public int PhotonAddCount;
    [ReadOnly] public Vector3 Target;
    [ReadOnly] public float DeltaTime;
    [ReadOnly] public float Speed;

    public void Execute()
    {
        int photonQueueIndex = 0;
            
        ActivePhotonCount[0] = 0;
        FinishedPhotonCount[0] = 0;
        
        for (int i = 0; i < MaxPhotonCount; i++)
        {
            if (Flags[i] == 0)
            {
                if (PhotonAddCount > photonQueueIndex)
                {
                    Flags[i] = 1;
                    (Positions[i], Directions[i]) = PhotonQueue[photonQueueIndex++];
                }
                else continue;
            }

            if(RecalculateDirection)
                Directions[i] = (Target - Positions[i]).normalized * Speed;
            
            Positions[i] += Directions[i] * DeltaTime;
            
            InputMatrices[i] = Matrix4x4.TRS(Positions[i], Quaternion.identity, Vector3.one * Scale);
            
            // Reset or destroy logic
            if (Positions[i].z > Target.z)
            {
                Flags[i] = 0;
                FinishedPhotonCount[0]++;
            }
        }
        
        for (int i = 0; i < Flags.Length; i++)
        {
            if (Flags[i] == 1)
            {
                OutputMatrices[ActivePhotonCount[0]] = InputMatrices[i];
                ActivePhotonCount[0]++;
            }
        }
    }
}