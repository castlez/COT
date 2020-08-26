using Assets.Cards;
using Assets.Enemies;
using Assets.PlayersClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;


public class FightController : MonoBehaviour
{
    // Local Constants
    public const string PLAYERS = "players";
    public const string ENEMIES = "enemies";

    // DEBUGGING(?)
    public const bool USE_KEYBOARD = true;  // use keyboard to control player 1
    public const bool KEYBOARD_ALL = true;  // use keyboard to contrall all players

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

    // Time Intervals
    public float moveInterval;  // non-target movement
    public float playInterval;  // how often cards can selected
    public float targetInterval; //target movement
    public float targetActionInterval;  // how long after entering targetting before target can change

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

    void finishCombat(bool playersWin)
    {

    }

    public delegate bool COperator(float a, float b);
    public COperator lessThan = delegate (float a, float b)
    {
        return a < b;
    };
    public COperator greaterThan = delegate (float a, float b)
    {
        return a > b;
    };

    bool CheckKey(string keyName, string pNum, COperator op)
    {
        float keyCheck;
        if (USE_KEYBOARD)
        {
            if (pNum == "1" || KEYBOARD_ALL)
            {
                // if the keyboard is in use, shift controllers down
                if (keyName == "XBoxPassTurn")
                {
                    keyCheck = Input.GetAxisRaw("KeyPass");
                }
                else if( keyName == "XBoxAction")
                {
                    keyCheck = Input.GetAxisRaw("KeyAction");
                }
                else if(keyName == "XBoxHoriz")
                {
                    keyCheck = Input.GetAxisRaw("Horizontal");
                }
                else
                {
                    Debug.LogError($"CheckKey recieved unknown key '{keyName}' with keyboard enabled!");
                    return false;
                }
            }
            else
            {
                int num = int.Parse(pNum) - 1;
                keyCheck = Input.GetAxisRaw($"{keyName}{num}");
            }
        }
        else  // shows as unreachable if USE_KEYBOARD is true
        {
            keyCheck = Input.GetAxisRaw($"{keyName}{pNum}");
        }
        if(op(keyCheck, 0))
        {
            return true;
        }
        return false;
    }

