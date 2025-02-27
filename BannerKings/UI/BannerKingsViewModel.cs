﻿using BannerKings.Managers.Policies;
using BannerKings.Populations;
using BannerKings.UI.Items;
using System;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

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
            int i = 0;
            foreach (Enum enumValue in policy.GetPolicies())
            {
                BKItemVM item = new BKItemVM(enumValue, true, policy.GetHint(i));
                selector.AddItem(item);
                i++;
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
