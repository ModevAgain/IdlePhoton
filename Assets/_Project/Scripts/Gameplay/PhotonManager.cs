using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


using UnityEngine;
using UnityEngine.Serialization;
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
    private NativeList<Matrix4x4> _results;
    
    private NativeArray<int> _photonFlags;
    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _direction;
    private NativeQueue<(Vector3, Vector3)> _photonAddQueue;
    private NativeList<int> _finishedPhotons;
    private Vector3[] _positionsBuffer;
    private int _randIndex;

    void Start()
    {
        _matrices = new NativeArray<Matrix4x4>(MaxPhotonCount, Allocator.Persistent);
        _positions = new NativeArray<Vector3>(MaxPhotonCount, Allocator.Persistent);
        _direction = new NativeArray<Vector3>(MaxPhotonCount, Allocator.Persistent);
        _photonFlags = new NativeArray<int>(MaxPhotonCount, Allocator.Persistent);
        _results = new NativeList<Matrix4x4>(Allocator.Persistent);
        _photonAddQueue = new NativeQueue<(Vector3, Vector3)>(Allocator.Persistent);
        _finishedPhotons = new NativeList<int>(Allocator.Persistent);
        
        FillInPosition();

        for (int i = 0; i < MaxPhotonCount; i++)
        {
            _photonFlags[i] = 0;
            _positions[i] = DequeueRandomPosition();
            _direction[i] = (CenterTarget.position - _positions[i]).normalized * Speed;
        }
    }

    public void AddPhoton()
    {
        var pos = DequeueRandomPosition();
        _photonAddQueue.Enqueue((pos, (CenterTarget.position - pos).normalized * Speed));
    }
    
    public void Tick(float deltaTime)
    {
        var job = new MatrixFilterJob
        {
            Positions = _positions,
            Directions = _direction,
            Flags = _photonFlags,
            InputMatrices = _matrices,
            OutputMatrices = _results,
            PhotonQueue = _photonAddQueue,
            FinishedPhotons = _finishedPhotons,
            Scale = Scale,
            PhotonCount = MaxPhotonCount,
            CenterPosZ = CenterTarget.position.z,
            DeltaTime = deltaTime
        };

        job.Run();
        
        CurrentPhotonCount = _results.Length;
        if(_finishedPhotons.Length > 0)
            PhotonsFinished?.Invoke(_finishedPhotons.Length);
    }

    private void Update()
    {
        if(Render)
            Graphics.DrawMeshInstanced(photonMesh, 0, photonMaterial, _results.AsArray().ToArray(), CurrentPhotonCount, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
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

    private Vector3 DequeueRandomPosition()
    {
        if(_randIndex + 1 >= _positionsBuffer.Length)
            _randIndex = 0;
        return _positionsBuffer[_randIndex++];
    }
}

[BurstCompile]
public struct MatrixFilterJob : IJob
{
    public NativeArray<Vector3> Positions;
    public NativeArray<Vector3> Directions;
    public NativeArray<int> Flags;
    public NativeArray<Matrix4x4> InputMatrices;
    public NativeList<Matrix4x4> OutputMatrices;
    public NativeQueue<(Vector3, Vector3)> PhotonQueue;
    public NativeList<int> FinishedPhotons;
    
    [ReadOnly] public float Scale;
    [ReadOnly] public int PhotonCount;
    [ReadOnly] public float CenterPosZ;
    [ReadOnly] public float DeltaTime;

    public void Execute()
    {
        OutputMatrices.Clear();
        FinishedPhotons.Clear();
        
        for (int i = 0; i < PhotonCount; i++)
        {
            if (Flags[i] == 0)
            {
                if (!PhotonQueue.IsEmpty())
                {
                    Flags[i] = 1;
                    (Positions[i], Directions[i]) = PhotonQueue.Dequeue();
                }
                else continue;
            }
            
            Positions[i] += Directions[i] * DeltaTime;


            InputMatrices[i] = Matrix4x4.TRS(Positions[i], Quaternion.identity, Vector3.one * Scale);
            
            // Reset or destroy logic
            if (Positions[i].z > CenterPosZ)
            {
                Flags[i] = 0;
                FinishedPhotons.Add(i);
            }
        }
        
        for (int i = 0; i < Flags.Length; i++)
        {
            if (Flags[i] == 1)
                OutputMatrices.Add(InputMatrices[i]);
        }
    }
}