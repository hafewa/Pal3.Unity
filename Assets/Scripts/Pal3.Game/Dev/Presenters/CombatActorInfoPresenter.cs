// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Game.Dev.Presenters
{
    using Core.DataReader.Gdb;
    using Engine.Abstraction;

    /// <summary>
    /// CombatActorInfo holder component to present ScnObjectInfo in the Unity inspector.
    /// </summary>
    public sealed class CombatActorInfoPresenter : GameEntityScript
    {
        [UnityEngine.SerializeField] public CombatActorInfo combatActorInfo;
    }
}