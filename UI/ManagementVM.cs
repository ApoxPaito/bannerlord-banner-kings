﻿using Populations.UI.Items;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static Populations.PolicyManager;


namespace Populations.UI
{
    public class ManagementVM : ViewModel
    {

        private PopulationOptionVM _slaveToogle;
        private PopulationOptionVM _popAccelerateToogle;
        private PopulationOptionVM _selfInvestToogle;
        private PopulationOptionVM _conscriptionToogle;
        private PopulationOptionVM _nobleExemptionToogle;
        private PopulationOptionVM _subsidizeMilitiaToogle;
        private SelectorVM<MilitiaItemVM> _militiaSelector;
        private SelectorVM<TaxItemVM> _taxSelector;
        private SelectorVM<WorkItemVM> _workSelector;
        private Settlement _settlement;
        private bool _isSelected;

        public ManagementVM(Settlement _settlement, bool _isSelected)
        {
            this._settlement = _settlement;
            this._isSelected = _isSelected;
            this.RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            List<PolicyElement> elements = PopulationConfig.Instance.PolicyManager.GetDefaultDecisions(_settlement);
            foreach (PolicyElement policy in elements)
            {
                PopulationOptionVM vm = new PopulationOptionVM()
                .SetAsBooleanOption(policy.description, policy.isChecked, delegate (bool value)
                {
                    PopulationConfig.Instance.PolicyManager.UpdatePolicy(_settlement, policy.type, value);
                    this.RefreshValues();

                }, new TextObject(policy.hint));
                switch (policy.type)
                {
                    case PolicyType.EXPORT_SLAVES:
                        SlaveToogle = vm;
                        break;
                    case PolicyType.POP_GROWTH:
                        AccelerateToogle = vm;
                        break;
                    case PolicyType.SELF_INVEST:
                        InvestToogle = vm;
                        break;
                    case PolicyType.CONSCRIPTION:
                        ConscriptionToogle = vm;
                        break;
                    case PolicyType.EXEMPTION:
                        ExemptionToogle = vm;
                        break;
                    case PolicyType.SUBSIDIZE_MILITIA:
                        SubsidizeToogle = vm;
                        break;
                }
            }

            int militiaIndex = 0;
            MilitiaPolicy militiaPolicy = PopulationConfig.Instance.PolicyManager.GetMilitiaPolicy(_settlement);
            if (militiaPolicy == MilitiaPolicy.Melee)
                militiaIndex = 1;
            else if (militiaPolicy == MilitiaPolicy.Ranged)
                militiaIndex = 2;
            MilitiaSelector = new SelectorVM<MilitiaItemVM>(0, new Action<SelectorVM<MilitiaItemVM>>(this.OnMilitiaChange));
            MilitiaSelector.SetOnChangeAction(null);
            foreach (MilitiaPolicy policy in _militiaPolicies)
            {
                MilitiaItemVM item = new MilitiaItemVM(policy, true);
                MilitiaSelector.AddItem(item);
            }
            MilitiaSelector.SetOnChangeAction(OnMilitiaChange);
            MilitiaSelector.SelectedIndex = militiaIndex;


            int taxIndex = 0;
            TaxType taxPolicy = PopulationConfig.Instance.PolicyManager.GetSettlementTax(_settlement);
            if (taxPolicy == TaxType.High)
                taxIndex = 1;
            else if (taxPolicy == TaxType.Low)
                taxIndex = 2;
            TaxSelector = new SelectorVM<TaxItemVM>(0, new Action<SelectorVM<TaxItemVM>>(this.OnTaxChange));
            TaxSelector.SetOnChangeAction(null);
            foreach (TaxType policy in _taxPolicies)
            {
                TaxItemVM item = new TaxItemVM(policy, true, PopulationConfig.Instance.PolicyManager.GetTaxHint(policy, _settlement.IsVillage));
                TaxSelector.AddItem(item);
            }
            TaxSelector.SetOnChangeAction(OnTaxChange);
            TaxSelector.SelectedIndex = taxIndex;


            int workIndex = 0;
            WorkforcePolicy workPolicy = PopulationConfig.Instance.PolicyManager.GetSettlementWork(_settlement);
            if (workPolicy == WorkforcePolicy.Land_Expansion)
                workIndex = 1;
            else if (workPolicy == WorkforcePolicy.Martial_Law)
                workIndex = 2;
            else if (workPolicy == WorkforcePolicy.Construction)
                workIndex = 3;
            WorkSelector = new SelectorVM<WorkItemVM>(0, new Action<SelectorVM<WorkItemVM>>(this.OnWorkChange));
            WorkSelector.SetOnChangeAction(null);
            foreach (WorkforcePolicy policy in _workPolicies)
            {
                WorkItemVM item = new WorkItemVM(policy, true);
                WorkSelector.AddItem(item);
            }
            WorkSelector.SetOnChangeAction(OnWorkChange);
            WorkSelector.SelectedIndex = workIndex;
        }

