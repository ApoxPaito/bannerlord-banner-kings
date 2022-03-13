using BannerKings.Managers;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using static BannerKings.Managers.TitleManager;
using BannerKings.Populations;
using BannerKings.Models;
using BannerKings.Managers.Policies;
using BannerKings.Managers.Decisions;
using BannerKings.Models.Populations;
using TaleWorlds.Library;
using BannerKings.Managers.Populations.Villages;
using BannerKings.Managers.Court;

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

namespace BannerKings
{
    public class BannerKingsConfig
    {

        public PopulationManager PopulationManager;
        public PolicyManager PolicyManager;
        public TitleManager TitleManager;
        public CourtManager CourtManager;
        public HashSet<IBannerKingsModel> Models = new HashSet<IBannerKingsModel>();
        public MBReadOnlyList<BuildingType> VillageBuildings { get; set; }

        public void InitManagers()
        {
            DefaultVillageBuildings.Instance.Init();
            this.PopulationManager = new PopulationManager(new Dictionary<Settlement, PopulationData>(), new List<MobileParty>());
            this.PolicyManager = new PolicyManager(new Dictionary<Settlement, List<BannerKingsDecision>>(), new Dictionary<Settlement,
            List<BannerKingsPolicy>>());
            this.TitleManager = new TitleManager(new Dictionary<FeudalTitle, Hero>(), new Dictionary<Hero, List<FeudalTitle>>(), new Dictionary<Kingdom, FeudalTitle>());
            this.CourtManager = new CourtManager(new Dictionary<Clan, CouncilData>());
            this.InitModels();
        }

        public void InitManagers(PopulationManager populationManager, PolicyManager policyManager, TitleManager titleManager, CourtManager court)
        {
            this.PopulationManager = populationManager;
            this.PolicyManager = policyManager;
            this.TitleManager = titleManager != null ? titleManager : new TitleManager(new Dictionary<FeudalTitle, Hero>(), new Dictionary<Hero, List<FeudalTitle>>(),
                new Dictionary<Kingdom, FeudalTitle>());
            this.CourtManager = court;
            this.InitModels();
        }

        private void InitModels()
        {
            this.Models.Add(new BKCultureAssimilationModel());
            this.Models.Add(new BKCultureAcceptanceModel());
            this.Models.Add(new BKAdministrativeModel());
            this.Models.Add(new BKLegitimacyModel());
            this.Models.Add(new BKUsurpationModel());
            this.Models.Add(new BKStabilityModel());
            this.Models.Add(new BKGrowthModel());
            this.Models.Add(new BKEconomyModel());
            this.Models.Add(new BKCaravanAttractionModel());
        }

        public static BannerKingsConfig Instance
        {
            get => ConfigHolder.CONFIG;
        }

        internal struct ConfigHolder
        {
             public static BannerKingsConfig CONFIG = new BannerKingsConfig();
        }
    }
}
