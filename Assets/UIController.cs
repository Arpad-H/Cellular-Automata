using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DefaultNamespace;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject SimController;
    private GameOfLifeSystem gameOfLifeSystem;
    public Button pauseButton;
    public TMP_InputField speedInput;
    public GameObject ActiveDuringSimulation;
    public CameraOrbit CameraController;
    public Toggle startPausedToggle;
    public TextMeshProUGUI desc;
    public TMP_InputField simSpeedInput;
    private Ruleset ruleset = Ruleset.CRAWLERS;


    [Category("SimSettings")] public TMP_InputField gridSizeInput;

    // Start is called before the first frame update
    void Start()
    {
        gameOfLifeSystem = SimController.GetComponent<GameOfLifeSystem>();
        string temp = ""+ gameOfLifeSystem.simulationDelay;
        speedInput.text =temp;
        ActiveDuringSimulation.SetActive(false);
        simSpeedInput.onEndEdit.AddListener(SetSpeed);
    }


    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Pause()
    {
        gameOfLifeSystem.isRunning = !gameOfLifeSystem.isRunning;
        pauseButton.GetComponentInChildren<TMP_Text>().text = gameOfLifeSystem.isRunning ? "Pause" : "Resume";
    }

    public void Step()
    {
        if (gameOfLifeSystem.isRunning)
        {
            Pause();
        }

        gameOfLifeSystem.Step();
    }

    public void SetSpeed(string speed)
    {
        Debug.Log(speed.Length);
        string temp = ""+ speed;
        Debug.Log(temp);
        speedInput.text =temp;
        gameOfLifeSystem.simulationDelay = Math.Abs(float.Parse(temp ));
    }

    public void StartSimulation()
    { 
        SimController.SetActive(true);
       
       
        gameOfLifeSystem.gridSize = int.Parse(gridSizeInput.text);
        CameraController.gridSize = gameOfLifeSystem.gridSize;
        switch (ruleset)
        {
            case Ruleset.ACCRETOR:
                gameOfLifeSystem.ruleset = Ruleset.ACCRETOR;
                break;
            case Ruleset.CRAWLERS:
                gameOfLifeSystem.ruleset = Ruleset.CRAWLERS;
                break;
            case Ruleset.CLOUDS:
                gameOfLifeSystem.ruleset = Ruleset.CLOUDS;
                break;
            case Ruleset.CRYSTALS:
                gameOfLifeSystem.ruleset = Ruleset.CRYSTALS;
                break;
        }
        gameOfLifeSystem.Setup();
        ActiveDuringSimulation.SetActive(true);
        gameOfLifeSystem.isRunning = true;
        gameOfLifeSystem.isStarted = true;
        pauseButton.GetComponentInChildren<TMP_Text>().text = "Pause";
        
        
      //  gameOfLifeSystem.ShowInitialGeneration();
        if (startPausedToggle.isOn)
        {
            Pause();
        }
    }

    public void UpdateCameraSpeed(float speed)
    {
        CameraController.orbitSpeed = speed;
    }

    public void UpdateCameraHeight(float height)
    {
        CameraController.height = height;
    }

    public void UpdateCameraDistance(float distance)
    {
        CameraController.distance = distance;
    }
    public void PreviousGeneration()
    {
        if (gameOfLifeSystem.isRunning)
        {
            Pause();
        }
        gameOfLifeSystem.PreviousGeneration();
    }

    public void Dropdown(int change)
    {
        switch (change)
        {
            case 0:
                desc.text =
                    "Syntax: 4/4/5/M\n4-Die Zelle überlebt wenn sie genau 4 Nachbarn hat\n4-Eine Zelle wird geboren wenn sie genau 4 Nachbarn hat\n5-Eine Zelle hat 5 Zustände und zerfällt bis sie 0 erreicht (Tot)\nM- Moore Nachbarschaft (26 Nacbarn)";
                ruleset = Ruleset.CRAWLERS;
                break;
            case 1:
                desc.text = "Noch nicht implementiert";
                ruleset = Ruleset.ACCRETOR;
                break;
            case 2 :
                desc.text ="Syntax: 13-26/13-14,17-19/2/M\n13-26 Nachbarn - Die Zelle überlebt\n13,14,17,18 oder 19 Nachbarn - Eine Leere Zelle wird belebt\n2 Zustände - Tot oder lebendig\nM -  Moore Nachbarschaft (26 Nacbarn) ";
                ruleset = Ruleset.CLOUDS;
                break;
            case 3 :
                desc.text ="Syntax: 0-6/1,3/2/N\n0-6 Nachbarn - Eine Zelle bleibt am Leben\n1 oder 3 Nachbarn - Eine Zelle wird geboren\n2 - 2 Zustände\nN - Von Neuman Nachbarshaft (nur direkte Nachbarn ohne Diagonalen)";
                ruleset = Ruleset.CRYSTALS;
                break;
            default:
                desc.text = "";
                break;
        }
    }
}