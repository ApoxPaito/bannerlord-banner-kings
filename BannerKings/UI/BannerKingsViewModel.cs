using BannerKings.Managers.Policies;
using BannerKings.Populations;
using BannerKings.UI.Items;
using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

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

namespace BannerKings.UI
{
    public class BannerKingsViewModel : ViewModel
    {

        protected PopulationData data;
        protected bool selected;

        public BannerKingsViewModel(PopulationData data, bool selected)
        {
            this.data = data;
            this.selected = selected;
        }

        protected string FormatValue(float value) => (value * 100f).ToString("0.00") + '%';
        protected string FormatDays(float value) => (value).ToString("0");
        protected SelectorVM<BKItemVM> GetSelector(BannerKingsPolicy policy, Action<SelectorVM<BKItemVM>> action)
        {
            SelectorVM<BKItemVM> selector = new SelectorVM<BKItemVM>(0, new Action<SelectorVM<BKItemVM>>(action));
            selector.SetOnChangeAction(null);
            foreach (Enum enumValue in policy.GetPolicies())
            {
                BKItemVM item = new BKItemVM(enumValue, true, policy.GetHint());
                selector.AddItem(item);
            }

            
            return selector;
        }

        [DataSourceProperty]
        public bool HasTown => !this.IsVillage;

        [DataSourceProperty]
        public bool IsVillage => this.data.Settlement.IsVillage;
        

        [DataSourceProperty]
        public bool IsSelected
        {
            get => this.selected;
            set
            {
                if (value != this.selected)
                {
                    this.selected = value;
                    if (value) this.RefreshValues();
                    base.OnPropertyChangedWithValue(value, "IsSelected");
                }
            }
        }

        public void ExecuteClose() => UIManager.Instance.CloseUI();
    }
}
