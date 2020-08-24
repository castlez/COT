using Assets.Enemies;
using Assets.PlayersClasses;
using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour
{
    // positions for selected cards of current player
    private float deselectedY;
    private float selectedY;

    // current turn players hand
    public GameObject hand1;
    public GameObject hand2;
    public GameObject hand3;
    public GameObject hand4;
    public GameObject hand5;

    // cards in the players hand
    private List<GameObject> hand;

    // which card the current player has selected
    public int lookCard;

    // how far up a selected card moves
    public float cardLift;

    // last time a card selection changed
    private float last_select;
    public float last_play;

    // how often a player can switch which thing they are looking at
    // and play cards
    public float moveInterval;
    public float playInterval;

    // currently active player
    public PlayerClassBase currentPlayer;

    // holds the players and enemies and turn order
    public List<PlayerClassBase> players;
    public List<EnemyBase> enemies;
    public Tuple<string, int> currentTurn;
    public List<int> takenFirstTurn;

    // turn phases
    public List<string> phase = new List<string>() {"draw", "play", "end" };
    public int phaseIndex;

    // Start is called before the first frame update
    void Start()
    {
        // init players
        players = new List<PlayerClassBase>();
        PlayerClassBase p1 = new Barbarian("1");
        players.Add(p1);

        currentPlayer = players[0];
        currentPlayer.Init();
        
        // init enemies
        enemies = new List<EnemyBase>();
        EnemyBase e1 = new Bandit("1");
        enemies.Add(e1);
        enemies[0].Init();

        takenFirstTurn = new List<int>();

        // set turn order
        // TODO for now just all players then all enemies
        currentTurn = new Tuple<string, int>("players", 0); // access with currentTurn.Item1 and currentTurn.Item2
        phaseIndex = 0;

        lookCard = 0;
        deselectedY = hand1.transform.position.y;
        selectedY = deselectedY + cardLift;
        last_select = Time.time;
        last_play = Time.time;

        for (int i = 0; i < Settings.MAX_HAND_SIZE; i++)
        {
            // find the card, set it inactive
            GameObject hcard = GameObject.Find($"Hand{i + 1}");
            GameObject hcd = hcard.transform.Find("CardDesc").gameObject;
            hcd.GetComponent<MeshRenderer>().enabled = false;  //TODO this needs to be current player not first player

            GameObject hcn = hcard.transform.Find("CardName").gameObject;
            hcn.GetComponent<MeshRenderer>().enabled = false;

            GameObject hcc = hcard.transform.Find("CardCost").gameObject;
            hcc.GetComponent<MeshRenderer>().enabled = false;
            hcard.GetComponent<Renderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentTurn.Item1 == "players")
        {
            var passTurn = HandlePlayerTurn();
            if (passTurn)
            {
                if (currentTurn.Item2 >= players.Count - 1)
                {
                    currentTurn = new Tuple<string, int>("enemies", 0);
                }
                else
                {
                    currentPlayer = players[currentTurn.Item2 + 1];
                    currentTurn = new Tuple<string, int>("players", currentTurn.Item2 + 1);
                    phaseIndex = 0;  // set to draw phase of next turn
                }
            }
        }
        if (currentTurn.Item1 == "enemies")
        {
            var passTurn = HandleEnemyTurn();
            if (passTurn)
            {
                if (currentTurn.Item2 >= enemies.Count - 1)
                {
                    currentTurn = new Tuple<string, int>("players", 0);
                    phaseIndex = 0;  // set to draw phase of next player turn
                }
                else
                {
                    currentTurn = new Tuple<string, int>("enemies", currentTurn.Item2 + 1);
                }
            }
        }
    }

    bool HandleEnemyTurn()
    {
        return enemies[currentTurn.Item2].TakeTurn(players, enemies);
    }

    bool HandlePlayerTurn()
    {
        if (phaseIndex == 0)
        {
            if (takenFirstTurn.Contains(currentTurn.Item2))
            {
                if (currentPlayer.hand.Count + 1 <= Settings.MAX_HAND_SIZE)
                {
                    currentPlayer.drawCard();
                }
            }
            else
            {
                for (int i = 0; i < Settings.STARTING_HAND_SIZE;i++)
                {
                    currentPlayer.drawCard();
                }
                takenFirstTurn.Add(currentTurn.Item2);
            }
            
            UpdateHand();
            phaseIndex = 1;
        }

        // Current Player always updates first
        if (Time.time - last_select > moveInterval)
        {
            if (Input.GetAxisRaw("XBoxHoriz1") > 0f || Input.GetAxisRaw("Horizontal") > 0f)
            {
                updateCardSelect(lookCard, lookCard + 1);
            }
            else if (Input.GetAxisRaw("XBoxHoriz1") < 0f || Input.GetAxisRaw("Horizontal") < 0f)
            {
                updateCardSelect(lookCard, lookCard - 1);
            }
        }

        if (Time.time - last_play > playInterval)
        {
            if (Input.GetAxisRaw("PassTurn") > 0f)
            {
                return true;
            }

            if (Input.GetAxisRaw("Action") > 0f)
            {
                if (currentPlayer.hand.Count > 0)
                {
                    if (lookCard < currentPlayer.hand.Count)
                    {
                        currentPlayer.playCard(lookCard, enemies[0]);
                        UpdateHand();
                    }
                }
            }
        }
        return false;
    }

    GameObject getCardInHand(int number)
    {
        switch (number)
        {
            case 0:
                return hand1;
            case 1:
                return hand2;
            case 2:
                return hand3;
            case 3:
                return hand4;
            case 4:
                return hand5;            
            default:
                return null;
        }
    }

    void UpdateHand()
    {
        for (int i = 0; i < Settings.MAX_HAND_SIZE; i++)  
        {
            // find the card, set it inactive
            GameObject hcard = GameObject.Find($"Hand{i + 1}");
            GameObject hcd = hcard.transform.Find("CardDesc").gameObject;
            hcd.GetComponent<MeshRenderer>().enabled = false;  //TODO this needs to be current player not first player

            GameObject hcn = hcard.transform.Find("CardName").gameObject;
            hcn.GetComponent<MeshRenderer>().enabled = false;

            GameObject hcc = hcard.transform.Find("CardCost").gameObject;
            hcc.GetComponent<MeshRenderer>().enabled = false;
            hcard.GetComponent<Renderer>().enabled = false;
        }

        int playerIndex = int.Parse(currentPlayer.playerNum) - 1;

        for (int i = 0; i < currentPlayer.hand.Count; i++)  
        {
            try
            {
                GameObject hcard = GameObject.Find($"Hand{i + 1}");

                GameObject hcd = hcard.transform.Find("CardDesc").gameObject;
                hcd.GetComponent<MeshRenderer>().enabled = true;
                hcd.GetComponent<TextMesh>().text = players[playerIndex].hand[i].cardText;  //TODO this needs to be current player not first player

                GameObject hcn = hcard.transform.Find("CardName").gameObject;
                hcn.GetComponent<MeshRenderer>().enabled = true;
                hcn.GetComponent<TextMesh>().text = players[playerIndex].hand[i].name;

                GameObject hcc = hcard.transform.Find("CardCost").gameObject;
                hcc.GetComponent<MeshRenderer>().enabled = true;
                hcc.GetComponent<TextMesh>().text = players[playerIndex].hand[i].cost.ToString();

                hcard.GetComponent<Renderer>().enabled = true;
                last_play = Time.time;
            }
            catch (System.Exception)
            {

                Debug.Log($"DIED on Hand{i + 1}, current hand size is {currentPlayer.hand.Count}");

            }
        }
    }

    void updateCardSelect(int oldCardIndex, int newCardIndex)
    {
        if (newCardIndex > currentPlayer.hand.Count - 1 || newCardIndex < 0)
        {
            return;
        }

        GameObject oldCard = getCardInHand(oldCardIndex);
        GameObject newCard = getCardInHand(newCardIndex);
        

        oldCard.transform.position = Vector2.MoveTowards(
            new Vector2(oldCard.transform.position.x, 
                        oldCard.transform.position.y), 
            new Vector2(oldCard.transform.position.x, 
                        deselectedY), 
            cardLift * Time.deltaTime);

        newCard.transform.position = Vector2.MoveTowards(
            new Vector2(newCard.transform.position.x, 
                        newCard.transform.position.y), 
            new Vector2(newCard.transform.position.x, 
                        selectedY), 
            cardLift * Time.deltaTime);
        
        if (lookCard == oldCardIndex)
        {
            lookCard = newCardIndex;
            last_select = Time.time;
        }
    }
}
