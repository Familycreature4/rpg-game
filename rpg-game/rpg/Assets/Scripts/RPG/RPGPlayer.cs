using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Input;
using UnityEngine;
namespace RPG
{
    public class RPGPlayer : Player
    {
        static public new RPGPlayer Current => Player.Current as RPGPlayer;
        public Party Party => party;
        Party party;
        public Selector selector;
        private void Awake()
        {
            if (current == null)
                current = this;

            input = new RPGInput();
            base.cameraController = new CameraControllers.RPGController(this);

            selector = new Selector(this);
            EventManager.OnInput += OnInput;
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

            EventManager.onPlayerPartySet?.Invoke(this, oldParty, party);
        }
        public void OnInput(Input.RPGInput input)
        {
            Vector3Int localMove = Vector3Int.zero;
            if (input.forward.Value)
            {
                localMove.z++;
            }

            if (input.backward.Value)
            {
                localMove.z--;
            }

            if (input.right.Value)
            {
                localMove.x++;
            }

            if (input.left.Value)
            {
                localMove.x--;
            }

            if (input.turnLeft.Pressed)
            {
                party.FormationRotation -= 90.0f;
            }

            if (input.turnRight.Pressed)
            {
                party.FormationRotation += 90.0f;
            }

            Vector3Int disp = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * localMove);
            party.Move(disp);
        }
    }
}