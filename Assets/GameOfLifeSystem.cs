using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOfLifeSystem : MonoBehaviour
{
    public ComputeShader gameOfLifeComputeShader;
    public Material instancedMaterial;
    public Mesh cubeMesh;

    private ComputeBuffer cubeColorBuffer;
    private ComputeBuffer cubePositionBuffer;
    private ComputeBuffer currentGridBuffer;
    private ComputeBuffer nextGridBuffer;

    public int gridSize = 64; 
    private int kernelHandle;

    private int[] currentGrid;
    private int[] nextGrid;


    public float simulationDelay = 0.5f; 
    private float timeSinceLastUpdate;

    void Start()
    {
        timeSinceLastUpdate = 0; //to skip the first delay wait time
        
        // Initialize buffers
        int totalCells = gridSize * gridSize * gridSize;
        cubeColorBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);
        cubePositionBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);
        currentGridBuffer = new ComputeBuffer(totalCells, sizeof(int));
        nextGridBuffer = new ComputeBuffer(totalCells, sizeof(int));

        // Initialize cell states
        currentGrid = new int[totalCells];
        nextGrid = new int[totalCells];
        for (int i = 0; i < totalCells; i++)
        {
            currentGrid[i] = Random.Range(0, 2) < 0.3f ? 1 : 0; // Random 0 or 1
        }

        currentGridBuffer.SetData(currentGrid);

        // compute shader kernel for stepin func
        kernelHandle = gameOfLifeComputeShader.FindKernel("GameOfLifeStep");

        // Set compute shader buffers
        gameOfLifeComputeShader.SetBuffer(kernelHandle, "currentGrid", currentGridBuffer);
        gameOfLifeComputeShader.SetBuffer(kernelHandle, "nextGrid", nextGridBuffer);
        gameOfLifeComputeShader.SetBuffer(kernelHandle, "cubeColors", cubeColorBuffer);
        gameOfLifeComputeShader.SetBuffer(kernelHandle, "cubePositions", cubePositionBuffer);

        // Set parameters for compute shader
        gameOfLifeComputeShader.SetInt("gridSize", gridSize);
        gameOfLifeComputeShader.SetFloat("maxDistance", gridSize);
        gameOfLifeComputeShader.SetVector("center", new Vector3(gridSize / 2, gridSize / 2, gridSize / 2));
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        // sim step timer
        if (timeSinceLastUpdate >= simulationDelay)
        {
           
            gameOfLifeComputeShader.Dispatch(kernelHandle, gridSize / 8, gridSize / 8, gridSize / 8);

            // Swap buffers
            ComputeBuffer temp = currentGridBuffer;
            currentGridBuffer = nextGridBuffer;
            nextGridBuffer = temp;


            gameOfLifeComputeShader.SetBuffer(kernelHandle, "currentGrid", currentGridBuffer);
            gameOfLifeComputeShader.SetBuffer(kernelHandle, "nextGrid", nextGridBuffer);


            // Pass the buffers to the instanced material and vertex / fragment shader
            instancedMaterial.SetBuffer("instanceColors", cubeColorBuffer);
            instancedMaterial.SetBuffer("instancePositions", cubePositionBuffer);

            timeSinceLastUpdate = 0f;
        }
        int[] test = new int[gridSize * gridSize * gridSize];
        cubePositionBuffer.GetData(test);
        
        // Draw cubes using instancing
        Graphics.RenderMeshPrimitives(new RenderParams(instancedMaterial), cubeMesh, 0,
            cubePositionBuffer.count);
        //Graphics.DrawMeshInstanced(cubeMesh, 0, instancedMaterial, new Matrix4x4[gridSize * gridSize * gridSize], gridSize * gridSize * gridSize);
    }

    void OnDestroy()
    {
        // Release all buffers
        cubeColorBuffer.Release();
        cubePositionBuffer.Release();
        currentGridBuffer.Release();
        nextGridBuffer.Release();
    }
}