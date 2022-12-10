using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Current;
        static UIManager current;
        public Canvas Canvas => GetComponent<Canvas>();
        World.TileManager tileManager;
        private void Awake()
        {
            if (current == null)
                current = this;

            tileManager = new World.TileManager();

            EventManager.onBattleStart += OnBattleStart;
            EventManager.onBattleEnd += OnBattleEnd;
            EventManager.OnStartHover += OnStartHover;
            EventManager.OnEndHover += OnEndHover;
        }
        private void Start()
        {
            GetPartyInfo(true).SetParty(RPG.RPGPlayer.Current.Party);
            GetPartyInfo(false, false);
            GetPawnSheet(false);
            GetStatComparison(false);

            EventManager.OnSelect += delegate (Events.SelectArgs args)
            {
                if (args.IsPawn)
                {
                    GetPawnSheet().SetPawn(args.Pawn);
                }
            };
        }
        private void Update()
        {
            //Vector2 origin = Vector2.zero;
            //Vector2 to = new Vector2(0, 4);
            //Vector2 inter = new Vector2(10, Mathf.Sin(Time.time) * 2);

            //Debug.DrawLine(origin, inter, Color.black);
            //Debug.DrawLine(inter, to, Color.black);

            //int c = 40;
            //float distance = Vector2.Distance(to, origin);
            //for (int i = 0; i < c; i++)
            //{
            //    Vector2 p1 = Bezier.Quadratic(origin, inter, to, distance / c * i);
            //    Vector2 p2 = Bezier.Quadratic(origin, inter, to, distance / c * (i + 1));
            //    Debug.DrawLine(p1, p2, Color.magenta);
            //}
        }
        public RPG.UI.PartyInfo GetPartyInfo(bool visibility = true, bool player = true)
        {
            string keyName = player ? "PlayerPartyInfo" : "EnemyPartyInfo";
            RPG.UI.PartyInfo partyInfo = transform.Find(keyName).GetComponent<PartyInfo>();
            if (partyInfo == null)
            {
                partyInfo = GameObject.Instantiate(Resources.Load<GameObject>("UI/PartyInfo"), gameObject.GetComponent<RectTransform>()).GetComponent<RPG.UI.PartyInfo>();
            }
            partyInfo.gameObject.SetActive(visibility);
            return partyInfo;
        }
        public RPG.UI.PawnSheet GetPawnSheet(bool visibility = true)
        {
            RPG.UI.PawnSheet pawnSheet = GameObject.FindObjectOfType<RPG.UI.PawnSheet>(true);
            if (pawnSheet == null)
            {
                pawnSheet = GameObject.Instantiate(Resources.Load<GameObject>("UI/PawnSheet"), gameObject.GetComponent<RectTransform>()).GetComponent<RPG.UI.PawnSheet>();
            }
            pawnSheet.gameObject.SetActive(visibility);
            return pawnSheet;
        }
        public RPG.UI.StatComparison GetStatComparison(bool visibility = true)
        {
            RPG.UI.StatComparison uiElement = GameObject.FindObjectOfType<RPG.UI.StatComparison>(true);
            if (uiElement == null)
            {
                uiElement = GameObject.Instantiate(Resources.Load<GameObject>("UI/StatComparison"), gameObject.GetComponent<RectTransform>()).GetComponent<RPG.UI.StatComparison>();
            }
            uiElement.gameObject.SetActive(visibility);
            return uiElement;
        }

        void OnBattleStart(Battle.Battle battle)
        {
            foreach (Battle.Attackers.Attacker attacker in battle.attackers)
            {
                GetPartyInfo(true, attacker.Party == RPGPlayer.Current.Party).SetParty(attacker.Party, PartyInfo.InfoType.Battle);
            }
        }
        void OnBattleEnd(Battle.Battle battle)
        {
            GetPartyInfo(false, false);
            GetPartyInfo(true, true).SetParty(RPGPlayer.Current.Party, PartyInfo.InfoType.Generic);
        }
        void OnStartHover(RPG.Events.SelectArgs args)
        {
            if (Director.Current.Battle != null)
            {
                Pawn targetPawn = Director.Current.Battle.GetPawnTarget(args.Pawn);
                if (targetPawn != null)
                {
                    StatComparison comparison = GetStatComparison(true);
                    if (args.Pawn.party.IsPlayer)
                        comparison.SetStats(args.Pawn.stats, targetPawn.stats);
                    else
                        comparison.SetStats(targetPawn.stats, args.Pawn.stats);
                }
            }
        }
        void OnEndHover(RPG.Events.SelectArgs args)
        {
            if (args.Pawn != null)
                GetStatComparison(false);
        }
    }
}
