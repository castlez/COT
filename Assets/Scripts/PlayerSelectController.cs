using Assets.PlayersClasses;
using Assets.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Cards;

public class PlayerController
{
    public int choice;  // which class they are looking at
    public MODE position;  // position in class view, either start deck or select rn
    public bool lookingAtDeck; // technically boolean
    public int dLookPos;  // which card in the deck preview they are looking at
    public int playerIndex;  // index in the player lists
    public string ctrl;  // player string, used to find game objects
    public bool ready;  // whether the player has selected their character
}

public enum MODE
{
    READYUP,
    LOOKATDECK
}
public class PlayerSelectController : MonoBehaviour
{
    
    // Start is called before the first frame update
    //public GameData gameData;
    public List<PlayerClassBase> playableClasses;

    public static int maxPlayers = 4;

    // Turn data
    public List<PlayerController> plrs;

    // movement
    public float moveInterval;
    private float last_select;

    // single player
    public int sCurrentPlayer = 0;

    // button hints
    public GameObject singleHints;
    public GameObject multiHints;

    private void Awake()
    {
        // TODO check if any active controllers
        // TODO make it so one person can have multiple heroes
        //List<string> controllers = Cin.checkControllers();
        //Cin.singlePlayer = false;  // TODO this means single player always uses keyboard

        playableClasses = new List<PlayerClassBase>() {
            new Barbarian("0"),
            new ShadowTinker("0")
        };
    }

    PlayerClassBase GetClass(int pi, string name)
    {
        switch(name)
        {
            case "Barbarian":
                return new Barbarian(plrs[pi].ctrl)
                {
                    pNum = $"{pi + 1}"
                };
            case "ShadowTinker":
                return new ShadowTinker(plrs[pi].ctrl)
                {
                    pNum = $"{pi + 1}"
                };
            default:
                return null;
        }
    }

    void Start()
    {
        // initialize the barbarian as first selected class
        // for all players, as well as button hints
        Init();
        
        // stand in so indexing is easier, if less than 4 players
        // are present MAKE SURE TO PRUNE THIS TODO TODO TODO
        GameData.currentPlayers = new List<PlayerClassBase>()
        {
            new Barbarian("1"),
            new Barbarian("1"),
            new Barbarian("1"),
            new Barbarian("1")
        };
    }

