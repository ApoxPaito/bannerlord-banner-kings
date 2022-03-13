﻿using TaleWorlds.CampaignSystem;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static BannerKings.Managers.TitleManager;
using TaleWorlds.CampaignSystem.Actions;
using HarmonyLib;
using TaleWorlds.Localization;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using SandBox;
using System.Linq;

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

namespace BannerKings.Behaviors
{
    class BKCompanionBehavior : CampaignBehaviorBase
    {

        private FeudalTitle titleGiven = null;
        List<InquiryElement> lordshipsToGive = new List<InquiryElement>();
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
           
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            AddDialog(campaignGameStarter);
        }

        private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
        {
        
        }

        private void AddDialog(CampaignGameStarter starter)
        {
            StringBuilder knighthoodSb = new StringBuilder();
            knighthoodSb.Append("By knighting, you are granting this person nobility and they will be bound to you as your vassal by the standard contract of the kingdom. A lordship must be given away to seal the contract.");
            knighthoodSb.Append(Environment.NewLine);
            knighthoodSb.Append(" ");
            knighthoodSb.Append(Environment.NewLine);
            knighthoodSb.Append("Their lands and titles henceforth can not be revoked without lawful cause, and any fief revenue will be theirs, taxed or not by you as per contract");
            knighthoodSb.Append(Environment.NewLine);
            knighthoodSb.Append(" ");
            knighthoodSb.Append(Environment.NewLine);
            knighthoodSb.Append("As a knight, they are capable of raising a personal retinue and are obliged to fulfill their duties.");

            starter.AddPlayerLine("companion_grant_knighthood", "companion_role", "companion_knighthood_question", "Would you like to serve me as my knight?", 
                new ConversationSentence.OnConditionDelegate(this.companion_grant_knighthood_on_condition), delegate {
                    InformationManager.ShowInquiry(new InquiryData("Bestowing Knighthood", knighthoodSb.ToString(), true, false, "Understood", null, null, null), false);
                }, 100, null, null);

            starter.AddDialogLine("companion_grant_knighthood_response", "companion_knighthood_question", "companion_knighthood_response",
                "My lord, I would be honored.", null, null, 100, null); 

            starter.AddPlayerLine("companion_grant_knighthood_response_confirm", "companion_knighthood_response", "companion_knighthood_accepted", "Let us decide your fief.",
                new ConversationSentence.OnConditionDelegate(this.companion_knighthood_accepted_on_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_knighthood_accepted_on_consequence), 100, null, null);

            starter.AddPlayerLine("companion_grant_knighthood_response_cancel", "companion_knighthood_response", "companion_role_pretalk", "Actualy, I would like to discuss this at a later time.",
               null, null, 100, null, null);

            starter.AddPlayerLine("companion_grant_knighthood_granted", "companion_knighthood_accepted", "close_window", "It is decided then. I bestow upon you the title of Knight.",
                null, null, 100, null, null);
        }

        private bool companion_grant_knighthood_on_condition()
        {
            if (BannerKingsConfig.Instance.TitleManager == null) return false;
            Hero companion = Hero.OneToOneConversationHero;
            FeudalTitle title = BannerKingsConfig.Instance.TitleManager.GetHighestTitle(Hero.MainHero);
            if (companion != null && companion.Clan == Clan.PlayerClan && Hero.MainHero.Clan.Tier >= 2 &&
                Hero.MainHero.Clan.Kingdom != null && title != null && title.type != TitleType.Lordship)
                return !BannerKingsConfig.Instance.TitleManager.IsHeroKnighted(companion);
            else return false;
        }

        private bool companion_knighthood_accepted_on_condition()
        {
            lordshipsToGive.Clear();
            List<FeudalTitle> titles = BannerKingsConfig.Instance.TitleManager.GetAllDeJure(Hero.MainHero);
            foreach (FeudalTitle title in titles)
            {
                if (title.type != TitleType.Lordship || title.fief == null || title.deJure != Hero.MainHero) continue;
                lordshipsToGive.Add(new InquiryElement(title, title.name.ToString(), new ImageIdentifier()));
            }

            if (lordshipsToGive.Count == 0)
                InformationManager.DisplayMessage(new InformationMessage("You currently do not lawfully own a lordship that could be given away."));

            return lordshipsToGive.Count >= 1;
        }

