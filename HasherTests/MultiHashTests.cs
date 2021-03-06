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
using System.Collections;
using NUnit.Framework;
using Classless.Hasher.Methods;

namespace Classless.Hasher.Tests {
	[TestFixture]
	public class MultiHashTests {
		[Test]
		public void DefaultConstructorTest() {
			MultiHash hasher = new MultiHash();
			CustomAssert.AreEqual(new byte[0], hasher.ComputeHash(new byte[0]));
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNullTest() {
			HashAlgorithmCollection hashCollection = null;
			MultiHash hasher = new MultiHash(hashCollection);
		}


		[Test]
		public void ConstructorCollectionTest() {
			HashAlgorithmCollection hashCollection = new HashAlgorithmCollection();
			MD5 md5 = new MD5();
			hashCollection.Add(md5);
			Sha1 sha1 = new Sha1();
			hashCollection.Add(sha1);
			Xor8 xor8 = new Xor8();
			hashCollection.Add(xor8);
			MultiHash hasher = new MultiHash(hashCollection);
			Assert.Contains(md5, hasher.HashAlgorithms);
			Assert.Contains(sha1, hasher.HashAlgorithms);
			Assert.Contains(xor8, hasher.HashAlgorithms);
		}


		static public object[] HashSizeTests = {
			new object[] { new System.Security.Cryptography.HashAlgorithm[] { new Whirlpool(), new Tiger(TigerParameters.GetParameters(TigerStandard.Tiger192BitVersion1)) }, 512 },
			new object[] { new System.Security.Cryptography.HashAlgorithm[] { new MD5(), new Tiger(TigerParameters.GetParameters(TigerStandard.Tiger192BitVersion1)) }, 192 },
			new object[] { new System.Security.Cryptography.HashAlgorithm[] { new MD5(), new SumBsd(), new Xor8() }, 128 },
		};

		[Test, TestCaseSource("HashSizeTests")]
		public void HashSizeTest(System.Security.Cryptography.HashAlgorithm[] algorithms, int expectedSize) {
			MultiHash hasher = new MultiHash(algorithms);
			Assert.AreEqual(expectedSize, hasher.HashSize);
		}


		[Test]
		public void ChangeHasherGoodTest() {
			MultiHash hasher = new MultiHash(new MD5(), new Sha1());
			hasher.HashAlgorithms.Add(new RipeMD128());
			hasher.TransformBlock(TestVectors.s("1234567890"), 0, 10, null, 0);
			hasher.TransformFinalBlock(new byte[1], 0, 0);
		}


		[Test]
		public void ChangeHasherGoodOrderTest() {
			MultiHash hasher = new MultiHash(new MD5(), new Sha1());
			hasher.TransformBlock(TestVectors.s("1234567890"), 0, 10, null, 0);
			hasher.HashAlgorithms.Reverse();
			hasher.TransformFinalBlock(new byte[1], 0, 0);
		}

	
		[Test, ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
		public void ChangeHasherBadTest() {
			MultiHash hasher = new MultiHash(new MD5(), new Sha1());
			hasher.TransformBlock(TestVectors.s("1234567890"), 0, 10, null, 0);
			hasher.HashAlgorithms.Add(new RipeMD128());
			hasher.TransformFinalBlock(new byte[1], 0, 0);
		}
	}
}
