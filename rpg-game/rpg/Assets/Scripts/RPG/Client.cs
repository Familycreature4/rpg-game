using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG;
/// <summary>
/// Couples the user to the game
/// </summary>
public class Client : MonoBehaviour, Input.IInputReceiver
{
    public static Client Current => instance;
    static Client instance;
    public Party party;
    public Input.Input input;
    BoundsInt lastCameraBounds;
    Vector3Int savedMove;
    float savedRotation;
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

        input = new Input.RPGInput();
        input.Subscribe(this);
    }
    private void Update()
    {
        input.Update();

        if (party == null)
        {
            party = Party.GetParty("PLAYER");
        }
    }

    public void OnInputReceived(Input.Input input)
    {
        Input.RPGInput rpgInput = input as Input.RPGInput;
        #region PAWNS/PARTY
        
        if (party == null)
            return;

        bool didTurn = false;

        if (rpgInput.turnLeft.Pressed)
        {
            savedRotation -= 90.0f;
        }
        if (rpgInput.turnRight.Pressed)
        {
            savedRotation += 90.0f;
        }

        savedRotation = Mathf.Clamp(savedRotation % 360.0f, -90f, 90f);

        if (party.CanMove)
        {
            if (rpgInput.forward.Value)
            {
                savedMove.z++;
                rpgInput.forward.Consume();
            }
            if (rpgInput.backward.Value)
            {
                savedMove.z--;
                rpgInput.backward.Consume();
            }
            if (rpgInput.left.Value)
            {
                savedMove.x--;
                rpgInput.left.Consume();
            }
            if (rpgInput.right.Value)
            {
                savedMove.x++;
                rpgInput.right.Consume();
            }
        }
        

        for (int c = 0; c < 3; c++)
        {
            savedMove[c] = Mathf.Clamp(savedMove[c], -1, 1);
        }

        if (savedRotation != 0 && party.CanMove)
        {
            party.FormationRotation += savedRotation;
            didTurn = true;
        }

        Vector3Int formationMove = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * savedMove);
        bool moved = party.Move(formationMove);

        if (didTurn || moved)
        {
            party.Leader.InvokeMoveDelay();

            savedMove = Vector3Int.zero;
            savedRotation = 0.0f;
        }
        #endregion
    }
    public int GetInputPriority() => int.MaxValue;
}
