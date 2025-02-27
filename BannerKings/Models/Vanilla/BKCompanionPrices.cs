﻿using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace BannerKings.Models.Vanilla
{
    public class BKCompanionPrices : DefaultCompanionHiringPriceCalculationModel
    {

        public override int GetCompanionHiringPrice(Hero companion)
        {
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			Settlement currentSettlement = companion.CurrentSettlement;
			Town town = (currentSettlement != null) ? currentSettlement.Town : null;
			if (town == null)
			{
				town = SettlementHelper.FindNearestTown(null, null).Town;
			}
			float num = 0f;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex++)
			{
				EquipmentElement itemRosterElement = companion.CharacterObject.Equipment[equipmentIndex];
				if (itemRosterElement.Item != null)
				{
					num += (float)town.GetItemPrice(itemRosterElement, null, false);
				}
			}
			for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.NumEquipmentSetSlots; equipmentIndex2++)
			{
				EquipmentElement itemRosterElement2 = companion.CharacterObject.FirstCivilianEquipment[equipmentIndex2];
				if (itemRosterElement2.Item != null)
				{
					num += (float)town.GetItemPrice(itemRosterElement2, null, false);
				}
			}
			explainedNumber.Add(num / 2f, null, null);
			explainedNumber.Add((float)(companion.CharacterObject.Level * 10), null, null);

			MBReadOnlyList<SkillObject> skills = MBObjectManager.Instance.GetObjectTypeList<SkillObject>();
			foreach (SkillObject skill in skills)
			{
				int skillValue = companion.GetSkillValue(skill);
				if (skillValue > 30) explainedNumber.Add(skillValue * GetCostFactor(skill), null, null);
			}


			if (Hero.MainHero.IsPartyLeader && Hero.MainHero.GetPerkValue(DefaultPerks.Steward.PaidInPromise))
				explainedNumber.AddFactor(DefaultPerks.Steward.PaidInPromise.PrimaryBonus * 0.01f, null);
			
			if (Hero.MainHero.PartyBelongedTo != null)
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Trade.GreatInvestor, Hero.MainHero.PartyBelongedTo, false, ref explainedNumber);
			
			return (int)explainedNumber.ResultNumber;
		}

		private int GetCostFactor(SkillObject skill)
		{
			if (skill == DefaultSkills.Bow || skill == DefaultSkills.OneHanded || skill == DefaultSkills.TwoHanded ||
				skill == DefaultSkills.Crossbow || skill == DefaultSkills.Throwing || skill == DefaultSkills.Polearm ||
				skill == DefaultSkills.Riding || skill == DefaultSkills.Athletics)
				return 3;
			else if (skill == DefaultSkills.Charm
				|| skill == DefaultSkills.Roguery)
				return 6;
			else if (skill == DefaultSkills.Scouting || skill == DefaultSkills.Crafting || skill == DefaultSkills.Medicine
				|| skill == DefaultSkills.Engineering || skill == DefaultSkills.Trade)
				return 10;
			else return 15;
		}
	}
}
