﻿using BannerKings.Managers.Titles;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static BannerKings.Managers.TitleManager;

namespace BannerKings.UI.Information
{
    class OwnershipNotification : InformationData
    {
        internal static void CollectObjectsOwnershipNotification(object o, List<object> collectedObjects)
        {
            ((OwnershipNotification)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
        }


        private FeudalTitle title;
        public override TextObject TitleText => new TextObject("Title Acquired");

        public override string SoundEventPath => "";

        public OwnershipNotification(FeudalTitle title, TextObject descriptionText) : base(descriptionText)
        {
            this.title = title;
        }
    }
}