    // Update is called once per frame
    void Update()
    {
        // TODO determine if a player has joined
        // or if single player has added a player for themselves

        if (plrs.Count == 0)
        {
            CheckForNewPlayers();
        }
        else
        {
            CheckForNewPlayers();
            for (int i = 0; i < plrs.Count; i++)
            {
                if (plrs[i].lookingAtDeck)
                {
                    HandleDeckView(i);
                }
                else
                {
                    HandlePlayerClassSelect(i);
                    HandleSelectAction(i);
                }
            }
        }


        // check if everyone is ready and move to game if so

        if (CheckAllPlayersReady())
        {
            // cement the changes and head to the next scene
            GameData.currentPlayers = new List<PlayerClassBase>();
            for (int i=0;i<plrs.Count;i++)
            {
                string cname = playableClasses[plrs[i].choice].GetType().Name;
                PlayerClassBase np = GetClass(i, cname);
                GameData.currentPlayers.Add(np);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Init()
    {
        // init button hints
        singleHints.SetActive(Cin.singlePlayer);
        multiHints.SetActive(!Cin.singlePlayer);

        // Init player list
        plrs = new List<PlayerController>();
        
        // hide all deck views and selectors at first
        for (int i = 0; i < maxPlayers;i++) 
        {
            string num = $"{i+1}";
            GameObject player = GameObject.Find($"PSelect{num}").gameObject;
            GameObject deckView = player.transform.Find("DeckView").gameObject;
            deckView.SetActive(false);
            player.SetActive(false);
        }
    }

    bool CheckAllPlayersReady()
    {
        if (plrs.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < plrs.Count; i++)
        {
            if (!plrs[i].ready)
            {
                return false;
            }
        }
        return true;
    }

    void CheckForNewPlayers()
    {
        if (Time.time - last_select > moveInterval)
        {
            string ctrl;
            string pnum;
            bool sAddedPlayer = false;
            for (int i = 0; i < maxPlayers; i++)
            {
                if (i >= plrs.Count)
                {
                    pnum = $"{i + 1}";
                    if (Cin.singlePlayer)
                    {
                        ctrl = "1";
                    }
                    else
                    {
                        // if we are playing multiplayer, the controller number 
                        // and the pane number are the same?
                        ctrl = pnum;
                    }

                    bool passPressed = false;
                    if (Cin.CheckKey("XBoxPass", "1", Cin.greaterThan))
                    {
                        passPressed = true;
                    }
                    
                    if (Cin.CheckKey("XBoxAction", ctrl, Cin.greaterThan) ||
                        (Cin.singlePlayer && passPressed))
                    {
                        if (passPressed && plrs.Count == 0)
                        {
                            return;
                        }
                        int pind = plrs.Count;
                        bool already = false;
                        for (int jz = 0; jz < plrs.Count; jz++)
                        {
                            if (plrs[jz].ctrl == ctrl)
                            {
                                already = true;
                                break;
                            }
                        }
                        bool newSHero = false;
                        if (Cin.CheckKey("XBoxPass", "1", Cin.greaterThan))
                        {
                            newSHero = true;
                        }
                        if (!already || (Cin.singlePlayer && newSHero && !sAddedPlayer))
                        {
                            plrs.Add(new PlayerController()
                            {
                                choice = 0,
                                position = 0,
                                lookingAtDeck = false,
                                dLookPos = 0,
                                playerIndex = pind,
                                ctrl = ctrl
                            });

                            // show the selector
                            GameObject cvs = GameObject.Find("Canvas").gameObject;
                            GameObject player = cvs.transform.Find($"PSelect{pind + 1}").gameObject;
                            player.SetActive(true);
                            SetPlayerClassChoice(pind, 0, MODE.READYUP);
                            sAddedPlayer = true;  // single player
                            if (Cin.singlePlayer && passPressed)
                            {
                                SetPlayerReady(pind-1, true);
                            }
                            last_select = Time.time;
                        }
                    }
                }
            }
        }
    }

    bool PlayerIsActive(int playerNum)
    {
        for (int i = 0; i< plrs.Count;i++)
        {
            if (plrs[i].playerIndex == playerNum)
            {
                return true;
            }
        }
        return false;
    }

    void SetPlayerClassChoice(int playerIndex, int classIndex, MODE interactIndex)
    {
        plrs[playerIndex].choice = classIndex;
        plrs[playerIndex].position = interactIndex;
        PlayerClassBase classChoice = playableClasses[plrs[playerIndex].choice];
        GameObject player = GameObject.Find($"PSelect{playerIndex+1}").gameObject;
        GameObject className = player.transform.Find("ClassName").gameObject;
        GameObject sprite = player.transform.Find("ClassSprite").gameObject;
        GameObject desc = player.transform.Find("ClassDescription").gameObject;
        GameObject res = player.transform.Find("ResourceValue").gameObject;

        GameObject startingD = player.transform.Find("StartingDeck").gameObject;
        GameObject sdSelect = startingD.transform.Find("Selected").gameObject;
        GameObject selectClass = player.transform.Find("SelectButton").gameObject;
        GameObject scSelect = selectClass.transform.Find("Selected").gameObject;

        className.GetComponent<MeshRenderer>().enabled = true;
        className.GetComponent<TextMesh>().text = classChoice.GetType().Name;
        sprite.GetComponent<SpriteRenderer>().enabled = true;
        sprite.GetComponent<SpriteRenderer>().sprite = classChoice.GetSprite();
        desc.GetComponent<MeshRenderer>().enabled = true;
        desc.GetComponent<TextMesh>().text = classChoice.classDescription;
        res.GetComponent<MeshRenderer>().enabled = true;
        res.GetComponent<TextMesh>().text = classChoice.resourceName + " " + classChoice.maxResource;

        if (plrs[playerIndex].position == MODE.READYUP)
        {
            scSelect.GetComponent<SpriteRenderer>().enabled = true;
            sdSelect.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (plrs[playerIndex].position == MODE.LOOKATDECK)
        {
            scSelect.GetComponent<SpriteRenderer>().enabled = false;
            sdSelect.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            Debug.LogError("HOW DID I GET HERE?");
        }
    }

    void HandlePlayerClassSelect(int playerIndex)
    {
        if (!plrs[playerIndex].ready)
        {
            string pi = plrs[playerIndex].ctrl;
            if (Cin.CheckKey("XBoxHoriz", pi, Cin.greaterThan))
            {
                if (plrs[playerIndex].choice + 1 < playableClasses.Count)
                {
                    SetPlayerClassChoice(playerIndex, plrs[playerIndex].choice + 1, plrs[playerIndex].position);
                }
            }
            if (Cin.CheckKey("XBoxHoriz", pi, Cin.lessThan))
            {
                if (plrs[playerIndex].choice - 1 >= 0)
                {
                    SetPlayerClassChoice(playerIndex, plrs[playerIndex].choice - 1, plrs[playerIndex].position);
                }
            }
            if (Cin.CheckKey("XBoxVert", pi, Cin.greaterThan))
            {
                if (plrs[playerIndex].position == MODE.READYUP)
                {
                    SetPlayerClassChoice(playerIndex, plrs[playerIndex].choice, MODE.LOOKATDECK);
                }
            }
            if (Cin.CheckKey("XBoxVert", pi, Cin.lessThan))
            {
                if (plrs[playerIndex].position == MODE.LOOKATDECK)
                {
                    SetPlayerClassChoice(playerIndex, plrs[playerIndex].choice, MODE.READYUP);
                }
            }
        }
    }

    void HandleSelectAction(int playerIndex)
    {
        if (!PlayerIsActive(playerIndex))
        {
            return;
        }
        if (Time.time - last_select > moveInterval)
        {
            PlayerController p = plrs[playerIndex];
            if (!p.ready)
            {
                if (Cin.CheckKey("XBoxAction", p.ctrl, Cin.greaterThan))
                {

                    GameObject player = GameObject.Find($"PSelect{playerIndex + 1}").gameObject;
                    if (p.position == MODE.LOOKATDECK)
                    {
                        p.lookingAtDeck = true;
                        GameObject deckView = player.transform.Find("DeckView").gameObject;
                        deckView.SetActive(true);
                        SetDeckViewChoice(playerIndex, plrs[playerIndex].dLookPos,
                            playableClasses[plrs[playerIndex].choice].GetUniqueCardsInDeck());
                    }
                    else if (p.position == MODE.READYUP)
                    {
                        SetPlayerReady(playerIndex, true);
                    }
                    last_select = Time.time;
                }
            }
            else
            {
                if (Cin.CheckKey("XBoxBack", p.ctrl, Cin.greaterThan))
                {
                    SetPlayerReady(playerIndex, false);
                }
            }
        }
    }

    void SetPlayerReady(int playerIndex, bool ready)
    {
        PlayerController p = plrs[playerIndex];
        p.ready = ready;

        GameObject player = GameObject.Find($"PSelect{playerIndex + 1}").gameObject;
        GameObject sb = player.transform.Find("SelectButton").gameObject;
        sb.GetComponent<TextMesh>().text = ready ? "Ready": "Select";
    }

    void HandleDeckView(int playerIndex)
    {
        string pi = plrs[playerIndex].ctrl;
        Dictionary<string, int> cards = playableClasses[plrs[playerIndex].choice].GetUniqueCardsInDeck();
        if (Cin.CheckKey("XBoxVert", pi, Cin.lessThan))
        {
            if (plrs[playerIndex].dLookPos + 1 < cards.Count)
            {
                SetDeckViewChoice(playerIndex, plrs[playerIndex].dLookPos + 1, cards);
            }
        }
        if (Cin.CheckKey("XBoxVert", pi, Cin.greaterThan))
        {
            if (plrs[playerIndex].dLookPos - 1 >= 0)
            {
                SetDeckViewChoice(playerIndex, plrs[playerIndex].dLookPos - 1, cards);
            }
        }
        if (Cin.CheckKey("XBoxBack", pi, Cin.greaterThan))
        {
            plrs[playerIndex].lookingAtDeck = false;
            GameObject player = GameObject.Find($"PSelect{plrs[playerIndex].playerIndex + 1}").gameObject;
            GameObject deckView = player.transform.Find("DeckView").gameObject;
            deckView.SetActive(false);
        }
    }

    void SetDeckViewChoice(int playerIndex, int newLook, Dictionary<string, int> cards)
    {
        PlayerController p = plrs[playerIndex];
        GameObject player = GameObject.Find($"PSelect{p.playerIndex + 1}").gameObject;
        GameObject deckView = player.transform.Find("DeckView").gameObject;

        int selected = newLook;
        // disable all except the one we selected
        if (Time.time - last_select > moveInterval)
        {
            List<string> cardKeys = new List<string>(cards.Keys);
            CardBase selectedCard = null;
            for (int i = 0; i < 6; i++)  // TODO this needs to be dynamic with prefabs
            {
                GameObject card = deckView.transform.Find($"Card{i + 1}").gameObject;
                GameObject goSelect = card.transform.Find("Selected").gameObject;
                if (i < cardKeys.Capacity)
                {
                    CardBase c = playableClasses[p.choice].GetCardInDeckByName(cardKeys[i]);
                    if (i == selected)
                    {
                        goSelect.GetComponent<SpriteRenderer>().enabled = true;
                        selectedCard = c;
                    }
                    else
                    {
                        goSelect.GetComponent<SpriteRenderer>().enabled = false;
                    }
                    card.GetComponent<TextMesh>().text = c.name + " x" + cards[cardKeys[i]];
                }
                if (i >= cards.Count)
                {
                    card.GetComponent<MeshRenderer>().enabled = false;
                    goSelect.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            p.dLookPos = newLook;
            SetDeckViewPreview(playerIndex, selectedCard);
            last_select = Time.time;
        }
    }

    void SetDeckViewPreview(int playerIndex, CardBase card)
    {
        PlayerController p = plrs[playerIndex];
        GameObject player = GameObject.Find($"PSelect{p.playerIndex + 1}").gameObject;
        GameObject deckView = player.transform.Find("DeckView").gameObject;
        GameObject preview = deckView.transform.Find("Preview").gameObject;
        
        GameObject pName = preview.transform.Find("Name").gameObject;
        GameObject pCost = preview.transform.Find("Cost").gameObject;
        GameObject pDesc = preview.transform.Find("Description").gameObject;

        pName.GetComponent<TextMesh>().text = card.name;
        pCost.GetComponent<TextMesh>().text = $"{card.cost}";
        pDesc.GetComponent<TextMesh>().text = card.cardText(playableClasses[p.choice]);
        
    }
}
