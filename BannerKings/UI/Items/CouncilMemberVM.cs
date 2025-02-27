﻿using BannerKings.Managers.Court;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace BannerKings.UI.Items
{
    class CouncilMemberVM : SettlementGovernorSelectionItemVM
    {
		private BasicTooltipViewModel courtHint;
		public CouncilMemberVM(Hero member, Action<SettlementGovernorSelectionItemVM> onSelection, CouncilPosition position, float competence) : base(member, onSelection)
        { 
            this.GovernorHint = new BasicTooltipViewModel(() => UIHelper.GetHeroGovernorEffectsTooltip(member, position, competence));
			this.CourtHint = new BasicTooltipViewModel(() => UIHelper.GetHeroCourtTooltip(member));
		}

		[DataSourceProperty]
		public BasicTooltipViewModel CourtHint
		{
			get => this.courtHint;	
			set
			{
				if (value != this.courtHint)
				{
					this.courtHint = value;
					base.OnPropertyChangedWithValue(value, "CourtHint");
				}
			}
		}
	}
}
