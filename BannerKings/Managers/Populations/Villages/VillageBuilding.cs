using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BannerKings.Managers.Populations.Villages
{
    public class VillageBuilding : Building
    {

        [SaveableField(1)]
        private Village village;

        public VillageBuilding(BuildingType buildingType, Town town, Village village) : base(buildingType, town)
        {
            this.village = village;
        }

        public Village Village => this.village;
    }
}
