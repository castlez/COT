using Assets.Cards;
using Assets.Enemies;
using Assets.PlayersClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class FightController : MonoBehaviour
{
    // Local Constants
    public const string PLAYERS = "players";
    public const string ENEMIES = "enemies";

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
    public List<string> phase = new List<string>() {"start", "draw", "play", "end" };
    public int phaseIndex;

    // targetting
    public bool enterTargetMode;
    public bool targetMode;
    public Tuple<string, int> currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        // init players
        // TODO get players from player select scene, for now hard coded to 2 players and 2 enemies
        players = new List<PlayerClassBase>();
        for (int i = 1; i < Meta.CURRENTPLAYERS+1; i++)
        {
            PlayerClassBase player = new Barbarian($"{i}");  // todo this loop stupid, there are other classes (soon)
            player.Init();
            players.Add(player);
        }

        // set turn order
        // TODO for now just all players then all enemies
        currentPlayer = players[0];
        currentTurn = new Tuple<string, int>(PLAYERS, 0); // access with currentTurn.Item1 and currentTurn.Item2
        Debug.Log($"Current player is {currentTurn.Item2}");

        phaseIndex = 0;

        // init enemies TODO make dynamic
        enemies = new List<EnemyBase>();
        EnemyBase e1 = new Bandit("1");
        enemies.Add(e1);
        enemies[0].Init();
        EnemyBase e2 = new Bandit("2");
        enemies.Add(e2);
        enemies[1].Init();

        SetupUi();

        lookCard = 0;
        deselectedY = hand1.transform.position.y;
        selectedY = deselectedY + cardLift;
        last_select = Time.time;
        last_play = Time.time;

        for (int i = 0; i < Meta.MAX_HAND_SIZE; i++)
        {
            // find the card, set it inactive
            GameObject hcard = GameObject.Find($"Hand{i + 1}");
            GameObject hcd = hcard.transform.Find("CardDesc").gameObject;
            hcd.GetComponent<MeshRenderer>().enabled = false; 

            GameObject hcn = hcard.transform.Find("CardName").gameObject;
            hcn.GetComponent<MeshRenderer>().enabled = false;

            GameObject hcc = hcard.transform.Find("CardCost").gameObject;
            hcc.GetComponent<MeshRenderer>().enabled = false;
            hcard.GetComponent<Renderer>().enabled = false;
        }
    }

    void SetupUi()
    {
        // Turn off all indicators
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTurnIndicator(false);
            players[i].SetTargetted(false);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            //enemies[i].SetTurnIndicator(false);
            enemies[i].SetTargetted(false);
        }

        targetMode = false;
        enterTargetMode = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentTurn.Item1 == PLAYERS)
        {
            var passTurn = HandlePlayerTurn();
            if (passTurn)
            {
                if (currentTurn.Item2 + 1 >= players.Count)
                {
                    currentTurn = new Tuple<string, int>(ENEMIES, 0);
                }
                else
                {
                    currentPlayer = players[currentTurn.Item2 + 1];
                    currentTurn = new Tuple<string, int>(PLAYERS, currentTurn.Item2 + 1);
                    phaseIndex = 0;  // set to draw phase of next turn
                }
            }
        }
        if (currentTurn.Item1 == "enemies")
        {
            var passTurn = HandleEnemyTurn();
            if (passTurn)
            {
                if (currentTurn.Item2 + 1 >= enemies.Count)
                {
                    currentPlayer = players[0];
                    currentTurn = new Tuple<string, int>(PLAYERS, 0);
                    phaseIndex = 0;  // set to draw phase of next player turn
                }
                else
                {
                    currentTurn = new Tuple<string, int>(ENEMIES, currentTurn.Item2 + 1);
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
        // check beginning of turn
        if (phaseIndex == 0)
        {
            currentPlayer.StartTurn();
            phaseIndex = 1;
        }
        if (phaseIndex == 1)
        {
            currentPlayer.DrawPhase();
            UpdateHand();
            phaseIndex = 2;
        }

        // play phase

        // Current Player always updates first TODO MULTIPLAYER
        if (Time.time - last_select > moveInterval)
        {
            if (Input.GetAxisRaw($"XBoxHoriz{currentPlayer.playerNum}") > 0f || 
                Input.GetAxisRaw("Horizontal") > 0f)
            {
                if (targetMode)
                {
                    string currentTargetType = currentTarget.Item1;
                    int currentTargetIndex = currentTarget.Item2;
                    if (currentTargetType == ENEMIES)
                    {
                        if (currentTargetIndex + 1 > enemies.Count - 1)
                        {
                            // Wrap around
                            enemies[currentTargetIndex].SetTargetted(false);
                            enemies[0].SetTargetted(true);
                            currentTarget = new Tuple<string, int>(ENEMIES, 0);
                        }
                        else
                        {
                            enemies[currentTargetIndex].SetTargetted(false);
                            enemies[currentTargetIndex + 1].SetTargetted(true);
                            currentTarget = new Tuple<string, int>(ENEMIES, currentTargetIndex + 1);
                        }
                        Debug.Log($"current target: {currentTarget.Item1}, {currentTarget.Item2}");

                    }
                }
                else
                {
                    updateCardSelect(lookCard, lookCard + 1);
                }
            }
            else if (Input.GetAxisRaw($"XBoxHoriz{currentPlayer.playerNum}") < 0f ||
                     Input.GetAxisRaw("Horizontal") < 0f)
            {
                 if (targetMode)
                 {
                    string currentTargetType = currentTarget.Item1;
                    int currentTargetIndex = currentTarget.Item2;
                    if (currentTargetType == ENEMIES)
                    {
                        if (currentTargetIndex - 1 < 0)
                        {
                            // Wrap around
                            enemies[currentTargetIndex].SetTargetted(false);
                            enemies[players.Count-1].SetTargetted(true);
                            currentTarget = new Tuple<string, int>(ENEMIES, players.Count-1);
                            Debug.Log("move left");
                        }
                        else
                        {
                            enemies[currentTargetIndex].SetTargetted(false);
                            enemies[currentTargetIndex-1].SetTargetted(true);
                            currentTarget = new Tuple<string, int>(ENEMIES, currentTargetIndex-1);
                            Debug.Log("move left");
                        }
                        Debug.Log($"current target: {currentTarget.Item1}, {currentTarget.Item2}");

                    }
                }
                 else
                 {
                    updateCardSelect(lookCard, lookCard - 1);
                 }

            }
        }

        if (Time.time - last_play > playInterval)
        {
            if (Input.GetAxisRaw($"XBoxPassTurn{currentPlayer.playerNum}") > 0f ||
                Input.GetAxisRaw("KeyPass") == 1f)
            {
                currentPlayer.EndPhase();
                return true;
            }

            if (Input.GetAxisRaw($"XBoxAction{currentPlayer.playerNum}") > 0f ||
                Input.GetAxisRaw("KeyAction") > 0f)
            {
                if (currentPlayer.hand.Count > 0)
                {
                    if (lookCard < currentPlayer.hand.Count)
                    {
                        if (Time.time - last_play > 5f && targetMode)  // TODO CONSTANT for 5f
                        {
                            CardBase toPlay = currentPlayer.hand[lookCard];
                            if (toPlay.targetType == TargetTypes.ENEMY)
                            {
                                int ti = currentTarget.Item2;
                                currentPlayer.playCard(lookCard, enemies[ti]);
                                Debug.Log("Getting there1");
                            }
                            if (toPlay.targetType == TargetTypes.PLAYER)
                            {
                                int ti = currentTarget.Item2;
                                currentPlayer.playCard(lookCard, players[ti]);
                            }
                            else if (toPlay.targetType == TargetTypes.SELF)
                            {
                                currentPlayer.playCard(lookCard, currentPlayer);
                            }
                            UpdateHand();
                            targetMode = false;
                        }
                        else
                        {
                            TargetTypes targetType = currentPlayer.hand[lookCard].targetType;
                            if (targetType == TargetTypes.ENEMY)
                            {
                                currentTarget = new Tuple<string, int>(ENEMIES, 0);
                                enemies[0].SetTargetted(true);
                                Debug.Log("Getting there2");
                            }
                            else if (targetType == TargetTypes.PLAYER)
                            {
                                currentTarget = new Tuple<string, int>(PLAYERS, 0);
                                players[0].SetTargetted(true);
                            }
                            targetMode = true;
                        }
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
        for (int i = 0; i < Meta.MAX_HAND_SIZE; i++)  
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
                hcd.GetComponent<TextMesh>().text = players[playerIndex].hand[i].cardText(players[playerIndex]);  //TODO this needs to be current player not first player

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
