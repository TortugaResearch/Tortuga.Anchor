// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this
// file to you under the MIT license. See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Collections.SortTests;

[TestClass]
public class List_Generic_Tests_int : List_Generic_Tests<int>
{
	protected override int CreateT(int seed)
	{
		var rand = new Random(seed);
		return rand.Next();
	}
}
