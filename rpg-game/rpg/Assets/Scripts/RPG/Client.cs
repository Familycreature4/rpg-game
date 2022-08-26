using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Couples the user to the game
/// </summary>
public class Client : MonoBehaviour
{
    public static Client Current => instance;
    static Client instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("DUPLICATE CLIENT");
        }
        else
        {
            instance = this;
        }
    }

    private void LateUpdate()
    {
        Text text = GameObject.FindObjectOfType<UnityEngine.UI.Text>();
        Party party = Party.GetParty("0");
        if (party != null)
        {
            text.text = "";
            text.canvas.enabled = true;
            foreach (Pawn pawn in party.pawns)
            {
                text.text += $"{pawn.name}\n";
            }
        }
        else
        {
            text.canvas.enabled = false;
        }
    }
}
