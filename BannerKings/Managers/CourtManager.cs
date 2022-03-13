using BannerKings.Managers.Court;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

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
    public class CourtManager
    {
        private Dictionary<Clan, CouncilData> COUNCILS { get; set; }

        public CourtManager(Dictionary<Clan, CouncilData> councils)
        {
            this.COUNCILS = councils;
        }

        public void ApplyCouncilEffect(ref ExplainedNumber result, Hero settlementOwner, CouncilPosition position, float maxEffect, bool factor)
        {
            CouncilData council = this.GetCouncil(settlementOwner);
            float competence = council.GetCompetence(position);
            if (competence != 0f)
            {
                if (!factor) result.Add(maxEffect * competence, new TextObject("{=!}Council Effect"));
                else result.AddFactor(maxEffect * competence, new TextObject("{=!}Council Effect"));
            }
        }

        public CouncilData GetCouncil(Hero hero)
        {
            Clan clan = hero.Clan;
            if (this.COUNCILS.ContainsKey(clan))
                return this.COUNCILS[clan];
            else
            {
                CouncilData council = new CouncilData(clan);
                this.COUNCILS.Add(clan, council);
                return council;
            }
        }

        public CouncilData GetCouncil(Clan clan)
        {
            if (this.COUNCILS.ContainsKey(clan))
                return this.COUNCILS[clan];
            else
            {
                CouncilData council = new CouncilData(clan);
                this.COUNCILS.Add(clan, council);
                return council;
            }
        }

        public void UpdateCouncil(Clan clan)
        {
            CouncilData data = this.GetCouncil(clan.Leader);
            data.Update(null);
        }
    }
}
