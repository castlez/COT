using Assets.Cards;
using Assets.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardRewardControl : MonoBehaviour
{
    internal class PlayerSelection
    {
        internal int curChoice = 0;
        internal bool lockedIn = false;
        internal List<GameObject> prefabs;
        internal List<CardBase> choices;
    }

    internal List<PlayerSelection> pselects;

    public GameObject paneFab;
    public GameObject cardFab;

    public List<GameObject> panes;

    // inputs
    public float moveInterval;
    public float lastMove;

    public float paneSpacingX;
    public float paneSpacingY;
    public float cardSpacing;

    // single player handling
    public int sCurrentPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        panes = new List<GameObject>() {};
        pselects = new List<PlayerSelection>()
        {
            new PlayerSelection(){ 
                curChoice = 0,
                lockedIn = false,
                prefabs = new List<GameObject>()
            }
        };
        for (int i = 0; i < 4; i++)
        {
            if (i >= GameData.currentPlayers.Count)
            {
                continue;
            }
            // all players besides 1 need a prefab instantiated for them
            // for some reasoni have to set the position again after the instantiate
            Vector3 panePos = paneFab.transform.position;
            GameObject pane = Instantiate(paneFab, new Vector3(panePos.x, panePos.y, panePos.z), Quaternion.identity);
            pane.name = $"PPane{i + 1}";

            // position
            pane.transform.SetParent(gameObject.transform);
            pane.transform.localScale = paneFab.transform.localScale;
            float px = i == 0 || i == 2 ? panePos.x : panePos.x + paneSpacingX;
            float py = i == 0 || i == 1 ? panePos.y : panePos.y + paneSpacingY;
            pane.transform.position = new Vector3(px, py, panePos.z);

            // color
            panes.Add(pane);

            // choice object
            pselects.Add(new PlayerSelection()
            {
                curChoice = 0,
                lockedIn = false,
                prefabs = new List<GameObject>()
            });

            // set player specific bits
            // like the cards they are choosing from and the first one
            // being selected

            // background color
            panes[i].transform.Find("background").GetComponent<SpriteRenderer>().color = GameData.pcolors[i];

            // ready sign
            panes[i].transform.Find("ready").gameObject.SetActive(false);

            // card rewards
            List<CardBase> rewardPool = GameData.currentPlayers[i].cardHandler.GetRewardPool(GameData.floorNumber);
            rewardPool = rewardPool.OrderBy(a => Guid.NewGuid()).ToList();
            pselects[i].choices = rewardPool.GetRange(0, 3);

            // laydown card choices prefabs
            for (int j = 0; j < pselects[i].choices.Count;j++)
            {
                // make sure the prefab is in the list
                if (i == 0)
                {
                    //
                }
                if (j == 0)  // first card is already there
                {
                    pselects[i].prefabs.Add(panes[i].transform.Find("card1").gameObject);
                }
                else
                {
                    Vector3 pos = pselects[i].prefabs[0].transform.position;
                    GameObject card = Instantiate(cardFab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                    card.transform.SetParent(panes[i].transform);
                    card.transform.localScale = cardFab.transform.localScale;
                    card.name = $"card{j+1}-{i+1}";
                    pselects[i].prefabs.Add(card);
                }

                // update the card

                // position
                Vector3 fPos = pselects[i].prefabs[0].transform.position;
                pselects[i].prefabs[j].transform.position = new Vector3(
                        fPos.x + cardSpacing * (float)j,
                        fPos.y,
                        fPos.z
                    );

                // text
                GameObject cname = pselects[i].prefabs[j].transform.Find("name").gameObject;
                GameObject ccost = pselects[i].prefabs[j].transform.Find("cost").gameObject;
                GameObject cdesc = pselects[i].prefabs[j].transform.Find("desc").gameObject;

                cname.GetComponent<TextMesh>().text = pselects[i].choices[j].name;
                ccost.GetComponent<TextMesh>().text = pselects[i].choices[j].cost.ToString();
                cdesc.GetComponent<TextMesh>().text = pselects[i].choices[j].cardText(GameData.currentPlayers[i]);
                SetChosenCard(i, 0);
            }
        }

        paneFab.SetActive(false);
        lastMove = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < GameData.currentPlayers.Count;i++)
        {
            HandleCardSelect(i);
        }
        if (CheckPlayersLockedIn())
        {
            FinishCardSelection();
        }
    }

    void FinishCardSelection()
    {
        for (int i = 0; i < GameData.currentPlayers.Count; i++)
        {
            if (pselects[i].curChoice != -1)
            {
                CardBase toAdd = pselects[i].choices[pselects[i].curChoice];
                GameData.currentPlayers[i].deck.Add(toAdd);
            }
        }

        GameData.floorNumber += 1;

        // TODO go back to the map
        // for now just go back to another combat
        SceneManager.LoadScene(2);  // TODO make dynamic
    }

    bool CheckPlayersLockedIn()
    {
        bool allPlayersLockedIn = true;
        for (int i = 0; i < GameData.currentPlayers.Count; i++)
        {
            if (pselects[i].lockedIn)
            {
                panes[i].transform.Find("ready").gameObject.SetActive(true);
            }
            else
            {
                panes[i].transform.Find("ready").gameObject.SetActive(false);
                allPlayersLockedIn = false;
            }
        }
        return allPlayersLockedIn;
    }

    void SetChosenCard(int playerNum, int choice)
    {
        if (choice >= 0 && choice < pselects[playerNum].choices.Count)
        {
            // first turn off all indicators
            for (int i = 0; i < pselects[playerNum].prefabs.Count; i++)
            {
                pselects[playerNum].prefabs[i].transform.Find("selected").gameObject.SetActive(false);
            }
            GameObject next = pselects[playerNum].prefabs[choice];
            next.transform.Find("selected").gameObject.SetActive(true);
            pselects[playerNum].curChoice = choice;
        }
        else if (choice == -1)
        {
            // why the fuck was this here?!
            //for (int i = 0; i < pselects[playerNum].prefabs.Count; i++)
            //{
            //    pselects[playerNum].prefabs[i].transform.Find("selected").gameObject.SetActive(false);
            //}
        }
    }

    void HandleCardSelect(int playerNum)
    {
        
        if (Time.time - lastMove > moveInterval)
        {
            if (playerNum != sCurrentPlayer)
            {
                return;
            }
            int pnum = playerNum;
            if (Cin.singlePlayer)
            {
                pnum = sCurrentPlayer;
            }

            if (Cin.CheckKey("XBoxHoriz", $"{playerNum + 1}", Cin.greaterThan))
            {
                SetChosenCard(pnum, pselects[pnum].curChoice + 1);
                lastMove = Time.time;
            }
            else if (Cin.CheckKey("XBoxHoriz", $"{playerNum + 1}", Cin.lessThan))
            {
                SetChosenCard(pnum, pselects[pnum].curChoice - 1);
                lastMove = Time.time;
            }
            else if (pselects[pnum].lockedIn)
            {
                if (Cin.CheckKey("XBoxBack", $"{playerNum+1}", Cin.greaterThan))
                {
                    pselects[pnum].lockedIn = false;
                    if (sCurrentPlayer > 0)
                    {
                        sCurrentPlayer -= 1;
                    }
                }
            }
            else
            {
                if (Cin.CheckKey("XBoxAction", $"{playerNum + 1}", Cin.greaterThan))
                {
                    pselects[pnum].lockedIn = true;
                    sCurrentPlayer += 1;
                    lastMove = Time.time;
                }
                else if (Cin.CheckKey("XBoxPassTurn", $"{playerNum + 1}", Cin.greaterThan))
                {
                    pselects[pnum].curChoice = -1;
                    SetChosenCard(pnum, -1);
                    pselects[pnum].lockedIn = true;
                    sCurrentPlayer += 1;
                    lastMove = Time.time;
                }
            }
        }
    }

}
