﻿using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using static BannerKings.Managers.TitleManager;

/*
    BSD 3-Clause License

    Copyright (c) 2021, Rodrigo Vaccari Melo
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice, this
       list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright notice,
       this list of conditions and the following disclaimer in the documentation
       and/or other materials provided with the distribution.

    3. Neither the name of the copyright holder nor the names of its
       contributors may be used to endorse or promote products derived from
       this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 */

namespace BannerKings.Behaviours
{
    public class BKClanBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(DailyClanTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }


        private void DailyClanTick(Clan clan)
        {
            if (clan.IsEliminated || clan.IsBanditFaction || clan.Kingdom == null || clan == Clan.PlayerClan ||
                BannerKingsConfig.Instance.TitleManager == null) return;

            BannerKingsConfig.Instance.CourtManager.UpdateCouncil(clan);

            if (clan.WarPartyComponents.Count < clan.CommanderLimit && clan.Companions.Count < clan.CompanionLimit && 
                clan.Settlements.Count(x => x.IsVillage ) > 1 && clan.Influence >= 150)
            {
                Settlement village = clan.Settlements.FirstOrDefault(x => x.IsVillage);
                if (village == null) return;
                List<FeudalTitle> clanTitles = BannerKingsConfig.Instance.TitleManager.GetAllDeJure(clan);
                FeudalTitle title = BannerKingsConfig.Instance.TitleManager.GetTitle(village);
                if (clanTitles.Count == 0 || title == null || !clanTitles.Contains(title) || title.deJure != clan.Leader) return;

                GenderLaw genderLaw = title.contract.genderLaw;
                CharacterObject template = genderLaw == GenderLaw.Agnatic ? clan.Culture.NotableAndWandererTemplates.FirstOrDefault(x => x.Occupation == Occupation.Wanderer && !x.IsFemale) : 
                    clan.Culture.NotableAndWandererTemplates.FirstOrDefault(x => x.Occupation == Occupation.Wanderer);
                if (template == null) return;

                Settlement settlement = clan.Settlements.FirstOrDefault();
                if (settlement == null) settlement = Town.AllTowns.FirstOrDefault(x => x.Culture == clan.Culture).Settlement;

                IEnumerable<MBEquipmentRoster> source = from e in MBObjectManager.Instance.GetObjectTypeList<MBEquipmentRoster>()
                                                        where e.EquipmentCulture == clan.Culture
                                                        select e;
                if (source == null) return;
                MBEquipmentRoster roster = (from e in source where e.IsMediumNobleEquipmentTemplate
                                                select e into x orderby MBRandom.RandomInt()
                                                select x).FirstOrDefault<MBEquipmentRoster>();
                if (roster == null) return;

                float price = this.GetPrice(village.Village.MarketTown.Settlement, roster);
                if (clan.Leader.Gold >= price * 2f)
                {
                    Hero hero = HeroCreator.CreateSpecialHero(template, settlement, clan, null,
                    Campaign.Current.Models.AgeModel.HeroComesOfAge + 5 + MBRandom.RandomInt(27));
                    EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, roster.AllEquipments.GetRandomElement());
                    GainKingdomInfluenceAction.ApplyForDefault(clan.Leader, -150f);
                    BannerKingsConfig.Instance.TitleManager.GrantLordship(title, title.deJure, hero);
                    bool mainParty = hero.PartyBelongedTo == MobileParty.MainParty;
                    MobilePartyHelper.CreateNewClanMobileParty(hero, clan, out mainParty);
                }    
            }
        }

        private float GetPrice(Settlement settlement, MBEquipmentRoster roster)
        {
            float price = 0;
            if (settlement != null)
            {
                Equipment equip = roster.AllEquipments.GetRandomElement<Equipment>();
                for (int i = 0; i < 12; i++)
                {
                    EquipmentElement element = new EquipmentElement(equip[i].Item, equip[i].ItemModifier, null, false);
                    if (!element.IsEmpty && element.Item != null)
                        price += settlement.Town.MarketData.GetPrice(element.Item);
                }
            }
            return price * 0.1f;
        }
    }
}
