﻿#region License
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
 * Portions created by the Initial Developer are Copyright (C) 2009 the Initial
 * Developer. All Rights Reserved.
 *
 * Contributor(s):
 *		Jason Simeone (jay@classless.net)
 * 
 * ***** END LICENSE BLOCK ***** */
#endregion

using System;
using NUnit.Framework;
using Classless.Hasher.Methods;
using Classless.Hasher.Utilities;

namespace Classless.Hasher.Tests {
	[TestFixture]
	public class BlockHashAlgorithmTests {
		static public object[] UnevenInputTests = {
			new object[] {
				TestVectors.h("7707D6AE4E027C70EEA2A935C2296F21"),
				new byte[][] {
					TestVectors.s(new string('a', 1000000)),
				}
			},
			new object[] {
				TestVectors.h("7707D6AE4E027C70EEA2A935C2296F21"),
				new byte[][] {
					TestVectors.s(new string('a', 1)),
					TestVectors.s(new string('a', 999999)),
				}
			},
			new object[] {
				TestVectors.h("7707D6AE4E027C70EEA2A935C2296F21"),
				new byte[][] {
					TestVectors.s(new string('a', 63)),
					TestVectors.s(new string('a', 999937)),
				}
			},
			new object[] {
				TestVectors.h("7707D6AE4E027C70EEA2A935C2296F21"),
				new byte[][] {
					TestVectors.s(new string('a', 63)),
					TestVectors.s(new string('a', 63)),
					TestVectors.s(new string('a', 999874)),
				}
			},
			new object[] {
				TestVectors.h("7707D6AE4E027C70EEA2A935C2296F21"),
				new byte[][] {
					TestVectors.s(new string('a', 20)),
					TestVectors.s(new string('a', 20)),
					TestVectors.s(new string('a', 86)),
					TestVectors.s(new string('a', 999874)),
				}
			},
		};

		[Test, TestCaseSource("UnevenInputTests")]
		public void UnevenInputTest(byte[] expectedHash, byte[][] inputChunks) {
			MD5 hasher = new MD5();

			for (int i = 0; i < inputChunks.Length - 1; i++) {
				hasher.TransformBlock(inputChunks[i], 0, inputChunks[i].Length, null, 0);
			}
			hasher.TransformFinalBlock(inputChunks[inputChunks.Length - 1], 0, inputChunks[inputChunks.Length - 1].Length);

			CustomAssert.AreEqual(expectedHash, hasher.Hash);
		}


		[Test, ExpectedException(typeof(ArgumentException)), TestCase(0), TestCase(-1), TestCase(-100)]
		public void BadBlockSizeTest(int blockSize) {
			HashList list = new HashList(new MD5(), blockSize);
		}
	}
}
