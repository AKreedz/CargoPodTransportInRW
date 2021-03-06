﻿using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Helicopter
{
    public static class HelicoptersArrivalActionUtility
    {

        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions<T>(Func<FloatMenuAcceptanceReport> acceptanceReportGetter, Func<T> arrivalActionGetter, string label, CompLaunchableHelicopter representative, int destinationTile,Caravan car) where T : TransportPodsArrivalAction
        {
            FloatMenuAcceptanceReport rep = acceptanceReportGetter();
            if (rep.Accepted || !rep.FailReason.NullOrEmpty() || !rep.FailMessage.NullOrEmpty())
            {
                if (!rep.FailReason.NullOrEmpty())
                {
                    yield return new FloatMenuOption(label + " (" + rep.FailReason + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
                else
                {
                    yield return new FloatMenuOption(label, delegate
                    {
                        FloatMenuAcceptanceReport floatMenuAcceptanceReport = acceptanceReportGetter();
                        if (floatMenuAcceptanceReport.Accepted)
                        {
                            representative.TryLaunch(destinationTile, arrivalActionGetter(),car);
                        }
                        else if (!floatMenuAcceptanceReport.FailMessage.NullOrEmpty())
                        {
                            Messages.Message(floatMenuAcceptanceReport.FailMessage, new GlobalTargetInfo(destinationTile), MessageTypeDefOf.RejectInput, false);
                        }
                    }, MenuOptionPriority.Default, null, null, 0f, null, null);
                }
            }
            yield break;
        }



        public static IEnumerable<FloatMenuOption> GetATKFloatMenuOptions(CompLaunchableHelicopter representative, IEnumerable<IThingHolder> pods, SettlementBase settlement,Caravan car)
        {
            foreach (FloatMenuOption f in HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_AttackSettlement>(() => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement), () => new TransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.EdgeDrop), "AttackAndDropAtEdge".Translate(new object[]
            {
        settlement.Label
            }), representative, settlement.Tile,car))
            {
                yield return f;
            }
            foreach (FloatMenuOption f2 in HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_AttackSettlement>(() => TransportPodsArrivalAction_AttackSettlement.CanAttack(pods, settlement), () => new TransportPodsArrivalAction_AttackSettlement(settlement, PawnsArrivalModeDefOf.CenterDrop), "AttackAndDropInCenter".Translate(new object[]
            {
        settlement.Label
            }), representative, settlement.Tile,car))
            {
                yield return f2;
            }
            yield break;
        }

        public static IEnumerable<FloatMenuOption> GetGIFTFloatMenuOptions(CompLaunchableHelicopter representative, IEnumerable<IThingHolder> pods, SettlementBase settlement,Caravan car)
        {
            if (settlement.Faction == Faction.OfPlayer)
            {
                return Enumerable.Empty<FloatMenuOption>();
            }
            return HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_GiveGift>(() => TransportPodsArrivalAction_GiveGift.CanGiveGiftTo(pods, settlement), () => new TransportPodsArrivalAction_GiveGift(settlement), "GiveGiftViaTransportPods".Translate(new object[]
            {
                settlement.Faction.Name,
                FactionGiftUtility.GetGoodwillChange(pods, settlement).ToStringWithSign()
            }), representative, settlement.Tile,car);
        }

        public static IEnumerable<FloatMenuOption> GetVisitFloatMenuOptions(CompLaunchableHelicopter representative, IEnumerable<IThingHolder> pods, SettlementBase settlement,Caravan car)
        {
            return HelicoptersArrivalActionUtility.GetFloatMenuOptions<TransportPodsArrivalAction_VisitSettlement>(() => TransportPodsArrivalAction_VisitSettlement.CanVisit(pods, settlement), () => new TransportPodsArrivalAction_VisitSettlement(settlement), "VisitSettlement".Translate(new object[]
            {
                settlement.Label
            }), representative, settlement.Tile,car);
        }
    }
}
