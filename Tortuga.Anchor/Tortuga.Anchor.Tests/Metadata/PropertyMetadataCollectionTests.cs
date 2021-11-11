using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortuga.Anchor.Metadata;
using Tortuga.Dragnet;

namespace Tests.Metadata;

[TestClass]
public class PropertyMetadataCollectionTests
{
	[TestMethod]
	public void PropertyMetadataCollection_CopyTo_Test1()
	{
		using (var verify = new Verify())
		{
			try
			{
				var result = MetadataCache.GetMetadata(typeof(Tests.Metadata.Mock)).Properties;
				result.CopyTo(null!, 0);

				verify.Fail("Expected an ArgumentNullException");
			}
			catch (ArgumentNullException ex)
			{
				verify.AreEqual("array", ex.ParamName, "Incorrect parameter name");
			}
		}
	}

	[TestMethod]
	public void PropertyMetadataCollection_CopyTo_Test2()
	{
		using (var verify = new Verify())
		{
			try
			{
				var result = MetadataCache.GetMetadata(typeof(Tests.Metadata.Mock)).Properties;
				var array = new PropertyMetadata[0];
				result.CopyTo(array, 0);

				verify.Fail("Expected an ArgumentNullException");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				verify.AreEqual("arrayIndex", ex.ParamName, "Incorrect parameter name");
			}
		}
	}

	[TestMethod]
	public void PropertyMetadataCollection_CopyTo_Test3()
	{
		using (var verify = new Verify())
		{
			var result = MetadataCache.GetMetadata(typeof(Tests.Metadata.Mock)).Properties;
			var array = new PropertyMetadata[result.Count];
			result.CopyTo(array, 0);
			foreach (var p in result)
				CollectionAssert.Contains(array, p);
		}
	}

	[TestMethod]
	public void PropertyMetadataCollection_CopyTo_Test4()
	{
		using (var verify = new Verify())
		{
			try
			{
				var result = MetadataCache.GetMetadata(typeof(Tests.Metadata.Mock)).Properties;
				var array = new PropertyMetadata[0];
				result.CopyTo(array, 1);

				verify.Fail("Expected an ArgumentNullException");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				verify.AreEqual("arrayIndex", ex.ParamName, "Incorrect parameter name");
			}
		}
	}

	[TestMethod]
	public void PropertyMetadataCollection_CopyTo_Test5()
	{
		using (var verify = new Verify())
		{
			try
			{
				var result = MetadataCache.GetMetadata(typeof(Tests.Metadata.Mock)).Properties;
				var array = new PropertyMetadata[0];
				result.CopyTo(array, -1);

				verify.Fail("Expected an ArgumentNullException");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				verify.AreEqual("arrayIndex", ex.ParamName, "Incorrect parameter name");
			}
		}
	}


	[TestMethod]
	public void ColumnMappingTest()
	{
		using (var verify = new Verify())
		{
			var properties = MetadataCache.GetMetadata(typeof(DataMapMock)).Properties;
			Assert.AreEqual("Column1", properties["Column1"].MappedColumnName);
			Assert.AreEqual("ColumnB", properties["Column2"].MappedColumnName);
			Assert.IsNull(properties["Column3"].MappedColumnName);

			Assert.IsFalse(properties["Column1"].IsKey);
			Assert.IsFalse(properties["Column2"].IsKey);
			Assert.IsFalse(properties["Column3"].IsKey);
			Assert.IsTrue(properties["Column4"].IsKey);
		}
	}


}
