﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AW.Data;
using AW.Data.EntityClasses;
using AW.Helper;
using AW.Helper.LLBL;
using AW.LinqToSQL;
using AW.Winforms.Helpers.Controls;
using AW.Winforms.Helpers.DataEditor;
using AW.Winforms.Helpers.LLBL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace AW.Tests
{
	///<summary>
	///	This is a test class for DataEditorExtensionsTest and is intended
	///	to contain all DataEditorExtensionsTest Unit Tests
	///</summary>
	[TestClass]
	public class DataEditorExtensionsTest
	{
		///<summary>
		///	Gets or sets the test context which provides
		///	information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//

		#endregion

		///<summary>
		///	A test for ShowInGrid
		///</summary>
		[TestMethod]
		public void EditInDataGridViewTest()
		{
			TestEditInDataGridView(((IEntity)MetaSingletons.MetaData.AddressType.First()).CustomPropertiesOfType);
			TestEditInDataGridView(MetaDataHelper.GetPropertiesToDisplay(typeof (AddressTypeEntity)));
			TestEditInDataGridView(NonSerializableClass.GenerateList());
			TestEditInDataGridView(SerializableClass.GenerateList());
			TestEditInDataGridView(SerializableClass.GenerateListWithBoth());
			TestEditInDataGridView(null);

			var arrayList = new ArrayList {1, 2, "3"};
			TestEditInDataGridView(arrayList);
		}

		[TestMethod]
		public void GridDataEditorBindEnumerableTest()
		{
			var gridDataEditor = new GridDataEditor();
			gridDataEditor.BindEnumerable(null, 1);

			var arrayList = new ArrayList { 1, 2, "3" };
			gridDataEditor.BindEnumerable(arrayList);

			gridDataEditor.BindEnumerable(arrayList, 1);
		}

		[TestMethod]
		public void ShowStringEnumerationInGridTest()
		{
			var enumerable = new[] { "s1", "s2", "s3" };
			TestEditInDataGridView(enumerable);
			enumerable = null;
			TestEditInDataGridView(enumerable);
			TestEditInDataGridView(new string[0]);
		}

		[TestMethod]
		public void EditEmptyInDataGridViewTest()
		{
			TestEditInDataGridView(new SerializableClass[0]);
		}

		[TestMethod]
		public void EditPagedQueryInDataGridViewTest()
		{
			var addressEntities = MetaSingletons.MetaData.Address.SkipTake(1, 15);
			addressEntities.ShowSelfServicingInGrid(20);
			addressEntities.ShowSelfServicingInGrid(0);
		}

		[TestMethod]
		public void QueryInGridIsReadonlyTest()
		{
			TestEditInDataGridView(MetaSingletons.MetaData.Address);
		}

		[TestMethod]
		public void ShowSelfServicingInGridTest()
		{
			MetaSingletons.MetaData.Address.ShowSelfServicingInGrid();
			MetaSingletons.MetaData.AddressType.ShowSelfServicingInGrid();
		}

		[TestMethod]
		public void ShowEntityCollectionInGridTest()
		{
			TestEditInDataGridView(MetaSingletons.MetaData.Address.ToEntityCollection());
			TestEditInDataGridView(MetaSingletons.MetaData.AddressType.ToEntityCollection());
		}

		[TestMethod]
		public void EditLinqtoSQLInDataGridViewTest()
		{
			var awDataClassesDataContext = AWDataClassesDataContext.GetNew();
			//		awDataClassesDataContext. = DbUtils.ActualConnectionString;
			//awDataClassesDataContext.Connection.ConnectionString
			TestEditInDataGridView(awDataClassesDataContext.AddressTypes);
			var addressTypesQuery = awDataClassesDataContext.AddressTypes.OrderByDescending(at => at.AddressTypeID);
			addressTypesQuery.ShowInGrid(awDataClassesDataContext);
			//TestEditInDataGridView(awDataClassesDataContext.);
			var actual = awDataClassesDataContext.AddressTypes.ShowInGrid();
			Assert.AreEqual(awDataClassesDataContext.AddressTypes, actual);
		}

		private static void TestEditInDataGridView<T>(IEnumerable<T> enumerable)
		{
			var actual = enumerable.ShowInGrid(null);
			Assert.AreEqual(enumerable, actual);
		}

		private static void TestEditInDataGridView(IEnumerable enumerable)
		{
			var actual = enumerable.ShowInGrid(null);
			Assert.AreEqual(enumerable, actual);
		}
	}
}