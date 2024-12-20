/* ================================================================
   ----------------------------------------------------------------
   Project   :   AI Tree
   Publisher :   Renowned Games
   Developer :   Zinnur Davleev
   ----------------------------------------------------------------
   Copyright 2022-2023 Renowned Games All rights reserved.
   ================================================================ */

using RenownedGames.Apex;
using UnityEngine;

namespace RenownedGames.AITree.Nodes
{
    [NodeContent("Player Prefs Get Float", "Tasks/Player Prefs/Get Float", IconPath = "Images/Icons/Node/PlayerPrefsIcon.png")]
    public class PPGetFloatTask : TaskNode
    {
        [Title("Blackboard")]
        [SerializeField]
        private StringKey key;

        [SerializeField]
        [NonLocal]
        private FloatKey storeKey;

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (key == null || storeKey == null)
            {
                return State.Failure;
            }

            storeKey.SetValue(PlayerPrefs.GetFloat(key.GetValue()));
            return State.Success;
        }
    }
}