using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class ShaderTest: MonoBehaviour
    {
        public ComputeShader gameOfLifeShader;

        void Start()
        {
            // Grid dimensions
            int gridSize = 3;
            int totalCells = gridSize * gridSize * gridSize;

            // Test data: initial grid
            int[] currentGrid = new int[totalCells];
            int[] expectedNextGrid = new int[totalCells];
            Vector4[] tempColor = new Vector4[totalCells];
            Vector4[] tempPos = new Vector4[totalCells];
            
            int i =gridSize * gridSize * gridSize / 2 + gridSize * gridSize / 2 + gridSize / 2;
            int j = (gridSize * gridSize * gridSize / 2 + gridSize * gridSize / 2 + gridSize / 2)+ 1;
            int k = (gridSize * gridSize * gridSize / 2 + gridSize * gridSize / 2 + gridSize / 2) + 2;
            int l = (gridSize * gridSize * gridSize / 2 + gridSize * gridSize / 2 + gridSize / 2) -1;
            i = 5;
            j = 6;
            k = 5 + gridSize;
            l = 6+gridSize;
            currentGrid[i] = 4;
            currentGrid[j] = 4;
            currentGrid[k] = 4;
            currentGrid[l] = 4;
            
            tempColor[i] = new Vector4(1, 1, 0, 1);
            tempColor[j] = new Vector4(1, 1, 0, 1);
            tempColor[k] = new Vector4(1, 1, 0, 1);
             tempColor[l] = new Vector4(1, 1, 0, 1);
            
            tempPos[i] = new Vector4(5,0, 0, 1);
            tempPos[j] = new Vector4(6, 0,0, 1);
            tempPos[k] = new Vector4(5, 1, 0, 1);
            tempPos[l] = new Vector4(6,1,0, 1);
            // Buffers
            ComputeBuffer currentGridBuffer = new ComputeBuffer(totalCells, sizeof(int));
            ComputeBuffer nextGridBuffer = new ComputeBuffer(totalCells, sizeof(int));
            ComputeBuffer cubeColorsBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);
            ComputeBuffer cubePositionsBuffer = new ComputeBuffer(totalCells, sizeof(float) * 4);

            currentGridBuffer.SetData(currentGrid);
            nextGridBuffer.SetData(new int[totalCells]); // Empty next grid

            // Set shader properties
            gameOfLifeShader.SetBuffer(0, "currentGrid", currentGridBuffer);
            gameOfLifeShader.SetBuffer(0, "nextGrid", nextGridBuffer);
            gameOfLifeShader.SetBuffer(0, "cubeColors", cubeColorsBuffer);
            gameOfLifeShader.SetBuffer(0, "cubePositions", cubePositionsBuffer);
            gameOfLifeShader.SetInt("gridSize", gridSize);
            gameOfLifeShader.SetFloat("maxDistance", gridSize);
            gameOfLifeShader.SetBool("wrapEdges", false);
            gameOfLifeShader.SetVector("center", new Vector3(gridSize / 2, gridSize / 2, gridSize / 2));

            // Dispatch shader
            int threadGroups = Mathf.CeilToInt(gridSize / 8.0f);
            gameOfLifeShader.Dispatch(0, threadGroups, threadGroups, threadGroups);

            // Retrieve results
            int[] resultNextGrid = new int[totalCells];
            nextGridBuffer.GetData(resultNextGrid);

            // Validate results
            bool isCorrect = resultNextGrid.SequenceEqual(expectedNextGrid);
            Debug.Log($"Simulation correct: {isCorrect}");

            // Cleanup
            currentGridBuffer.Release();
            nextGridBuffer.Release();
            cubeColorsBuffer.Release();
            cubePositionsBuffer.Release();
        }
    }
}