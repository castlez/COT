using Assets.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenControl : MonoBehaviour
{
    public GameObject start;
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject exitButton;

    public GameObject numSelect;
    public GameObject singlePlayer;
    public GameObject multiplayer;

    private Color selectedColor;
    private Color deselectedColor;

    private List<GameObject> selections;
    private int curChoice;

    private float last_select;
    public float selectInterval;

    public enum MODE
    {
        START,
        NUMPLAYERS
    }

    private MODE _curMode;

    // Start is called before the first frame update
    void Start()
    {
        numSelect.SetActive(false);

        _curMode = MODE.START;
        selections = GetModeOptions();
        curChoice = 0;

        selectedColor = startButton.GetComponent<TextMesh>().color;
        deselectedColor = settingsButton.GetComponent<TextMesh>().color;
        last_select = Time.time;

        GameObject.FindGameObjectWithTag("music").GetComponent<MusicHandler>().PlayMusic();
    }

    public List<GameObject> GetModeOptions()
    {
        if (_curMode == MODE.START)
        {
            return new List<GameObject>() {
                startButton,
                settingsButton,
                exitButton
            };
        }
        else if (_curMode == MODE.NUMPLAYERS)
        {
            return new List<GameObject>() {
                singlePlayer,
                multiplayer
            };
        }
        else
        {
            throw new System.Exception($"UNKNOWN MODE {_curMode}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleSelect();
        HandleAction();
    }

    void HandleAction()
    {
        if (Time.time - last_select > selectInterval)
        {
            // everyone can control the start screen
            for (int i = 0; i < 4; i++)
            {
                if (Cin.CheckKey("XBoxAction", $"{i + 1}", Cin.greaterThan))
                {
                    if (_curMode == MODE.START)
                    {
                        if (curChoice == 0)
                        {
                            // start game
                            _curMode = MODE.NUMPLAYERS;
                            selections = GetModeOptions();
                            start.SetActive(false);
                            numSelect.SetActive(true);
                            UpdateSelected(0);
                            last_select = Time.time;
                            return;
                        }
                        else if (curChoice == 1)
                        {
                            // settings TODO
                        }
                        else if (curChoice == 2)
                        {
                            // exit game
                            Debug.Log("Exiting game!");
                            Application.Quit();
                        }
                    }
                    else if (_curMode == MODE.NUMPLAYERS)
                    {
                        if (curChoice == 0)
                        {
                            Cin.singlePlayer = true;
                            // start game
                            Debug.Log("Starting Game!");
                            SceneManager.LoadScene(1);
                        }
                        else if (curChoice == 1)
                        {
                            Cin.singlePlayer = false;
                            // start game
                            Debug.Log("Starting Game!");
                            SceneManager.LoadScene(1);
                        }
                    }
                    
                }
                if (Cin.CheckKey("XBoxBack", $"{i + 1}", Cin.greaterThan))
                {
                    if (_curMode == MODE.NUMPLAYERS)
                    {
                        _curMode = MODE.START;
                        selections = GetModeOptions();
                        start.SetActive(true);
                        numSelect.SetActive(false);
                        UpdateSelected(0);
                        last_select = Time.time;
                        return;
                    }
                }
            }
        }
    }

    void HandleSelect()
    {
        if (Time.time - last_select > selectInterval)
        {
            // everyone can control the start screen
            for (int i = 0; i < 4; i++)
            {
                if (Cin.CheckKey("XBoxVert", $"{i + 1}", Cin.greaterThan))
                {
                    if (curChoice - 1 >= 0)
                    {
                        UpdateSelected(curChoice - 1);
                        last_select = Time.time;
                    }
                }
                else if (Cin.CheckKey("XBoxVert", $"{i + 1}", Cin.lessThan))
                {
                    if (curChoice + 1 < selections.Count)
                    {
                        UpdateSelected(curChoice + 1);
                        last_select = Time.time;
                    }
                }
            }
        }
    }

    public void UpdateSelected(int newChoice)
    {
        foreach(GameObject s in selections)
        {
            s.GetComponent<TextMesh>().color = deselectedColor;
        }
        curChoice = newChoice;
        selections[curChoice].GetComponent<TextMesh>().color = selectedColor;
    }
}
