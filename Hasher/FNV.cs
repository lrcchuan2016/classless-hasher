#region License
/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Classless.Hasher - C#/.NET Hash and Checksum Algorithm Library.
 *
 * The Initial Developer of the Original Code is Classless.net.
 * Portions created by the Initial Developer are Copyright (C) 2004 the Initial
 * Developer. All Rights Reserved.
 *
 * Contributor(s):
 *		Jason Simeone (jay@classless.net)
 * 
 * ***** END LICENSE BLOCK ***** */
#endregion

using System;
using Classless.Hasher.Utilities;

namespace Classless.Hasher {
	/// <summary>Computes the FNV hash for the input data using the managed library.</summary>
	public class Fnv : HashAlgorithm, IParametrizedHashAlgorithm {
		private readonly object syncLock = new object();

		private FnvParameters parameters;
		private ulong hash;


		/// <summary>Gets the HashAlgorithmParameters being used by this HashAlgorithm.</summary>
		public HashAlgorithmParameters Parameters {
			get { return parameters; }
		}


		/// <summary>Initializes a new instance of the FNV class.</summary>
		/// <remarks>This constructor implements the default parameters of Fnv32BitType1A.</remarks>
		public Fnv() : this(FnvParameters.GetParameters(FnvStandard.Fnv32BitType1A)) { }

		/// <summary>Initializes a new instance of the FNV class.</summary>
		/// <param name="parameters">The parameters to utilize in the FNV calculation.</param>
		/// <exception cref="ArgumentNullException">When the specified parameters are null.</exception>
		public Fnv(FnvParameters parameters) : base() {
			lock (syncLock) {
				if (parameters == null) { throw new ArgumentNullException("parameters", Hasher.Properties.Resources.paramCantBeNull); }
				this.parameters = parameters;
				HashSizeValue = this.parameters.Order;
				hash = (ulong)this.parameters.OffsetBasis;
			}
		}


		/// <summary>Initializes the algorithm.</summary>
		override public void Initialize() {
			lock (syncLock) {
				base.Initialize();
				hash = (ulong)parameters.OffsetBasis;
			}
		}


		/// <summary>Drives the hashing function.</summary>
		/// <param name="array">The array containing the data.</param>
		/// <param name="ibStart">The position in the array to begin reading from.</param>
		/// <param name="cbSize">How many bytes in the array to read.</param>
		override protected void HashCore(byte[] array, int ibStart, int cbSize) {
			lock (syncLock) {
				for (int i = ibStart; i < (ibStart + cbSize); i++) {
					if (parameters.AlgorithmType == FnvAlgorithmType.Fnv1) {
						hash *= (ulong)parameters.Prime;
						hash ^= array[i];
					} else if (parameters.AlgorithmType == FnvAlgorithmType.Fnv1A) {
						hash ^= array[i];
						hash *= (ulong)parameters.Prime;
					}
				}
			}
		}


		/// <summary>Performs any final activities required by the hash algorithm.</summary>
		/// <returns>The final hash value.</returns>
		override protected byte[] HashFinal() {
			lock (syncLock) {
				byte[] temp = Conversions.ULongToByte(hash, EndianType.BigEndian);
				byte[] final = new byte[parameters.Order / 8];

				for (int i = (final.Length - 1); i >= 0; i--) {
					final[i] = temp[temp.Length - ((parameters.Order / 8) - i)];
				}

				return final;
			}
		}
	}
}
