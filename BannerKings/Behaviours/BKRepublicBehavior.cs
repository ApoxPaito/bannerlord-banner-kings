using BannerKings.Managers.Kingdoms;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
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
    public class BKRepublicBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(DailySettlementTick));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }


        private void DailySettlementTick(Settlement settlement)
        {

            if (settlement.OwnerClan == null || BannerKingsConfig.Instance.TitleManager == null) return;

            CampaignTime now = CampaignTime.Now;
            int day = now.GetDayOfYear;

            if (day == 1)
            {
                Kingdom kingdom = settlement.OwnerClan.Kingdom;
                if (kingdom != null)
                {
                    FeudalTitle title = BannerKingsConfig.Instance.TitleManager.GetSovereignTitle(kingdom);
                    if (title != null)
                    {
                        GovernmentType government = title.contract.government;
                        if (government == GovernmentType.Republic)
                        {
                            bool inElection = false;
                            foreach (KingdomDecision decision in kingdom.UnresolvedDecisions)
                                if (decision is RepublicElectionDecision)
                                {
                                    inElection = true;
                                    break;
                                }
                                    
                            if (!inElection)
                                kingdom.AddDecision(new RepublicElectionDecision(kingdom.RulingClan, kingdom.RulingClan), true);
                        }     
                    }
                }
            }      
        }
    }
}
