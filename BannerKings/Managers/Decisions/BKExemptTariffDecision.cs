﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace BannerKings.Managers.Decisions
{
    public class BKExemptTariffDecision : BannerKingsDecision
    {
        public BKExemptTariffDecision(Settlement settlement, bool enabled) : base(settlement, enabled)
        {

        }

        public override string GetHint() => new TextObject("{=!}Exempt merchants from tariffs, reducing prices and attracting caravans").ToString();

        public override string GetIdentifier() => "decision_tariff_exempt";

        public override string GetName() => new TextObject("{=!}Tariffs exemption").ToString();
    }
}
