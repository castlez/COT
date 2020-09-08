using Assets.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenControl : MonoBehaviour
{
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject exitButton;

    private Color selectedColor;
    private Color deselectedColor;

    private List<GameObject> selections;
    private int curChoice;

    private float last_select;
    public float selectInterval;

    // Start is called before the first frame update
    void Start()
    {
        selections = new List<GameObject>() {
            startButton,
            settingsButton,
            exitButton
        };
        curChoice = 0;

        selectedColor = startButton.GetComponent<TextMesh>().color;
        deselectedColor = settingsButton.GetComponent<TextMesh>().color;
        last_select = Time.time;

        GameObject.FindGameObjectWithTag("music").GetComponent<MusicHandler>().PlayMusic();
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
                    if (curChoice == 0)
                    {
                        // start game
                        Debug.Log("Starting Game!");
                        SceneManager.LoadScene(1);
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
                        selections[curChoice].GetComponent<TextMesh>().color = deselectedColor;
                        curChoice -= 1;
                        selections[curChoice].GetComponent<TextMesh>().color = selectedColor;
                        last_select = Time.time;
                    }
                }
                else if (Cin.CheckKey("XBoxVert", $"{i + 1}", Cin.lessThan))
                {
                    if (curChoice + 1 < selections.Count)
                    {
                        selections[curChoice].GetComponent<TextMesh>().color = deselectedColor;
                        curChoice += 1;
                        selections[curChoice].GetComponent<TextMesh>().color = selectedColor;
                        last_select = Time.time;
                    }
                }
            }
        }
    }
}
