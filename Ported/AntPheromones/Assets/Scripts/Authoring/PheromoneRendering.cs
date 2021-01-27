﻿using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PheromoneRendering : MonoBehaviour
{
    [Header("Update Bools")]
    [SerializeField] private bool _randomizePheromones = false;
    [SerializeField] private bool _allowPheromoneDynamicBuffer = false;

    [Header("Map Properties")]
    [SerializeField] private Material _groundMat = null;
    [SerializeField] private Texture2D _defaultOverlayTexture = null;
    [SerializeField] public static int _resolution = 128;
    [SerializeField] public static float _worldSize = 128;
    [SerializeField] public static float2 _worldOffset = 128;

    // Private Variables
    Texture2D _pheromoneTexture = null;
    byte[] _pheromoneArray = null;


    #region // MonoBehaviour Events
    private void Start()
    {
        _pheromoneArray = new byte[_resolution * _resolution];
    }

    private void Update()
    {
        if(_randomizePheromones)
        {
            GenerateRandomData();
            SetPheromoneArray(_pheromoneArray);
        }
    }

    private void OnDisable()
    {
        if (_pheromoneTexture == null) return;
        Texture2D.Destroy(_pheromoneTexture);
    }
    #endregion


    #region // MonoBehaviour Random Data Generation
    private void GenerateRandomData()
    {
        for (int i = 0; i < _pheromoneArray.Length; i++)
        {
            _pheromoneArray[i] = (byte)UnityEngine.Random.Range(0, 255);
        }
    }

    // This byteArray is generated by this MonoBehaviour
    public void SetPheromoneArray(byte[] byteArray)
    {
        CheckTextureInit();

        _pheromoneTexture.SetPixelData(byteArray, 0, 0);
        _pheromoneTexture.Apply();
    }

    #endregion


    // This dynamic buffer is generated by the PheromoneSystem
    public void SetPheromoneArray(DynamicBuffer<PheromoneStrength> pheromoneBuffer)
    {
        if (_allowPheromoneDynamicBuffer)
        {
            CheckTextureInit();

            _pheromoneTexture.SetPixelData(pheromoneBuffer.AsNativeArray(), 0, 0);

            _pheromoneTexture.Apply();
        }
    }

    // Initialize the texture
    public void CheckTextureInit()
    {
        if(_pheromoneTexture != null && _pheromoneTexture.width != _resolution)
        {
            Texture2D.Destroy(_pheromoneTexture);
            _pheromoneTexture = null;
        }

        if (_pheromoneTexture == null)
        {
            _pheromoneTexture = new Texture2D(_resolution, _resolution, TextureFormat.R8, mipChain: false, linear: true);
        }

        _groundMat.SetTexture("_TEX_Overlay", _pheromoneTexture);
    }

    public void OnDestroy()
    {
        _groundMat.SetTexture("_TEX_Overlay", _defaultOverlayTexture);
    }
}
