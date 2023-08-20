// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Core.DataReader.Txt
{
    using System.Collections.Generic;
    using Gdb;
    using Nav;

    public sealed class CombatScnFile
    {
        public Dictionary<string, WuLingType> CombatSceneWuLingInfo  { get; }
        public Dictionary<string, Dictionary<NavFloorKind, string>> CombatSceneMapInfo  { get; }

        public CombatScnFile(Dictionary<string, WuLingType> combatSceneWuLingInfo,
            Dictionary<string, Dictionary<NavFloorKind, string>> combatSceneMapInfo)
        {
            CombatSceneWuLingInfo = combatSceneWuLingInfo;
            CombatSceneMapInfo = combatSceneMapInfo;
        }
    }
}