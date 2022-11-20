using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Editor.Entities
{
    public class PartySpawn : Entity
    {
        public bool isPlayer = false;

        public override void OnSerialize(JObject json)
        {
            base.OnSerialize(json);

            json["isPlayer"] = isPlayer;
        }

        public override void OnDeserialize(JObject json)
        {
            base.OnDeserialize(json);

            isPlayer = json["isPlayer"].ToObject<bool>();
        }
    }
}
