using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
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
    class BKTitleBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            throw new NotImplementedException();
        }

        public override void SyncData(IDataStore dataStore)
        {
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(OnClanDestroyed));
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(OnOwnerChanged));
        }

        private void OnClanDestroyed(Clan clan) 
        {
            
            if (BannerKingsConfig.Instance.TitleManager == null) return;
            List<FeudalTitle> titles = BannerKingsConfig.Instance.TitleManager.GetAllDeJure(clan);
            if (titles.Count > 0)
            {
                foreach (FeudalTitle title in titles)
                {
                    if (BannerKingsConfig.Instance.TitleManager.HasSuzerain(title))
                    {
                        FeudalTitle suzerain = BannerKingsConfig.Instance.TitleManager.GetImmediateSuzerain(title);
                        if (suzerain.deJure.IsAlive && !clan.Heroes.Contains(suzerain.deJure))
                        {
                            BannerKingsConfig.Instance.TitleManager.InheritAllTitles(title.deJure, suzerain.deJure);
                            continue;
                        }
                            
                    } 
                    
                    if (title.sovereign != null)
                        if (title.sovereign.deJure != title.deJure && title.sovereign.deJure.IsAlive)
                        {
                            BannerKingsConfig.Instance.TitleManager.InheritAllTitles(title.deJure, title.sovereign.deJure); 
                            continue;
                        }    
                    
                    if (title.deJure != title.DeFacto)
                    {
                        BannerKingsConfig.Instance.TitleManager.DeactivateDeJure(title);
                        continue;
                    }

                    BannerKingsConfig.Instance.TitleManager.DeactivateTitle(title);
                }  
            }
        }

        private void OnOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, 
            ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            if (BannerKingsConfig.Instance.TitleManager == null) return;

            FeudalTitle title = BannerKingsConfig.Instance.TitleManager.GetTitle(settlement);
            if (title != null)
            {
                if (detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByKingDecision)
                    BannerKingsConfig.Instance.TitleManager.InheritTitle(oldOwner, newOwner, title);
                else
                    BannerKingsConfig.Instance.TitleManager.ApplyOwnerChange(settlement, newOwner);
            }
            
        }
    }
}