        private void companion_knighthood_accepted_on_consequence()
        {
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                    "Select the fief you would like to give away", string.Empty, lordshipsToGive, true, 1, 
                    GameTexts.FindText("str_done", null).ToString(), "", new Action<List<InquiryElement>>(this.OnNewPartySelectionOver), 
                    new Action<List<InquiryElement>>(this.OnNewPartySelectionOver), ""), false);
        }

        private void OnNewPartySelectionOver(List<InquiryElement> element)
        {
            if (element.Count == 0)
                return;
            
            this.titleGiven = (FeudalTitle)element[0].Identifier;
            BannerKingsConfig.Instance.TitleManager.GrantLordship(this.titleGiven, Hero.MainHero, Hero.OneToOneConversationHero);
        }
    }

    namespace Patches
    {

        [HarmonyPatch(typeof(Hero), "SetHeroEncyclopediaTextAndLinks")]
        class HeroDescriptionPatch
        {
            static void Postfix(ref string __result, Hero o)
            {
                List<FeudalTitle> titles = BannerKingsConfig.Instance.TitleManager.GetAllDeJure(o);
                if (titles != null && titles.Count > 0)
                {
                    string desc = "";
                    FeudalTitle current = null;
                    List<FeudalTitle> finalList = titles.OrderBy(x => (int)x.type).ToList();
                    foreach (FeudalTitle title in finalList)
                    {
                        if (current == null)
                            desc += string.Format("{0} of {1}", Helpers.Helpers.GetTitleHonorary(title.type, false), title.shortName);
                        else if (current.type == title.type)
                            desc += ", " + title.shortName;
                        else if (current.type != title.type)
                            desc += string.Format(" and {0} of {1}", Helpers.Helpers.GetTitleHonorary(title.type, false), title.shortName);
                        current = title;
                    }
                    __result = __result + Environment.NewLine + desc;
                }
            }
        }

        [HarmonyPatch(typeof(LordConversationsCampaignBehavior), "conversation_lord_give_oath_go_on_condition")]
        class ShowContractPatch
        {
            static void Postfix()
            {
                if (BannerKingsConfig.Instance.TitleManager != null)
                {
                    PartyBase party = PlayerEncounter.EncounteredParty;
                    BannerKingsConfig.Instance.TitleManager.ShowContract(party.LeaderHero, "I accept");
                }
            }
        }

        [HarmonyPatch(typeof(ClanPartiesVM), "ExecuteCreateNewParty")]
        class ClanCreatePartyPatch
        {
            static bool Prefix(ClanPartiesVM __instance, Clan ____faction, Func<Hero, Settlement> ____getSettlementOfGovernor)
            {
                if (__instance.CanCreateNewParty)
                {
                    List<InquiryElement> list = new List<InquiryElement>();
                    foreach (Hero hero in (from h in ____faction.Heroes
                                           where !h.IsDisabled
                                           select h).Union(____faction.Companions))
                    {
                        if ((hero.IsActive || hero.IsReleased || hero.IsFugitive) && !hero.IsChild && hero != Hero.MainHero && hero.CanLeadParty())
                        {
                            bool isEnabled = false;
                            MethodInfo hintMethod = __instance.GetType().GetMethod("GetPartyLeaderAssignmentSkillsHint", BindingFlags.NonPublic | BindingFlags.Instance);
                            string hint = (string)hintMethod.Invoke(__instance, new object[] { hero });
                            if (hero.PartyBelongedToAsPrisoner != null)
                                hint = new TextObject("{=vOojEcIf}You cannot assign a prisoner member as a new party leader", null).ToString();
                            else if (hero.IsReleased)
                                hint = new TextObject("{=OhNYkblK}This hero has just escaped from captors and will be available after some time.", null).ToString();
                            else if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero == hero)
                                hint = new TextObject("{=aFYwbosi}This hero is already leading a party.", null).ToString();
                            else if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero != Hero.MainHero)
                                hint = new TextObject("{=FjJi1DJb}This hero is already a part of an another party.", null).ToString();
                            else if (____getSettlementOfGovernor(hero) != null)
                                hint = new TextObject("{=Hz8XO8wk}Governors cannot lead a mobile party and be a governor at the same time.", null).ToString();
                            else if (hero.HeroState == Hero.CharacterStates.Disabled)
                                hint = new TextObject("{=slzfQzl3}This hero is lost", null).ToString();
                            else if (hero.HeroState == Hero.CharacterStates.Fugitive)
                                hint = new TextObject("{=dD3kRDHi}This hero is a fugitive and running from their captors. They will be available after some time.", null).ToString();
                            else if (!BannerKingsConfig.Instance.TitleManager.IsHeroKnighted(hero))
                                hint = new TextObject("A hero must be knighted and granted land before being able to raise a personal retinue. You may bestow knighthood by talking to them.", null).ToString();
                            else isEnabled = true;

                            list.Add(new InquiryElement(hero, hero.Name.ToString(), new ImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false)), isEnabled, hint));
                        }
                    }
                    if (list.Count > 0)
                    {
                        MethodInfo method = __instance.GetType().GetMethod("OnNewPartySelectionOver", BindingFlags.NonPublic | BindingFlags.Instance);
                        InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=0Q4Xo2BQ}Select the Leader of the New Party", null).ToString(),
                            string.Empty, list, true, 1, GameTexts.FindText("str_done", null).ToString(), "", new Action<List<InquiryElement>>(delegate (List<InquiryElement> x) { method.Invoke(__instance, new object[] { x }); }),
                            new Action<List<InquiryElement>>(delegate (List<InquiryElement> x) { method.Invoke(__instance, new object[] { x }); }), ""), false);
                    }
                    else InformationManager.AddQuickInformation(new TextObject("{=qZvNIVGV}There is no one available in your clan who can lead a party right now.", null), 0, null, "");
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Settlement))]
        [HarmonyPatch("Owner", MethodType.Getter)]
        class VillageOwnerPatch
        {
            static void Postfix(Settlement __instance, ref Hero __result)
            {
                if (__instance.IsVillage && BannerKingsConfig.Instance.TitleManager != null)
                {
                    FeudalTitle title = BannerKingsConfig.Instance.TitleManager.GetTitle(__instance);
                    if (title != null)
                        __result = title.deFacto;
                }
            }
        }
    }
}
