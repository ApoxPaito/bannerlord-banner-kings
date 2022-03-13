using BannerKings.Components;
using BannerKings.Managers;
using BannerKings.Managers.Court;
using BannerKings.Managers.Decisions;
using BannerKings.Managers.Institutions;
using BannerKings.Managers.Policies;
using BannerKings.Managers.Populations.Villages;
using BannerKings.Populations;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using static BannerKings.Managers.Policies.BKCriminalPolicy;
using static BannerKings.Managers.Policies.BKDraftPolicy;
using static BannerKings.Managers.Policies.BKGarrisonPolicy;
using static BannerKings.Managers.Policies.BKMilitiaPolicy;
using static BannerKings.Managers.Policies.BKTaxPolicy;
using static BannerKings.Managers.Policies.BKWorkforcePolicy;
using static BannerKings.Managers.PopulationManager;
using static BannerKings.Managers.TitleManager;

namespace BannerKings
{
    class SaveDefiner : SaveableTypeDefiner
    {

        public SaveDefiner() : base(82818189)
        {

        }

        protected override void DefineClassTypes()
        {
            base.AddEnumDefinition(typeof(PopType), 1);
            base.AddClassDefinition(typeof(PopulationClass), 2);
            base.AddClassDefinition(typeof(MilitaryData), 3);
            base.AddClassDefinition(typeof(CultureData), 4);
            base.AddClassDefinition(typeof(EconomicData), 5);
            base.AddClassDefinition(typeof(LandData), 6);
            base.AddClassDefinition(typeof(PopulationData), 7);
            base.AddClassDefinition(typeof(BannerKingsDecision), 8);
            base.AddClassDefinition(typeof(BannerKingsPolicy), 9);
            base.AddEnumDefinition(typeof(TaxType), 10);
            base.AddEnumDefinition(typeof(MilitiaPolicy), 11);
            base.AddEnumDefinition(typeof(WorkforcePolicy), 12);
            base.AddClassDefinition(typeof(PopulationManager), 13);
            base.AddClassDefinition(typeof(PolicyManager), 14);
            base.AddClassDefinition(typeof(PopulationPartyComponent), 15);
            base.AddClassDefinition(typeof(MilitiaComponent), 16);
            base.AddEnumDefinition(typeof(GarrisonPolicy), 17);
            base.AddEnumDefinition(typeof(CriminalPolicy), 18);
            base.AddEnumDefinition(typeof(CouncilPosition), 19);
            base.AddClassDefinition(typeof(CouncilMember), 20); 
            base.AddClassDefinition(typeof(CouncilData), 21);
            base.AddEnumDefinition(typeof(TitleType), 22);
            base.AddEnumDefinition(typeof(FeudalDuties), 23);
            base.AddEnumDefinition(typeof(FeudalRights), 24);
            base.AddEnumDefinition(typeof(GovernmentType), 25);
            base.AddEnumDefinition(typeof(SuccessionType), 26);
            base.AddEnumDefinition(typeof(InheritanceType), 27);
            base.AddEnumDefinition(typeof(GenderLaw), 28);
            base.AddClassDefinition(typeof(FeudalContract), 29);
            base.AddClassDefinition(typeof(FeudalTitle), 30);
            base.AddClassDefinition(typeof(Guild), 31);
            base.AddClassDefinition(typeof(VillageBuilding), 32);
            base.AddClassDefinition(typeof(CultureDataClass), 33);
            base.AddEnumDefinition(typeof(DraftPolicy), 34);
            base.AddClassDefinition(typeof(TitleManager), 37);
            base.AddClassDefinition(typeof(CourtManager), 38); 
            base.AddClassDefinition(typeof(BKDraftPolicy), 39);
            base.AddClassDefinition(typeof(BKGarrisonPolicy), 40);
            base.AddClassDefinition(typeof(BKMilitiaPolicy), 41);
            base.AddClassDefinition(typeof(BKTaxPolicy), 42);
            base.AddClassDefinition(typeof(BKWorkforcePolicy), 43);
            base.AddClassDefinition(typeof(BKEncourageMercantilism), 44);
            base.AddClassDefinition(typeof(BKEncourageMilitiaDecision), 45);
            base.AddClassDefinition(typeof(BKExemptTariffDecision), 46);
            base.AddClassDefinition(typeof(BKExportSlavesDecision), 47);
            base.AddClassDefinition(typeof(BKRationDecision), 48);
            base.AddClassDefinition(typeof(BKSubsidizeMilitiaDecision), 49);
            base.AddClassDefinition(typeof(BKTaxSlavesDecision), 50);
            base.AddClassDefinition(typeof(BKCriminalPolicy), 51);
            base.AddClassDefinition(typeof(VillageData), 52);
            base.AddClassDefinition(typeof(TournamentData), 53);
            base.AddClassDefinition(typeof(BannerKingsData), 54);
        }

        protected override void DefineContainerDefinitions()
        {
            base.ConstructContainerDefinition(typeof(List<CultureDataClass>));
            base.ConstructContainerDefinition(typeof(List<PopulationClass>));
            base.ConstructContainerDefinition(typeof(Dictionary<Settlement, PopulationData>));
            base.ConstructContainerDefinition(typeof(List<BannerKingsDecision>));
            base.ConstructContainerDefinition(typeof(List<BannerKingsPolicy>));
            base.ConstructContainerDefinition(typeof(List<CouncilMember>));
            base.ConstructContainerDefinition(typeof(List<FeudalTitle>));
            base.ConstructContainerDefinition(typeof(Dictionary<Settlement, List<BannerKingsPolicy>>));
            base.ConstructContainerDefinition(typeof(Dictionary<Settlement, List<BannerKingsDecision>>));
            base.ConstructContainerDefinition(typeof(Dictionary<Clan, CouncilData>));
            base.ConstructContainerDefinition(typeof(Dictionary<FeudalDuties, float>));
            base.ConstructContainerDefinition(typeof(List<FeudalRights>));
            base.ConstructContainerDefinition(typeof(Dictionary<FeudalTitle, Hero>));
            base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, FeudalTitle>));
            base.ConstructContainerDefinition(typeof(List<VillageBuilding>));
            
        }
    }
}
