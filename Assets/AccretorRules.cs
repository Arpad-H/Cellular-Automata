using UnityEngine;

public  class CellularAutomatonRules : MonoBehaviour
{
    private int[,,,] rules;
    private int maxStates = 5; // Number of possible states (0..4)
    private int faceNeighbors = 7; // 0..6
    private int edgeNeighbors = 13; // 0..12
    private int cornerNeighbors = 9; // 0..8

    [Range(0, 100)] public float fillPercentage = 20f; // Fill density (in percentage)

    private void Start()
    {
        // Initialize the rule array
        rules = new int[maxStates, faceNeighbors, edgeNeighbors, cornerNeighbors];

        // Seed the rule array
        SeedRules();
    }

    private void SeedRules()
    {
        float fillThreshold = fillPercentage / 100f; // Convert percentage to probability

        for (int state = 0; state < maxStates; state++)
        {
            for (int face = 0; face < faceNeighbors; face++)
            {
                for (int edge = 0; edge < edgeNeighbors; edge++)
                {
                    for (int corner = 0; corner < cornerNeighbors; corner++)
                    {
                        if (Random.value < fillThreshold) // Fill based on density
                        {
                            rules[state, face, edge, corner] = Random.Range(0, maxStates);
                        }
                        else
                        {
                            rules[state, face, edge, corner] = 0; // Default value
                        }
                    }
                }
            }
        }

        Debug.Log("Rules seeded with fill percentage: " + fillPercentage + "%");
    }
    public int[,,,] GetRules()
    {
        return rules;
    }
}