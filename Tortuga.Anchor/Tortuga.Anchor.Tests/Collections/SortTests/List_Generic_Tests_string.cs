// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this
// file to you under the MIT license. See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Collections.SortTests;

[TestClass]
public class List_Generic_Tests_string : List_Generic_Tests<string>
{
	protected override string CreateT(int seed)
	{
		int stringLength = seed % 10 + 5;
		Random rand = new Random(seed);
		byte[] bytes = new byte[stringLength];
		rand.NextBytes(bytes);
		return Convert.ToBase64String(bytes);
	}
}
