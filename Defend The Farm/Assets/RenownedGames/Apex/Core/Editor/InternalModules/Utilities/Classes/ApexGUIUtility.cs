﻿/* ================================================================
   ---------------------------------------------------
   Project   :    Apex
   Publisher :    Renowned Games
   Developer :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2020-2023 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;

namespace RenownedGames.ApexEditor
{
    /// <summary>
    /// Miscellaneous helper stuff for visual entities.
    /// </summary>
    public static class ApexGUIUtility
    {
        /// <summary>
        /// Smoothly animate updating height of visual entities.
        /// </summary>
        public static bool Animate { get; set; } = true;

        /// <summary>
        /// Vertical spacing between visual entities.
        /// </summary>
        //public static float VerticalSpacing { get; internal set; } = 2;

        /// <summary>
        /// Horizontal spacing between visual entities.
        /// </summary>
        public static float HorizontalSpacing { get; internal set; } = 2;

        /// <summary>
        /// Top and bottom bounds of group style containers.
        /// </summary>
        public static float BoxBounds { get; internal set; } = 5;

        /// <summary>
        /// Check if we are at the beginning of the none hierarchy mode.
        /// </summary>
        internal static bool FixIndentLevel 
        {
            get
            {
                return EditorGUI.indentLevel == 0 && !EditorGUIUtility.hierarchyMode;
            }
        }
    }
}
