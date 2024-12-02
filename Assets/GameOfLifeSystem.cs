using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameOfLifeSystem : MonoBehaviour
{
    public ComputeShader crawlComputeShader;
    public ComputeShader accretorComputeShader;
    public ComputeShader crystalsComputeShader;
    public ComputeShader cloudsComputeShader;
    private ComputeShader computeShader;
    public Material instancedMaterial;
    public Mesh cubeMesh;

    public TextMeshProUGUI generationText;

    private ComputeBuffer cubeColorBuffer;
    private ComputeBuffer cubePositionBuffer;
    private ComputeBuffer currentGridBuffer;
    private ComputeBuffer nextGridBuffer;

    public int gridSize = 64;
    public bool wrapEdges = false;
    private int kernelHandle;


    private int[] currentGrid;
    private int[] nextGrid;
    private Stack<int[]> generationHistory = new Stack<int[]>();
    private Stack<Vector4[]> colorHistory = new Stack<Vector4[]>();
    private Stack<Vector4[]> posHistory = new Stack<Vector4[]>();
    private const int MaxHistorySize = 10;

    public bool isRunning = false;
    public bool isStarted = false;
    public float simulationDelay = 0.5f;
    private float timeSinceLastUpdate;
    private int generation = 0;
    public Ruleset ruleset = Ruleset.CRAWLERS;
   private int [,,,] AccretorRules;
    public void Start()
    {
    }

    public void Setup()
    {
        timeSinceLastUpdate = 0;

        // Initialize buffers
        int totalCells = gridSize * gridSize * gridSize;
        Vector3 center = new Vector3(gridSize / 2, gridSize / 2, gridSize / 2);
        cubeColorBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);
        cubePositionBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);
        currentGridBuffer = new ComputeBuffer(totalCells, sizeof(int));
        nextGridBuffer = new ComputeBuffer(totalCells, sizeof(int));


        String kernelName = "ComputeStep";
        switch (ruleset)
        {
            case Ruleset.CRAWLERS:
                kernelName = "ComputeStep";
                computeShader = crawlComputeShader;
                SetupCrawlers(totalCells, center);
                break;
            case Ruleset.ACCRETOR:
                kernelName = "AccretorCompute";
                computeShader = accretorComputeShader;
                SetupAccretor(totalCells, center);
                break;
            case Ruleset.CLOUDS:
                kernelName = "CloudCompute";
                computeShader = cloudsComputeShader;
                SetupClouds(totalCells, center);
                break;
            case Ruleset.CRYSTALS:
                kernelName = "CrystalGrowth";
                computeShader = crystalsComputeShader;
                SetupCrystals(totalCells, center);
                break;
            default:
                break;
        }
        Debug.Log("Kernel name: " + kernelName);
        // compute shader kernel for stepin func
        kernelHandle = computeShader.FindKernel(kernelName);

        // Set compute shader buffers
        computeShader.SetBuffer(kernelHandle, "currentGrid", currentGridBuffer);
        computeShader.SetBuffer(kernelHandle, "nextGrid", nextGridBuffer);
        computeShader.SetBuffer(kernelHandle, "cubeColors", cubeColorBuffer);
        computeShader.SetBuffer(kernelHandle, "cubePositions", cubePositionBuffer);


        // Set parameters for compute shader
        computeShader.SetInt("gridSize", gridSize);
        computeShader.SetFloat("maxDistance", gridSize);
        computeShader.SetVector("center", center);
        computeShader.SetBool("wrapEdges", false);

        SetBuffers();
    }

    private void SetupAccretor(int totalCells, Vector3 center)
    {
       // AccretorRules = GameObject.Find("AccretorRules").GetComponent<CellularAutomatonRules>().GetRules();
        currentGrid = new int[totalCells];
        Vector4[] tempColor = new Vector4[totalCells];
        Vector4[] tempPos = new Vector4[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            Vector3 pos = OneDToThreeD(i);
            if (Math.Abs(pos.x - center.x) > 2.5 || Math.Abs(pos.y - center.y) >2.5 ||
                Math.Abs(pos.z - center.z) > 2.5)
            {
                continue;
            }

            
                currentGrid[i] = 1;
                tempColor[i] = new Vector4(1, 1, 0, 1);

                int x = i % gridSize;
                int y = (i / gridSize) % gridSize;
                int z = i / (gridSize * gridSize);

                tempPos[i] = new Vector4(x, y, z, 1);
            
        }


        currentGridBuffer.SetData(currentGrid);
        cubeColorBuffer.SetData(tempColor);
        cubePositionBuffer.SetData(tempPos);
    }

    private void SetupCrawlers(int totalCells, Vector3 center)
    {
        Debug.Log("Setting up crawlers");
        // Initialize cell states
        currentGrid = new int[totalCells];
        Vector4[] tempColor = new Vector4[totalCells];
        Vector4[] tempPos = new Vector4[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            Vector3 pos = OneDToThreeD(i);
            if (Math.Abs(pos.x - center.x) > gridSize / 4 || Math.Abs(pos.y - center.y) > gridSize / 4 ||
                Math.Abs(pos.z - center.z) > gridSize / 4)
            {
                continue;
            }

            if (Random.Range(0, 100) <= 60)
            {
                currentGrid[i] = 4;
                tempColor[i] = new Vector4(1, 1, 0, 1);

                int x = i % gridSize;
                int y = (i / gridSize) % gridSize;
                int z = i / (gridSize * gridSize);

                tempPos[i] = new Vector4(x, y, z, 1);
            }
            else
            {
                currentGrid[i] = 0;
                tempColor[i] = new Vector4(0, 0, 0, 0);
                tempPos[i] = new Vector4(1111110, 1111110, 1111110, 1);
            }
        }


        currentGridBuffer.SetData(currentGrid);
        cubeColorBuffer.SetData(tempColor);
        cubePositionBuffer.SetData(tempPos);
    }
    private void SetupClouds(int totalCells, Vector3 center)
    {
        Debug.Log("Setting up clouds");
        // Initialize cell states
        currentGrid = new int[totalCells];
        Vector4[] tempColor = new Vector4[totalCells];
        Vector4[] tempPos = new Vector4[totalCells];

        for (int i = 0; i < totalCells; i++)
        {
            Vector3 pos = OneDToThreeD(i);
            if (Math.Abs(pos.x - center.x) > gridSize / 4 || Math.Abs(pos.y - center.y) > gridSize / 4 ||
                Math.Abs(pos.z - center.z) > gridSize / 4)
            {
                continue;
            }

            if (Random.Range(0, 100) <= 60)
            {
                currentGrid[i] = 1;
                tempColor[i] = new Vector4(pos.x/gridSize, pos.y/gridSize, pos.z/gridSize, 1);

                int x = i % gridSize;
                int y = (i / gridSize) % gridSize;
                int z = i / (gridSize * gridSize);

                tempPos[i] = new Vector4(x, y, z, 1);
            }
            else
            {
                currentGrid[i] = 0;
                tempColor[i] = new Vector4(0, 0, 0, 0);
                tempPos[i] = new Vector4(1111110, 1111110, 1111110, 1);
            }
        }


        currentGridBuffer.SetData(currentGrid);
        cubeColorBuffer.SetData(tempColor);
        cubePositionBuffer.SetData(tempPos);
    }
    
    private void SetupCrystals(int totalCells, Vector3 center)
    {
        Debug.Log("Setting up crystals");
        // Initialize cell states
        currentGrid = new int[totalCells];
        Vector4[] tempColor = new Vector4[totalCells];
        Vector4[] tempPos = new Vector4[totalCells];

        int i =ThreeDToOneD(gridSize/2 , gridSize/2, gridSize/2);
        currentGrid[i] = 1;
        tempColor[i] = new Vector4(1, 1, 0, 1);
        tempPos[i] = new Vector4(gridSize/2, gridSize/2, gridSize/2, 1);


        currentGridBuffer.SetData(currentGrid);
        cubeColorBuffer.SetData(tempColor);
        cubePositionBuffer.SetData(tempPos);
    }

    private int ThreeDToOneD(int x, int y, int z)
    {
        return x + y * gridSize + z * gridSize * gridSize;
    }

    private Vector3 OneDToThreeD(int i)
    {
        int x = i % gridSize;
        int y = i / (gridSize * gridSize);
        int z = (i / gridSize) % gridSize;
        return new Vector3(x, y, z);
    }


    void Update()
    {
        if (!isStarted)
        {
            return;
        }

        if (isRunning)
        {
            timeSinceLastUpdate += Time.deltaTime;

            // sim step timer
            if (timeSinceLastUpdate >= simulationDelay)
            {
                Step();
                timeSinceLastUpdate = 0f;
            }
        }

        DrawGeneration();
        //Graphics.DrawMeshInstanced(cubeMesh, 0, instancedMaterial, new Matrix4x4[gridSize * gridSize * gridSize], gridSize * gridSize * gridSize);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(gridSize / 2, gridSize / 2, gridSize / 2),
            new Vector3(gridSize, gridSize, gridSize));
    }

    private void DrawGeneration()
    {
        Graphics.RenderMeshPrimitives(new RenderParams(instancedMaterial), cubeMesh, 0,
            cubePositionBuffer.count);
    }

    private void SetBuffers()
    {
        computeShader.SetBuffer(kernelHandle, "currentGrid", currentGridBuffer);
        computeShader.SetBuffer(kernelHandle, "nextGrid", nextGridBuffer);


        // Pass the buffers to the instanced material and vertex / fragment shader
        instancedMaterial.SetBuffer("instanceColors", cubeColorBuffer);
        instancedMaterial.SetBuffer("instancePositions", cubePositionBuffer);
    }

    public void Step()
    {
        SnapshotState();

        computeShader.Dispatch(kernelHandle, gridSize / 8, gridSize / 8, gridSize / 8);
        // Swap buffers

        ComputeBuffer temp = currentGridBuffer;
        currentGridBuffer = nextGridBuffer;
        nextGridBuffer = temp;

        SetBuffers();
        generation++;

        OnGUI();
    }

    private void SnapshotState()
    {
        int[] currentGridSnapshot = new int[gridSize * gridSize * gridSize];
        Vector4[] colorSnapshot = new Vector4[gridSize * gridSize * gridSize];
        Vector4[] posSnapshot = new Vector4[gridSize * gridSize * gridSize];
        currentGridBuffer.GetData(currentGridSnapshot);
        generationHistory.Push(currentGridSnapshot);
        cubeColorBuffer.GetData(colorSnapshot);
        colorHistory.Push(colorSnapshot);
        cubePositionBuffer.GetData(posSnapshot);
        posHistory.Push(posSnapshot);

        if (generationHistory.Count > MaxHistorySize)
        {
            generationHistory.TrimExcess();
            colorHistory.TrimExcess();
            posHistory.TrimExcess();
        }
    }

    void OnDestroy()
    {
        // Release all buffers
        cubeColorBuffer.Release();
        cubePositionBuffer.Release();
        currentGridBuffer.Release();
        nextGridBuffer.Release();
    }

    public void PreviousGeneration()
    {
        if (generationHistory.Count == 0)
        {
            return;
        }

        generation--;

        int[] previousGridState = generationHistory.Pop();
        Vector4[] previousColorState = colorHistory.Pop();
        Vector4[] previousPosState = posHistory.Pop();

        currentGridBuffer.SetData(previousGridState);
        cubeColorBuffer.SetData(previousColorState);
        cubePositionBuffer.SetData(previousPosState);
        SetBuffers();
    }

    private void OnGUI()
    {
        generationText.text = "Generation: " + generation;
    }
}