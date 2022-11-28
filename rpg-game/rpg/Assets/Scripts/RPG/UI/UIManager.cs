using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum UI
    {
        PawnSheet,
        PartyInfo,
    }
    public static UIManager Current;
    static UIManager current;
    public Canvas Canvas => GetComponent<Canvas>();
    private void Awake()
    {
        if (current == null)
            current = this;
    }
    private void Start()
    {
        GetUI(UI.PartyInfo).GetComponent<RPG.UI.PartyInfo>().SetParty((RPG.RPGPlayer.Current as RPG.RPGPlayer).Party);

        (Player.Current as RPG.RPGPlayer).selector.onGameObjectSelected += delegate (GameObject ob)
        {
            if (ob.TryGetComponent<RPG.Pawn>(out RPG.Pawn pawn))
                GetUI(UI.PawnSheet).GetComponent < RPG.UI.PawnSheet>().SetPawn(pawn);
        };
        
    }

    public GameObject GetUI(UI type)
    {
        switch (type)
        {
            case UI.PartyInfo:
                RPG.UI.PartyInfo partyInfo = GameObject.FindObjectOfType<RPG.UI.PartyInfo>();
                if (partyInfo == null)
                {
                    partyInfo = GameObject.Instantiate(Resources.Load<GameObject>("UI/PartyInfo"), Canvas.gameObject.GetComponent<RectTransform>()).GetComponent<RPG.UI.PartyInfo>();
                }

                return partyInfo.gameObject;
            case UI.PawnSheet:
                RPG.UI.PawnSheet pawnSheet = GameObject.FindObjectOfType<RPG.UI.PawnSheet>();
                if (pawnSheet == null)
                {
                    pawnSheet = GameObject.Instantiate(Resources.Load<GameObject>("UI/PawnSheet"), Canvas.gameObject.GetComponent<RectTransform>()).GetComponent<RPG.UI.PawnSheet>();
                }
                return pawnSheet.gameObject;
        }

        return null;
    }
}
