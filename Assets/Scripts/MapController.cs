using Assets.PlayersClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameData gameData;

    private void Awake()
    {
    }

    void Start()
    {
        GameData.currentPlayers = new List<PlayerClassBase>() {
            new Barbarian("1"),
            new Barbarian("2")
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("XBoxAction1") > 0f || Input.GetAxisRaw("KeyAction") > 0f)
        {
            SceneManager.LoadScene(1);
        }
    }
}
