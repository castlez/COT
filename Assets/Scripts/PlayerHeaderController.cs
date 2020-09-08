using Assets.Cards;
using Assets.PlayersClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeaderController : MonoBehaviour
{
    public GameObject paneFab;
    public List<GameObject> panes;

    public const float STATUS_SPACING = 0.4f;
    public const float PANE_SPACING = 3.45f;

    // Start is called before the first frame update
    void Start()
    {
        // only render players headers for current players
        // init list of panes with player 1
        panes = new List<GameObject>() {
            paneFab
        };
        for (int i = 0; i < 4;i++)
        {
            if (i >= GameData.currentPlayers.Count)
            {
                continue;
            }
            else if (i != 0)
            {
                // all players besides 1 need a prefab instantiated for them
                // for some reasoni have to set the position again after the instantiate
                Vector3 pos = paneFab.transform.position;
                GameObject pane = GameObject.Instantiate(paneFab, new Vector3(pos.x + 0.4f, pos.y, pos.z), Quaternion.identity);
                pane.name = $"PPane{i+1}";

                // position
                pane.transform.parent = gameObject.transform;
                pane.transform.localScale = paneFab.transform.localScale;
                pane.transform.position = new Vector3(pos.x + PANE_SPACING * (float)i, pos.y, pos.z);

                // color
                panes.Add(pane);
            }

            // set player specific bits
            panes[i].transform.Find("background").GetComponent<SpriteRenderer>().color = GameData.pcolors[i];
            panes[i].transform.Find("Portrait").gameObject.transform.Find("PNumber")
                .gameObject.GetComponent<TextMesh>().text = $"{i+1}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // read the players data and update headers accordingly
        for (int i = 0; i < GameData.currentPlayers.Count;i++)
        {
            UpdateTurnInd(i);
            UpdateStatuses(i);
            UpdateResources(i);
            UpdateArmour(i);
            UpdateHealth(i);
            UpdateDeckGrave(i);
        }
    }

    void UpdateDeckGrave(int playerNum)
    {
        GameObject me = GameObject.Find($"PPane{playerNum + 1}").gameObject;
        TextMesh deckCount = me.transform.Find("DeckCounter").gameObject.transform
            .Find("Dnum").gameObject.GetComponent<TextMesh>();
        TextMesh graveCount = me.transform.Find("GraveCounter").gameObject.transform
            .Find("Gnum").gameObject.GetComponent<TextMesh>(); ;

        if (deckCount.text != GameData.currentPlayers[playerNum].curDeck.Count.ToString())
        {
            deckCount.text = GameData.currentPlayers[playerNum].curDeck.Count.ToString();
        }
        if (graveCount.text != GameData.currentPlayers[playerNum].grave.Count.ToString())
        {
            graveCount.text = GameData.currentPlayers[playerNum].grave.Count.ToString();
        }
    }

    void UpdateTurnInd(int playerNum)
    {
        PlayerClassBase p = GameData.currentPlayers[playerNum];
        GameObject cvs = GameObject.Find("Canvas").gameObject;
        GameObject me = cvs.transform.Find($"Player{p.ctrlNum}").gameObject;
        GameObject turn = me.transform.Find("TurnInd").gameObject;
        turn.GetComponent<SpriteRenderer>().enabled = p.myTurn;
    }

    void UpdateStatuses(int playerNum)
    {
        PlayerClassBase p = GameData.currentPlayers[playerNum];
        if(p.statuses.Count != p.statusPrefabs.Count)
        {
            for (int i = 0; i < p.statusPrefabs.Count;i++)
            {
                Destroy(p.statusPrefabs[i]);
            }
            p.statusPrefabs = new List<GameObject>();

            for (int i = 0; i < p.statuses.Count; i++)
            {
                GameObject me = GameObject.Find($"PPane{playerNum+1}").gameObject;
                GameObject sts = me.transform.Find("statuses").gameObject;
                Vector3 statusPos = sts.transform.position;
                statusPos.x = statusPos.x + STATUS_SPACING * (float)i;

                GameObject newStat = Instantiate(p.statusPrefabTemplate, statusPos, Quaternion.identity);
                newStat.GetComponent<SpriteRenderer>().sprite = p.statuses[i].GetSprite();

                p.statusPrefabs.Add(newStat);
            }
        }
    }

    void UpdateResources(int playerNum)
    {
        PlayerClassBase p = GameData.currentPlayers[playerNum];
        GameObject me = GameObject.Find($"PPane{p.ctrlNum}").gameObject;
        GameObject rCur = me.transform.Find("rCur").gameObject;
        GameObject rMax = me.transform.Find("rMax").gameObject;

        // populate resources
        rMax.GetComponent<TextMesh>().text = p.maxResource.ToString();
        rCur.GetComponent<TextMesh>().text = p.currentResource.ToString();
    }

    void UpdateArmour(int playerNum)
    {
        PlayerClassBase p = GameData.currentPlayers[playerNum];
        GameObject me = GameObject.Find($"PPane{p.ctrlNum}").gameObject;
        GameObject physArm = me.transform.Find("PhysicalArmour").gameObject;
        GameObject armAmount = physArm.transform.Find("armAmount").gameObject;

        // TODO more armour types?
        if (p.armours.ContainsKey(DamageTypes.PHYSICAL))
        {
            physArm.GetComponent<SpriteRenderer>().enabled = true;
            armAmount.GetComponent<MeshRenderer>().enabled = true;

            armAmount.GetComponent<TextMesh>().text = p.armours[DamageTypes.PHYSICAL].ToString();
        }
        else
        {
            physArm.GetComponent<SpriteRenderer>().enabled = false;
            armAmount.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    void UpdateHealth(int playerNum)
    {
        PlayerClassBase p = GameData.currentPlayers[playerNum];
        GameObject me = GameObject.Find($"PPane{p.ctrlNum}").gameObject;
        GameObject hpobj = me.transform.Find("hpbar").gameObject;
        hpobj.GetComponent<Image>().fillAmount = (float)p.Hp/p.maxHp;

        if (p.Hp <= 0)
        {
            me.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
