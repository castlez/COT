using Assets.PlayersClasses;
using Assets.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Cards;

public class PlayerTurn
{
    public int choice;  // which class they are looking at
    public MODE position;  // position in class view, either start deck or select rn
    public bool lookingAtDeck; // technically boolean
    public int dLookPos;  // which card in the deck preview they are looking at
    public int playerIndex;  // index in the player lists
    public string ps;  // player string, used to find game objects
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

    // Turn data
    public List<PlayerTurn> plrs;

    // movement
    public float moveInterval;
    private float last_select;

    // DEBUGGING(?)
    public const bool USE_KEYBOARD = true;  // use keyboard to control player 1
    public bool singlePlayer;  // use keyboard to control all players, basically single player

    private void Awake()
    {
        // TODO check if any active controllers
        singlePlayer = true;
        

        playableClasses = new List<PlayerClassBase>() {
            new Barbarian("0"),
            new ShadowTinker("0")
        };
    }

    void Start()
    {
        // initialize the barbarian as first selected class
        // for all players
        Init();
        

        //GameData.currentPlayers = new List<PlayerClassBase>() {
        //    new Barbarian("1"),
        //    new Barbarian("2")
        //};

        // stand in so indexing is easier, if less than 4 players
        // are present MAKE SURE TO PRUNE THIS TODO TODO TODO
        GameData.currentPlayers = new List<PlayerClassBase>()
        {
            new Barbarian("1"),
            new Barbarian("1"),
            new Barbarian("1"),
            new Barbarian("1")
        };
        SetPlayerClassChoice(0, 0, MODE.READYUP);  // set player 1 to barbarian selecting "Select"
    }

    // Update is called once per frame
    void Update()
    {
        // TODO determine if a player has joined
        // or if single player has added a player for themselves


        // TODO handle players selecting classes
        int i = 0;  // make this a loop for all players
        string pi = $"{i + 1}";
        if (plrs[i].lookingAtDeck)
        {
            HandleDeckView(i);
        }
        else
        {
            HandlePlayerClassSelect(i);
            HandleSelectAction(i);
        }
        

        // check if everyone is ready and move to game if so

        //if (Input.GetAxisRaw("XBoxAction1") > 0f || Input.GetAxisRaw("KeyAction") > 0f)
        //{
        //    SceneManager.LoadScene(1);
        //}
    }

    private void Init()
    {
        plrs = new List<PlayerTurn>();
        for (int i = 0; i < 4; i++)
        {
            plrs.Add(new PlayerTurn() {
                choice=0,
                position=0,  
                lookingAtDeck = false,
                dLookPos=0, 
                playerIndex=i,
                ps=$"{i+1}"
            });
        }

        // hide all deck views
        // TODO do the rest of them?
        int ip = 0;

        GameObject player = GameObject.Find($"PSelect{plrs[ip].ps}").gameObject;
        GameObject deckView = player.transform.Find("DeckView").gameObject;
        deckView.SetActive(false);

    }

    void SetPlayerClassChoice(int playerIndex, int classIndex, MODE interactIndex)
    {
        plrs[playerIndex].choice = classIndex;
        plrs[playerIndex].position = interactIndex;
        PlayerClassBase classChoice = playableClasses[plrs[playerIndex].choice];
        GameObject player = GameObject.Find($"PSelect{plrs[playerIndex].ps}").gameObject;
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
        string pi = $"{playerIndex + 1}";
        // TODO do for all plays in a loop
        if (Cin.CheckKey("XBoxHoriz", $"{playerIndex+1}", Cin.greaterThan))
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

    void HandleSelectAction(int playerIndex)
    {
        PlayerTurn p = plrs[playerIndex];
        if (Cin.CheckKey("XBoxAction", p.ps, Cin.greaterThan))
        {
            if (p.position == MODE.LOOKATDECK)
            {
                p.lookingAtDeck = true;
                GameObject player = GameObject.Find($"PSelect{p.ps}").gameObject;
                GameObject deckView = player.transform.Find("DeckView").gameObject;
                deckView.SetActive(true);
                SetDeckViewChoice(playerIndex, plrs[playerIndex].dLookPos,
                    playableClasses[plrs[playerIndex].choice].GetUniqueCardsInDeck());
            }
        }
    }

    void HandleDeckView(int playerIndex)
    {
        string pi = plrs[playerIndex].ps;
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
            GameObject player = GameObject.Find($"PSelect{plrs[playerIndex].ps}").gameObject;
            GameObject deckView = player.transform.Find("DeckView").gameObject;
            deckView.SetActive(false);
        }
    }

    void SetDeckViewChoice(int playerIndex, int newLook, Dictionary<string, int> cards)
    {
        PlayerTurn p = plrs[playerIndex];
        GameObject player = GameObject.Find($"PSelect{p.ps}").gameObject;
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
        PlayerTurn p = plrs[playerIndex];
        GameObject player = GameObject.Find($"PSelect{p.ps}").gameObject;
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
