﻿using BannerKings.Managers.Policies;
using System.Linq;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using static BannerKings.Managers.Policies.BKGarrisonPolicy;
using static BannerKings.Managers.Policies.BKMilitiaPolicy;
using static BannerKings.Managers.Policies.BKCriminalPolicy;
using BannerKings.Managers.Decisions;

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

namespace BannerKings.Managers
{
    public class PolicyManager
    {
        private Dictionary<Settlement, HashSet<BannerKingsDecision>> SettlementDecisions { get; set; }
        private Dictionary<Settlement, HashSet<BannerKingsPolicy>> SettlementPolicies { get; set; }

        private IEnumerable<string> TownDecisions
        {
            get
            {
                yield return "decision_ration";
                yield return "decision_militia_encourage";
                yield return "decision_slaves_export";
                yield return "decision_militia_subsidize";
                yield return "decision_tariff_exempt";
                yield return "decision_slaves_tax";
                yield return "decision_mercantilism";
                yield break;
            }
        }

        private IEnumerable<string> CastleDecisions
        {
            get
            {
                yield return "decision_ration";
                yield return "decision_militia_encourage";
                yield return "decision_slaves_export";
                yield return "decision_militia_subsidize";
                yield return "decision_tariff_exempt";
                yield return "decision_slaves_tax";
                yield return "decision_mercantilism";
                yield break;
            }
        }

        private IEnumerable<string> VillageDecisions
        {
            get
            {
                yield return "decision_militia_encourage";
                yield return "decision_militia_subsidize";
                yield break;
            }
        }

        public IEnumerable<string> Policies
        {
            get
            {
                yield return "criminal";
                yield return "garrison";
                yield return "draft";
                yield return "militia";
                yield return "tax";
                yield return "workforce";
                yield break;
            }
        }

        public PolicyManager(Dictionary<Settlement, HashSet<BannerKingsDecision>> DECISIONS, Dictionary<Settlement, HashSet<BannerKingsPolicy>> POLICIES)
        {
            this.SettlementDecisions = DECISIONS;
            this.SettlementPolicies = POLICIES;
        }

        public void InitializeSettlement(Settlement settlement)
        {
            if (!this.SettlementDecisions.ContainsKey(settlement))
                InitializeDecisions(settlement);
            if (!SettlementPolicies.ContainsKey(settlement))
                InitializePolicies(settlement);
        }

        public bool IsSettlementPoliciesSet(Settlement settlement) => SettlementDecisions.ContainsKey(settlement);
        public HashSet<BannerKingsDecision> GetDefaultDecisions(Settlement settlement)
        {
            if (!SettlementDecisions.ContainsKey(settlement))
                InitializeDecisions(settlement);

            return SettlementDecisions[settlement];
        }

        private void InitializeDecisions(Settlement settlement)
        {
            HashSet<BannerKingsDecision> decisions = new HashSet<BannerKingsDecision>();
            if (settlement.IsVillage)
            {
                foreach (string id in VillageDecisions)
                    decisions.Add(this.GenerateDecision(settlement, id));
            } else if (settlement.IsCastle)
            {
                foreach (string id in CastleDecisions)
                    decisions.Add(this.GenerateDecision(settlement, id));
            } else if (settlement.IsTown)
            {
                foreach (string id in TownDecisions)
                    decisions.Add(this.GenerateDecision(settlement, id));
            }
            this.SettlementDecisions.Add(settlement, decisions);
        }

        private void InitializePolicies(Settlement settlement)
        {
            HashSet<BannerKingsPolicy> policies = new HashSet<BannerKingsPolicy>();

            foreach (string id in Policies)
                policies.Add(this.GeneratePolicy(settlement, id));
     
            this.SettlementPolicies.Add(settlement, policies);
        }

        public int GetActiveCostlyDecisionsNumber(Settlement settlement)
        {
            if (this.SettlementDecisions.ContainsKey(settlement))
            {
                int i = 0;
                foreach (BannerKingsDecision decision in this.SettlementDecisions[settlement])
                    if (decision.Enabled)
                    {
                        string id = decision.GetIdentifier();
                        if (id != "decision_mercantilism" && id != "decision_slaves_tax" && id != "decision_tariff_exempt")
                            i += 1;
                    }
                        
                return i;
            }
            return 0;
        }
        public bool IsPolicyEnacted(Settlement settlement, string policyType, int value)
        {
            BannerKingsPolicy policy = this.GetPolicy(settlement, policyType);
            return policy.Selected == value;
        }

