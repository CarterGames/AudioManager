/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CarterGames.Shared.AudioManager
{
    /// <summary>
    /// A helper class for assembly related logic.
    /// </summary>
    public static class AssemblyHelper
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static Assembly[] audioManagerAssemblies;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets all the cart assemblies to use when checking in internally only.
        /// </summary>
        private static IEnumerable<Assembly> AudioManagerAssemblies
        {
            get
            {
                if (audioManagerAssemblies != null) return audioManagerAssemblies;
                audioManagerAssemblies = GetAssemblies();
                return audioManagerAssemblies;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The assemblies for the library.
        /// </summary>
        /// <returns>Returns all the assemblies.</returns>
        private static Assembly[] GetAssemblies()
        {
#if UNITY_EDITOR
            return ProjectAssemblyDefinition.ProjectEditorAssemblies;
#else
            return ProjectAssemblyDefinition.ProjectRuntimeAssemblies;
#endif
        }


        /// <summary>
        /// Gets the number of classes of the requested type in the project.
        /// </summary>
        /// <param name="internalCheckOnly">Check internally to the asset only.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>The total in the project.</returns>
        public static int CountClassesOfType<T>(bool internalCheckOnly = true)
        {
            var assemblies = internalCheckOnly ? AudioManagerAssemblies : AppDomain.CurrentDomain.GetAssemblies();
                
            return assemblies.SelectMany(x => x.GetTypes())
                .Count(x => x.IsClass && typeof(T).IsAssignableFrom(x));
        }
        
        
        /// <summary>
        /// Gets the number of classes of the requested type in the project.
        /// </summary>
        /// <param name="assemblies">The assemblies to check through.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>The total in the project.</returns>
        public static int CountClassesOfType<T>(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => x.GetTypes())
                .Count(x => x.IsClass && typeof(T).IsAssignableFrom(x));
        }
        
        
        /// <summary>
        /// Gets all the classes of the entered type in the project.
        /// </summary>
        /// <param name="internalCheckOnly">Check internally to the asset only.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>All the implementations of the entered class.</returns>
        public static IEnumerable<T> GetClassesOfType<T>(bool internalCheckOnly = true)
        {
            var assemblies = internalCheckOnly ? AudioManagerAssemblies : AppDomain.CurrentDomain.GetAssemblies();

            return assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && typeof(T).IsAssignableFrom(x) && !x.IsAbstract && x.FullName != typeof(T).FullName)
                .Select(type => (T)Activator.CreateInstance(type));
        }
        
        
        /// <summary>
        /// Gets all the classes of the entered type in the project.
        /// </summary>
        /// <param name="internalCheckOnly">Check internally to the asset only.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>All the implementations of the entered class.</returns>
        public static IEnumerable<Type> GetClassesNamesOfType<T>(bool internalCheckOnly = true)
        {
            var assemblies = internalCheckOnly ? AudioManagerAssemblies : AppDomain.CurrentDomain.GetAssemblies();

            return assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && typeof(T).IsAssignableFrom(x) && x.FullName != typeof(T).FullName);
        }
        
        
        /// <summary>
        /// Gets all the classes of the entered type in the project.
        /// </summary>
        /// <param name="assemblies">The assemblies to check through.</param>
        /// <typeparam name="T">The type to find.</typeparam>
        /// <returns>All the implementations of the entered class.</returns>
        public static IEnumerable<T> GetClassesOfType<T>(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && typeof(T).IsAssignableFrom(x) && x.FullName != typeof(T).FullName)
                .Select(type => (T)Activator.CreateInstance(type));
        }
    }
}