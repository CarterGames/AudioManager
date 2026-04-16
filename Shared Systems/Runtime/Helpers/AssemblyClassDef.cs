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
using System.Linq;
using UnityEngine;

namespace CarterGames.Shared.AudioManager
{
	/// <summary>
	/// A class for storing info about a class so it can be referenced from its assembly & type names.
	/// </summary>
	[Serializable]
	public sealed class AssemblyClassDef
	{
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		[SerializeField] private string assembly;
		[SerializeField] private string type;

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Properties
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Gets if the class is valid or not.
		/// </summary>
		public bool IsValid => !string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(type);

		
		/// <summary>
		/// The assembly string stored.
		/// </summary>
		public string Assembly => assembly;
		
		
		/// <summary>
		/// The type string stored.
		/// </summary>
		public string Type => type;

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Creates a new definition when called.
		/// </summary>
		/// <param name="assembly">The assembly to reference.</param>
		/// <param name="type">The type to reference.</param>
		public AssemblyClassDef(string assembly, string type)
		{
			this.assembly = assembly;
			this.type = type;
		}

		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Converts System.Type to a AssemblyClassDef instance.
		/// </summary>
		/// <param name="type">The type to convert.</param>
		/// <returns>The created AssemblyClassDef from the type.</returns>
		public static implicit operator AssemblyClassDef(Type type)
		{
			return new AssemblyClassDef(type.Assembly.FullName, type.FullName);
		}
		
		/* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
		|   Fields
		───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
		
		/// <summary>
		/// Gets the type stored in this AssemblyClassDef.
		/// </summary>
		/// <typeparam name="T">The type to make.</typeparam>
		/// <returns>The made type or the types default on failure.</returns>
		public T GetDefinedType<T>()
		{
			if (!IsValid)
			{
				Debug.LogError("[GetDefinedType]: Data not valid to generate the defined type");
				return default;
			}
			
			try
			{
				return AssemblyHelper.GetClassesOfType<T>(false).FirstOrDefault(t =>
					t.GetType().Assembly.FullName == Assembly && t.GetType().FullName == Type);
			}
#pragma warning disable
			catch (Exception e)
			{
				Debug.LogError(
					"[GetDefinedType]: Failed to generate type from stored data. If you have refactored the class selected, please reselect it to update the record.");

				return default;
			}
#pragma warning restore
		}
	}
}