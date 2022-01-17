﻿using TaleWorlds.Core;
using TaleWorlds.Localization;
using static Populations.Managers.TitleManager;

namespace Populations.UI.Information
{
    class OwnershipNotification : InformationData
    {

        private FeudalTitle title;
        public override TextObject TitleText => new TextObject("Title Acquired");

        public override string SoundEventPath => "";

        public OwnershipNotification(FeudalTitle title, TextObject descriptionText) : base(descriptionText)
        {
            this.title = title;
        }
    }
}