        public BannerKingsPolicy GetPolicy(Settlement settlement, string policyType)
        {
            BannerKingsPolicy result = null;
            if (SettlementPolicies.ContainsKey(settlement))
            {
                HashSet<BannerKingsPolicy> policies = SettlementPolicies[settlement];
                BannerKingsPolicy policy = policies.FirstOrDefault(x => x.GetIdentifier() == policyType);
                if (policy != null) result = policy;
            } else
            {
                result = GeneratePolicy(settlement, policyType);
                HashSet<BannerKingsPolicy> set = new HashSet<BannerKingsPolicy>();
                set.Add(result);
                SettlementPolicies.Add(settlement, set);
            }

            if (result == null) result = GeneratePolicy(settlement, policyType);

            return result;
        }

        public BannerKingsDecision GenerateDecision(Settlement settlement, string policyType)
        {
            if (policyType == "decision_militia_subsidize")
                return new BKSubsidizeMilitiaDecision(settlement, false);
            else if (policyType == "decision_militia_encourage")
                return new BKEncourageMilitiaDecision(settlement, false);
            else if (policyType == "decision_ration")
                return new BKRationDecision(settlement, false);
            else if (policyType == "decision_tariff_exempt")
                return new BKExemptTariffDecision(settlement, false);
            else if (policyType == "decision_foreigner_ban")
                return new BKBanForeignersDecision(settlement, false);
            else if (policyType == "decision_slaves_tax")
                return new BKTaxSlavesDecision(settlement, false);
            else if (policyType == "decision_mercantilism")
                return new BKEncourageMercantilism(settlement, false);
            else
                return new BKExportSlavesDecision(settlement, true);

        }

        public BannerKingsPolicy GeneratePolicy(Settlement settlement, string policyType)
        {
            if (policyType == "garrison")
                return new BKGarrisonPolicy(GarrisonPolicy.Standard, settlement);
            else if (policyType == "militia")
                return new BKMilitiaPolicy(MilitiaPolicy.Balanced, settlement);
            if (policyType == "tax")
                return new BKTaxPolicy(BKTaxPolicy.TaxType.Standard, settlement);
            else if (policyType == "tariff")
                return new BKTariffPolicy(BKTariffPolicy.TariffType.Standard, settlement);
            if (policyType == "workforce")
                return new BKWorkforcePolicy(BKWorkforcePolicy.WorkforcePolicy.None, settlement);
            else if (policyType == "draft")
                return new BKDraftPolicy(BKDraftPolicy.DraftPolicy.Standard, settlement);
            else return new BKCriminalPolicy(CriminalPolicy.Enslavement, settlement);
        }

        private void AddSettlementPolicy(Settlement settlement)
        {
            SettlementPolicies.Add(settlement, new HashSet<BannerKingsPolicy>());
        }

        private void AddSettlementDecision(Settlement settlement)
        {
            SettlementDecisions.Add(settlement, new HashSet<BannerKingsDecision>());
        }

        public void UpdateSettlementPolicy(Settlement settlement, BannerKingsPolicy policy)
        {
            if (SettlementPolicies.ContainsKey(settlement))
            {
                HashSet<BannerKingsPolicy> policies = SettlementPolicies[settlement];
                BannerKingsPolicy target = policies.FirstOrDefault(x => x.GetIdentifier() == policy.GetIdentifier());
                if (target != null) policies.Remove(target);
                policies.Add(policy);
            }
            else
            {
                AddSettlementPolicy(settlement);
            }
        }

        public bool IsDecisionEnacted(Settlement settlement, string type)
        {
            BannerKingsDecision decision = null;
            if (SettlementDecisions.ContainsKey(settlement))
                decision = SettlementDecisions[settlement].FirstOrDefault(x => x.GetIdentifier() == type);
            return decision != null ? decision.Enabled : false;
        }

        public void UpdateSettlementDecision(Settlement settlement, BannerKingsDecision decision)
        {
            if (SettlementDecisions.ContainsKey(settlement))
            {
                HashSet<BannerKingsDecision> policies = SettlementDecisions[settlement];
                BannerKingsDecision target = policies.FirstOrDefault(x => x.GetIdentifier() == decision.GetIdentifier());
                if (target != null) policies.Remove(target);
                policies.Add(decision);
            }
            else
            {
                AddSettlementPolicy(settlement);
            }
        }
    }
}