    void DisableTargetIndicators()
    {
        // Turn off all indicators
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTargetted(false);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            //enemies[i].SetTurnIndicator(false);
            enemies[i].SetTargetted(false);
        }
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
        CleanUpDead();
    }

    void CleanUpDead()
    {
        // players
        players = players.Where(p => p.Hp > 0).ToList();
        // enemies
        enemies = enemies.Where(e => e.Hp > 0).ToList();
    }

    bool HandleEnemyTurn()
    {
        if (enemies.Count == 0)
        {
            finishCombat(true);
            return true;
        }
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

        // Handle movement
        if (targetMode)
        {
            HandleTargetMode();
            HandleTargetAction();
        }
        else
        {
            HandleCardSelect();

            if (HandlePassTurn())
            {
                return true;
            }

            HandleSelectAction();
        }

        // handle action (press A or space)

        // if we didnt already pass turn, say we arent passing turn
        return false;
    }

    void PlayCard(object target)
    {
        bool casted = currentPlayer.playCard(lookCard, target);
        if (casted)
        {
            UpdateHand();
            DisableTargetIndicators();
            targetMode = false;

            // after playing a card, move to the card to the left 
            if (lookCard - 1 < 0)
            {
                updateCardSelect(lookCard, lookCard);
            }
            else
            {
                updateCardSelect(lookCard, lookCard - 1);
            }
        }
    }

    bool HandlePassTurn()
    {
        if (Time.time - last_play > playInterval)
        {
            if (CheckKey($"XBoxPassTurn", currentPlayer.playerNum, greaterThan))
            {
                currentPlayer.EndPhase();
                last_play = Time.time;
                return true;
            }
        }
        return false;
    }

    void HandleSelectAction()
    {
        if (currentPlayer.hand.Count == 0 || enemies.Count == 0)
        {
            return;
        }
        if (Time.time - last_play > playInterval)
        {
            if (CheckKey("XBoxAction", currentPlayer.playerNum, greaterThan))
            {
                TargetTypes targetType = currentPlayer.hand[lookCard].targetType;
                if (targetType == TargetTypes.ENEMY)
                {
                    currentTarget = new Tuple<string, int>(ENEMIES, 0);
                    enemies[0].SetTargetted(true);
                    targetMode = true;
                    last_play = Time.time;
                }
                else if (targetType == TargetTypes.PLAYER)
                {
                    currentTarget = new Tuple<string, int>(PLAYERS, 0);
                    players[0].SetTargetted(true);
                    targetMode = true;
                    last_play = Time.time;
                }
                else if (targetType == TargetTypes.SELF)
                {
                    currentTarget = new Tuple<string, int>(PLAYERS, currentTurn.Item2);
                    players[currentTurn.Item2].SetTargetted(true);
                    targetMode = true;
                    last_play = Time.time;
                }
            }
        }
    }

    void HandleTargetAction()
    {
        if (currentPlayer.hand.Count == 0)
        {
            return;
        }
        if (Time.time - last_play > targetActionInterval)
        {
            if (CheckKey("XBoxAction", currentPlayer.playerNum, greaterThan))
            {
                Debug.Log($"Casting {currentPlayer.hand[lookCard].name}");

                CardBase toPlay = currentPlayer.hand[lookCard];
                if (toPlay.targetType == TargetTypes.ENEMY)
                {
                    int ti = currentTarget.Item2;
                    PlayCard(enemies[ti]);
                    last_play = Time.time;
                }
                else if (toPlay.targetType == TargetTypes.PLAYER)
                {
                    int ti = currentTarget.Item2;
                    PlayCard(players[ti]);
                    last_play = Time.time;
                }
                else if (toPlay.targetType == TargetTypes.SELF)
                {
                    PlayCard(currentPlayer);
                    last_play = Time.time;
                }
                else
                {
                    Debug.Log($"UNKNOWN CARD TYPE");
                }
            }
        }
    }

    void HandleCardSelect()
    {
        // card select
        if (Time.time - last_select > moveInterval)
        {
            if (CheckKey("XBoxHoriz", currentPlayer.playerNum, greaterThan))
            {
                DisableTargetIndicators();
                updateCardSelect(lookCard, lookCard + 1);
                last_select = Time.time;
            }
            else if (CheckKey("XBoxHoriz", currentPlayer.playerNum, lessThan))
            {
                DisableTargetIndicators();
                updateCardSelect(lookCard, lookCard - 1);
                last_select = Time.time;
            }
        }
    }

    void HandleTargetMode()
    {
        if (Time.time - last_select > targetInterval)
        {
            if (CheckKey("XBoxHoriz", currentPlayer.playerNum, lessThan))
            {
                string currentTargetType = currentTarget.Item1;
                int currentTargetIndex = currentTarget.Item2;
                if (currentTargetType == ENEMIES)
                {
                    if (currentTargetIndex + 1 > enemies.Count - 1)
                    {
                        // Wrap around
                        //enemies[currentTargetIndex].SetTargetted(false);
                        //enemies[0].SetTargetted(true);
                        //currentTarget = new Tuple<string, int>(ENEMIES, 0);
                        last_select = Time.time;
                    }
                    else
                    {
                        enemies[currentTargetIndex].SetTargetted(false);
                        enemies[currentTargetIndex + 1].SetTargetted(true);
                        currentTarget = new Tuple<string, int>(ENEMIES, currentTargetIndex + 1);
                        last_select = Time.time;
                    }
                    Debug.Log($"current target: {currentTarget.Item1}, {currentTarget.Item2}");
                }
            }
            if (CheckKey("XBoxHoriz", currentPlayer.playerNum, greaterThan))
            {
                string currentTargetType = currentTarget.Item1;
                int currentTargetIndex = currentTarget.Item2;
                if (currentTargetType == ENEMIES)
                {
                    if (currentTargetIndex - 1 < 0)
                    {
                        // Wrap around
                        //enemies[currentTargetIndex].SetTargetted(false);
                        //enemies[0].SetTargetted(true);
                        //currentTarget = new Tuple<string, int>(ENEMIES, 0);
                        last_select = Time.time;
                    }
                    else
                    {
                        enemies[currentTargetIndex].SetTargetted(false);
                        enemies[currentTargetIndex - 1].SetTargetted(true);
                        currentTarget = new Tuple<string, int>(ENEMIES, currentTargetIndex - 1);
                        last_select = Time.time;
                    }
                    Debug.Log($"current target: {currentTarget.Item1}, {currentTarget.Item2}");
                }
            }
        }
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
