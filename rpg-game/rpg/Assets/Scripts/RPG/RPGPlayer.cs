using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Input;
using UnityEngine;
namespace RPG
{
    public class RPGPlayer : Player, Input.IInputReceiver
    {
        public Party Party => party;
        Party party;
        public Selector selector;
        public System.Action<Party, Party> onPartyChange;
        private void Awake()
        {
            if (current == null)
                current = this;

            input = new RPGInput();
            base.cameraController = new CameraControllers.RPGController(this);

            input.Subscribe(this);

            selector = new Selector(this);
        }
        void Update()
        {
            input?.Update();
        }
        private void LateUpdate()
        {
            cameraController?.LateUpdate();
        }
        public void SetParty(Party party)
        {
            Party oldParty = this.party;
            this.party = party;

            onPartyChange?.Invoke(oldParty, this.party);
        }

        public void OnInputReceived(Input.Input input)
        {
            RPGInput rpgInput = input as RPGInput;

            Vector3Int localMove = Vector3Int.zero;
            if (rpgInput.forward.Value)
            {
                localMove.z++;
            }

            if (rpgInput.backward.Value)
            {
                localMove.z--;
            }

            if (rpgInput.right.Value)
            {
                localMove.x++;
            }

            if (rpgInput.left.Value)
            {
                localMove.x--;
            }

            if (rpgInput.turnLeft.Pressed)
            {
                party.FormationRotation -= 90.0f;
            }

            if (rpgInput.turnRight.Pressed)
            {
                party.FormationRotation += 90.0f;
            }

            Vector3Int disp = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * localMove);
            party.Move(disp);
        }

        public int GetInputPriority()
        {
            return 0;
        }
    }
}