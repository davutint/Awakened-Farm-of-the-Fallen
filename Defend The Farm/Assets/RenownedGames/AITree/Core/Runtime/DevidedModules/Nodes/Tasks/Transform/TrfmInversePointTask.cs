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
    [NodeContent("Inverse Transform Point", "Tasks/Transform/TInverseTransformPoint", IconPath = "Images/Icons/Node/TransformIcon.png")]
    public class TrfmInversePointTask : TaskNode
    {
        [Title("Blackboard")]
        [SerializeField]
        [NonLocal]
        private TransformKey transform;

        [SerializeField]
        private Vector3Key worldPosition;

        [SerializeField]
        [NonLocal]
        private Vector3Key storeResult;

        /// <summary>
        /// Called every tick during node execution.
        /// </summary>
        /// <returns>State.</returns>
        protected override State OnUpdate()
        {
            if (transform == null || worldPosition == null || storeResult == null)
            {
                return State.Failure;
            }

            Transform _transform = transform.GetValue();
            if (_transform == null)
            {
                return State.Failure;
            }

            storeResult.SetValue(_transform.InverseTransformPoint(worldPosition.GetValue()));
            return State.Success;
        }
    }
}