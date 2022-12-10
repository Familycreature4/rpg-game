using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG;
using RPG.Events;
/// <summary>
/// Message Bus for game events
/// </summary>
public static class EventManager
{
    public static Action<World> onWorldInitialized;

    public static Action<Input.RPGInput> OnInput;

    #region Battle

    public static Action<RPG.Battle.Battle> onBattleStart;
    public static Action<RPG.Battle.Battle> onBattleEnd;

    public static Action<RPG.Battle.Battle, RPG.Battle.Attackers.Attacker> onAttackerTurnStart;
    public static Action<RPG.Battle.Battle, RPG.Battle.Attackers.Attacker> onAttackerTurnEnd;

    #endregion

    #region Player

    public static Action<RPGPlayer, Party, Party> onPlayerPartySet;

    #endregion

    public static Action<SelectArgs> OnStartHover;
    public static Action<SelectArgs> OnEndHover;
    public static Action<SelectArgs> OnSelect;

    public static Action<Party> onPartyMove;
    public static Action<Pawn, int> onPawnDamaged;
}