        private void OnMilitiaChange(SelectorVM<MilitiaItemVM> obj)
        {
            if (obj.SelectedItem != null)
            {
                MilitiaItemVM selectedItem = obj.SelectedItem;
                PopulationConfig.Instance.PolicyManager.UpdateMilitiaPolicy(_settlement, selectedItem.policy);
            }
        }

        private void OnTaxChange(SelectorVM<TaxItemVM> obj)
        {
            if (obj.SelectedItem != null)
            {
                TaxItemVM selectedItem = obj.SelectedItem;
                PopulationConfig.Instance.PolicyManager.UpdateTaxPolicy(_settlement, selectedItem.policy);
            }
        }

        private void OnWorkChange(SelectorVM<WorkItemVM> obj)
        {
            if (obj.SelectedItem != null)
            {
                WorkItemVM selectedItem = obj.SelectedItem;
                PopulationConfig.Instance.PolicyManager.UpdateWorkPolicy(_settlement, selectedItem.policy);
            }
        }


        private IEnumerable<WorkforcePolicy> _workPolicies
        {
            get
            {
                yield return WorkforcePolicy.None;
                yield return WorkforcePolicy.Martial_Law;
                yield return WorkforcePolicy.Construction;
                yield break;
            }
        }

        private IEnumerable<TaxType> _taxPolicies
        {
            get
            {
                yield return TaxType.Standard;
                yield return TaxType.High;
                yield return TaxType.Low;
                yield break;
            }
        }

        private IEnumerable<MilitiaPolicy> _militiaPolicies
        {
            get
            {
                yield return MilitiaPolicy.Balanced;
                yield return MilitiaPolicy.Melee;
                yield return MilitiaPolicy.Ranged;
                yield break;
            }
        }

        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    if (value) this.RefreshValues();
                    base.OnPropertyChangedWithValue(value, "IsSelected");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<WorkItemVM> WorkSelector
        {
            get
            {
                return this._workSelector;
            }
            set
            {
                if (value != this._workSelector)
                {
                    this._workSelector = value;
                    base.OnPropertyChangedWithValue(value, "WorkSelector");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<TaxItemVM> TaxSelector
        {
            get
            {
                return this._taxSelector;
            }
            set
            {
                if (value != this._taxSelector)
                {
                    this._taxSelector = value;
                    base.OnPropertyChangedWithValue(value, "TaxSelector");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<MilitiaItemVM> MilitiaSelector
        {
            get
            {
                return this._militiaSelector;
            }
            set
            {
                if (value != this._militiaSelector)
                {
                    this._militiaSelector = value;
                    base.OnPropertyChangedWithValue(value, "MilitiaSelector");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM SlaveToogle
        {
            get => _slaveToogle;
            set
            {
                if (value != _slaveToogle)
                {
                    _slaveToogle = value;
                    base.OnPropertyChangedWithValue(value, "SlaveToogle");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM AccelerateToogle
        {
            get => _popAccelerateToogle;
            set
            {
                if (value != _popAccelerateToogle)
                {
                    _popAccelerateToogle = value;
                    base.OnPropertyChangedWithValue(value, "AccelerateToogle");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM InvestToogle
        {
            get => _selfInvestToogle;
            set
            {
                if (value != _selfInvestToogle)
                {
                    _selfInvestToogle = value;
                    base.OnPropertyChangedWithValue(value, "InvestToogle");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM ConscriptionToogle
        {
            get => _conscriptionToogle;
            set
            {
                if (value != _conscriptionToogle)
                {
                    _conscriptionToogle = value;
                    base.OnPropertyChangedWithValue(value, "ConscriptionToogle");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM ExemptionToogle
        {
            get => _nobleExemptionToogle;
            set
            {
                if (value != _nobleExemptionToogle)
                {
                    _nobleExemptionToogle = value;
                    base.OnPropertyChangedWithValue(value, "ExemptionToogle");
                }
            }
        }

        [DataSourceProperty]
        public PopulationOptionVM SubsidizeToogle
        {
            get => _subsidizeMilitiaToogle;
            set
            {
                if (value != _subsidizeMilitiaToogle)
                {
                    _subsidizeMilitiaToogle = value;
                    base.OnPropertyChangedWithValue(value, "SubsidizeToogle");
                }
            }
        }
    }
}
