using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An alliance of pawns
/// </summary>
public class Party
{
    static Dictionary<string, Party> parties = new Dictionary<string, Party>();
    public static bool AddToParty(string name, Pawn pawn)
    {
        if (parties.TryGetValue(name, out Party party) == false)
        {
            // Create one
            party = new Party();
            parties.Add(name, party);
        }

        if (party.pawns.Contains(pawn) == false)
        {
            party.pawns.Add(pawn);
            return true;
        }

        return false;
    }
    public static Party GetParty(string name)
    {
        if (parties.TryGetValue(name, out Party p))
            return p;

        return null;
    }
    public List<Pawn> pawns = new List<Pawn>();
}
