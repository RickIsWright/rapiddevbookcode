﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AW.Data.FactoryClasses;
using AW.Data.Linq;
using AW.Helper;
using AW.Helper.LLBL;
using AW.LLBLGen.DataContextDriver.Static;
using FluentAssertions;
using LINQPad.Extensibility.DataContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Northwind.DAL;
using Northwind.DAL.EntityClasses;
using Northwind.DAL.SqlServer;
using SD.LLBLGen.Pro.ORMSupportClasses;
using CustomerEntity = AW.Data.EntityClasses.CustomerEntity;
using ElementCreator = Northwind.DAL.FactoryClasses.ElementCreator;
using EntityType = AW.Data.EntityType;

namespace AW.LLBLGen.DataContextDriver.Tests
{
  /// <summary>
  ///   This is a test class for LLBLGenStaticDriverTest and is intended to contain all LLBLGenStaticDriverTest Unit Tests
  /// </summary>
  [TestClass]
  public class LLBLGenStaticDriverTest
  {
    /// <summary>
    ///   Gets or sets the test context which provides information about and functionality for the current test run.
    /// </summary>
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

    /// <summary>
    ///   A test for GetSchema using AW
    /// </summary>
    [TestMethod]
    public void GetAWSchemaTest()
    {
      var customType = typeof (LinqMetaData);
      var explorerItems = GetSchemaTest<EntityType>(customType, EntityFactoryFactory.GetFactory, true);
      var customerExplorerItem = explorerItems.First(e => e.Text == EntityHelper.GetNameFromEntityEnum(EntityType.CustomerEntity));
      IEntityCore customerEntity = new CustomerEntity();
      var msDescription = customerEntity.CustomPropertiesOfType.Values.First();
      StringAssert.Contains(customerExplorerItem.ToolTipText, msDescription);

      var firstExplorerItem = customerExplorerItem.Children.First();
      Assert.IsFalse(string.IsNullOrWhiteSpace(firstExplorerItem.ToolTipText));
      var firstField = (IEntityField) customerEntity.Fields[0];
      StringAssert.Contains(firstExplorerItem.ToolTipText, firstField.SourceColumnName);

      StringAssert.Contains(customerExplorerItem.ToolTipText, firstField.SourceObjectName);
      StringAssert.Contains(customerExplorerItem.ToolTipText, firstField.SourceCatalogName);
      StringAssert.Contains(customerExplorerItem.ToolTipText, firstField.SourceSchemaName);

      var individualExplorerItem = explorerItems.First(e => e.Text.Contains(EntityHelper.GetNameFromEntityEnum(EntityType.IndividualEntity)));
      foreach (var explorerItem in customerExplorerItem.Children)
      {
        var item = individualExplorerItem.Children.Single(c => c.Text == explorerItem.Text);
        Assert.AreEqual(explorerItem.ToolTipText, item.ToolTipText);
      }
    }

    /// <summary>
    ///   A test for GetSchema using AW
    /// </summary>
    [TestMethod]
    public void GetAWSchemaPropertiesTest()
    {
      var customType = typeof (LinqMetaData);
      GetSchemaTest<EntityType>(customType, EntityFactoryFactory.GetFactory, false, 3);
    }

    /// <summary>
    ///   A test for GetSchema using Northwind
    /// </summary>
    [TestMethod]
    public void GetNorthwindSchemaFieldsTest()
    {
      var explorerItems = GetSchemaTest<Northwind.DAL.EntityType>(typeof (Northwind.DAL.Linq.LinqMetaData),
                                                                  Northwind.DAL.FactoryClasses.EntityFactoryFactory.GetFactory, true);

      var customerExplorerItem = TestNorthWindToolTips(explorerItems);
      var employeeExplorerItem = customerExplorerItem.Children.Single(e => e.Text == "EmployeesViaOrders");
      Assert.AreEqual(ExplorerIcon.ManyToMany, employeeExplorerItem.Icon);

    }

    private static ExplorerItem TestNorthWindToolTips(List<ExplorerItem> explorerItems, bool testForeignKey = true)
    {
      var customerName = EntityHelper.GetNameFromEntityEnum(Northwind.DAL.EntityType.CustomerEntity);
      var explorerItem = explorerItems.First(e => e.Text == customerName);

      var orderExplorerItem = explorerItems.Single(e => e.Text == EntityHelper.GetNameFromEntityEnum(Northwind.DAL.EntityType.OrderEntity));
      var customerNavigator = orderExplorerItem.Children.Single(e => e.Text == customerName);

      var customerEntityType = typeof (Northwind.DAL.EntityClasses.CustomerEntity);
      var displayNameAttributes = MetaDataHelper.GetDisplayNameAttributes(customerEntityType).ToList();
      foreach (var displayNameAttribute in displayNameAttributes)
      {
        explorerItem.ToolTipText.Should().Contain(displayNameAttribute.DisplayName);
        customerNavigator.ToolTipText.Should().Contain(displayNameAttribute.DisplayName);
      }
      var descriptionAttributes = MetaDataHelper.GetDescriptionAttributes(customerEntityType).ToList();
      foreach (var descriptionAttribute in descriptionAttributes)
      {
        explorerItem.ToolTipText.Should().Contain(descriptionAttribute.Description);
        customerNavigator.ToolTipText.Should().Contain(descriptionAttribute.Description);
      }

      var orderEntityType = typeof (OrderEntity);
      var orderPropertiesToShowInSchema = LLBLGenDriverHelper.GetPropertiesToShowInSchema(orderEntityType);
      var customerPropertyDescriptor = orderPropertiesToShowInSchema.Single(p => p.Name == customerName);
      StringAssert.Contains(customerNavigator.ToolTipText, customerPropertyDescriptor.Description);
      StringAssert.Contains(customerNavigator.ToolTipText, customerPropertyDescriptor.DisplayName);

      if (testForeignKey)
        StringAssert.Contains(customerNavigator.ToolTipText, OrderFieldIndex.CustomerId.ToString());

      var first = explorerItem.Children.First();
      Assert.IsFalse(string.IsNullOrWhiteSpace(first.ToolTipText));
      var customerPropertiesToShowInSchema = LLBLGenDriverHelper.GetPropertiesToShowInSchema(customerEntityType);
      var description = customerPropertiesToShowInSchema.First().Description;
      Assert.IsFalse(String.IsNullOrEmpty(description));
      Assert.IsTrue(first.ToolTipText.Contains(description));

      var orderDetailExplorerItem = explorerItems.Single(e => e.Text == EntityHelper.GetNameFromEntityEnum(Northwind.DAL.EntityType.OrderDetailEntity));
      var quantityExplorerItem = orderDetailExplorerItem.Children.Single(ei => ei.DragText == "Quantity");
      StringAssert.Contains(quantityExplorerItem.ToolTipText, StringConstants.QuantityDescription);

      var contactTitleExplorerItem = explorerItem.Children.Single(e => e.DragText == "ContactTitle");
      contactTitleExplorerItem.ToolTipText.Should().Contain("Contact Title");
      return explorerItem;
    }

    [TestMethod]
    public void GetNorthwindElementCreatorSchemaFieldsTest()
    {
      var explorerItems = GetSchemaTest<Northwind.DAL.EntityType>(typeof (ElementCreator),
                                                                  Northwind.DAL.FactoryClasses.EntityFactoryFactory.GetFactory, true);
      TestNorthWindToolTips(explorerItems);
    }

    [TestMethod]
    public void GetNorthwindSchemaPropertiesTest()
    {
      var explorerItems = GetSchemaTest<Northwind.DAL.EntityType>(typeof (Northwind.DAL.Linq.LinqMetaData),
                                                                  Northwind.DAL.FactoryClasses.EntityFactoryFactory.GetFactory, false);
      TestNorthWindToolTips(explorerItems, false);
    }

    private static List<ExplorerItem> GetSchemaTest<TEnum>(Type customType, Func<TEnum, IEntityFactoryCore> entityFactoryFactory, bool useFields, byte additionalLinqMetaDataProperties = 0)
    {
      var entityTypes = GeneralHelper.EnumAsEnumerable<TEnum>();
      var target = new LLBLGenStaticDriver();
      var mockedIConnectionInfo = MockedIConnectionInfo(useFields);

      var actual = target.GetSchema(mockedIConnectionInfo.Object, customType);
      Assert.AreEqual(entityTypes.Length + additionalLinqMetaDataProperties, actual.Count);
      var orderedEnumerable = entityTypes.OrderBy(et => EntityHelper.GetNameFromEntityName(et.ToString()));
      var index = 0;
      foreach (var entityType in orderedEnumerable)
      {
        var entityFactory = entityFactoryFactory(entityType);
        var entity = entityFactory.Create();
        var fieldNames = entity.Fields.Select(f => f.Name).Distinct();
        var navigatorProperties = useFields ? EntityHelper.GetNavigatorProperties(entity) : MetaDataHelper.GetPropertyDescriptors(entity.GetType()).FilterByIsEnumerable(typeof (IEntityCore));
        ExplorerItem explorerItem;
        do
        {
          explorerItem = actual[index];
          index++;
        } while (explorerItem.Tag != null && (Type) explorerItem.Tag != entity.GetType());
        var entityName = entityFactory.ForEntityName + " - " + explorerItem.Text;
        var notCoreCount = navigatorProperties.Count(er => !EntityHelper.IsEntityCore(er));
        var coreCount = navigatorProperties.Count(propertyDescriptor => EntityHelper.IsEntityCore(propertyDescriptor));
        var propertyCount = explorerItem.Children.Count(ei => ei.Kind == ExplorerItemKind.Property);
        var collectionLinkCount = explorerItem.Children.Count(ei => ei.Kind == ExplorerItemKind.CollectionLink);
        var referenceLinkCount = explorerItem.Children.Count(ei => ei.Kind == ExplorerItemKind.ReferenceLink);
        if (useFields)
        {
          Assert.AreEqual(fieldNames.Count(), propertyCount, entityName + " - fieldNames -" + fieldNames.JoinAsString());
          Assert.AreEqual(notCoreCount, collectionLinkCount, entityName + " - Many navigator");
          Assert.AreEqual(coreCount, referenceLinkCount, entityName + " - single navigator");
        }
        else
        {
          Assert.IsTrue(fieldNames.Count() <= propertyCount, entityName + " - fieldNames -" + fieldNames.JoinAsString());
          Assert.IsTrue(notCoreCount <= collectionLinkCount, entityName + " - Many navigator");
          Assert.IsTrue(coreCount <= referenceLinkCount, entityName + " - single navigator");
        }
      }
      return actual;
    }

    private static Mock<IConnectionInfo> MockedIConnectionInfo(bool useFields)
    {
      var mockedICustomTypeInfo = new Mock<ICustomTypeInfo>();
      var mockedIConnectionInfo = new Mock<IConnectionInfo>();
      var mockedIDatabaseInfo = new Mock<IDatabaseInfo>();
      var xElementDriverData = new XElement("DriverData");
      var xElementUseFields = new XElement(ConnectionDialog.ElementNameUseFields) {Value = useFields.ToString()};
      xElementDriverData.Add(xElementUseFields);
      mockedIConnectionInfo.Setup(ci => ci.DriverData).Returns(xElementDriverData);
      mockedIConnectionInfo.Setup(ci => ci.CustomTypeInfo).Returns(mockedICustomTypeInfo.Object);
      mockedIConnectionInfo.Setup(ci => ci.DatabaseInfo).Returns(mockedIDatabaseInfo.Object);
      ConnectionDialog.SetDriverDataValue(mockedIConnectionInfo.Object, ConnectionDialog.ElementNameAdapterType, "Northwind.DAL.SqlServer.DataAccessAdapter");
      ConnectionDialog.SetDriverDataValue(mockedIConnectionInfo.Object, ConnectionDialog.ElementNameAdapterAssembly, "Northwind.DAL.SqlServer.dll");
      return mockedIConnectionInfo;
    }


    //[TestMethod]
    //public void GetFieldsToShowInSchemaTest()
    //{
    //  var fieldsToShowInSchema = LLBLGenStaticDriver.GetFieldsToShowInSchema(typeof (Northwind.DAL.EntityClasses.CustomerEntity));
    //  Assert.IsFalse(String.IsNullOrEmpty(fieldsToShowInSchema.First().Description));
    //}

    [TestMethod]
    public void GetPropertiesToShowInSchemaTest()
    {
      var propertiesToShowInSchema = LLBLGenDriverHelper.GetPropertiesToShowInSchema(typeof (Northwind.DAL.EntityClasses.CustomerEntity));
      Assert.IsFalse(String.IsNullOrEmpty(propertiesToShowInSchema.First().Description));
    }

    [TestMethod]
    public void GetPropertiesToShowInSchemaBuddyTest()
    {
      var type = typeof (OrderDetailEntity);
      MetaDataHelper.AddAssociatedMetadataProvider(type);
      var descriptionAttributes = MetaDataHelper.GetDescriptionAttributes(type, OrderDetailFieldIndex.Quantity.ToString());
      Assert.IsTrue(descriptionAttributes.Any());
      var propertiesToShowInSchema = LLBLGenDriverHelper.GetPropertiesToShowInSchema(type);
      Assert.IsFalse(String.IsNullOrEmpty(propertiesToShowInSchema.GetFieldPropertyDescriptor(OrderDetailFieldIndex.Quantity.ToString()).Description));
    }

    [TestProperty("Winforms", "Interactive"), TestMethod]
    public void ConnectionDialogTest()
    {
      var mockedIConnectionInfo = MockedIConnectionInfo(true);
      ConnectionDialog.SetDriverDataValue(mockedIConnectionInfo.Object, ConnectionDialog.ElementNameAdapterAssembly, Path.GetFullPath("Northwind.DAL.SqlServer.dll"));
      var connectionDialog = new ConnectionDialog(mockedIConnectionInfo.Object, false);
      connectionDialog.ChooseAdapterOrFactoryClass("AdapterAssembly");
    }

    [TestMethod]
    public void SetSQLTranslationWriterTest()
    {
      var marshalByRefClass = new MarshalByRefClass();
      marshalByRefClass.SetSQLTranslationWriterTest();
    }

    [TestMethod]
    public void SetSQLTranslationWriterDomainIsolatorTest()
    {
      var domainIsolator = new DomainIsolator("friendlyName");
      domainIsolator.GetInstance<MarshalByRefClass>().SetSQLTranslationWriterTest();
    }

    [TestCategory("OrmProfiler"), TestMethod, Ignore]
    public void GetFactoryAfterInterceptorCoreInitializeDomainIsolatorTest()
    {
      var appDomainSetup = new AppDomainSetup
        {
          ApplicationBase = Environment.GetEnvironmentVariable("TEMP")
        };
      var domainIsolator = new DomainIsolator("friendlyName", appDomainSetup);
      domainIsolator.GetInstance<MarshalByRefClass>().GetFactoryAfterInterceptorCoreInitializeTest();
    }

    [TestCategory("OrmProfiler"), TestMethod]
    public void GetFactoryAfterInterceptorCoreInitializeTest()
    {
      var marshalByRefClass = new MarshalByRefClass();
      marshalByRefClass.GetFactoryAfterInterceptorCoreInitializeTest();
    }
  }

  internal class MarshalByRefClass : MarshalByRefObject
  {
    public void GetFactoryAfterInterceptorCoreInitializeTest()
    {
      var programFilesPathx86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      var ormProfilerAssemblyFilePath = Path.Combine(Path.Combine(programFilesPathx86, ProfilerHelper.SolutionsDesignOrmProfilerPath), ProfilerHelper.OrmProfilerAssemblyFileName);
      Assert.IsTrue(File.Exists(ormProfilerAssemblyFilePath), ormProfilerAssemblyFilePath);
      var interceptorAssembly = Assembly.LoadFrom(ormProfilerAssemblyFilePath);
      var type = interceptorAssembly.GetType(ProfilerHelper.OrmProfilerInterceptorTypeName);
      Assert.IsNotNull(type);
      ProfilerHelper.InterceptorCoreInitialize(type);
      //CommonEntityBase.Initialize();
      //Assert.AreEqual(type, Type.GetType(type.AssemblyQualifiedName));
      //Assert.AreEqual(type, Type.GetType(ProfilerHelper.OrmProfilerInterceptorAssemblyQualifiedTypeName));
      DbProviderFactories.GetFactory("System.Data.SqlClient");
    }

    public void SetSQLTranslationWriterTest()
    {
      var dataAccessAdapter = new DataAccessAdapter();
      var eventInfo = dataAccessAdapter.GetType().GetEvent(SQLTraceEventArgs.SqlTraceEventName);
      Assert.IsNotNull(eventInfo);

      TextWriter writer = null;
      var handler2 = new Action<object, EventArgs>((sender, e) =>
        {
          if (e != null)
          {
            //var x = 1 + 2;
            //SQLTraceEventHandler(sender, e);
            SQLTraceEventArgs.WriteSQLTranslation(writer, null);
          }
        });
      Delegate.CreateDelegate(eventInfo.EventHandlerType, writer, handler2.Method);

      EventHandler<EventArgs> handler3 = (sender, e) => SQLTraceEventArgs.WriteSQLTranslation(writer, e);
      Delegate.CreateDelegate(eventInfo.EventHandlerType, writer, handler3.Method);

      var handler4 = new EventHandler<EventArgs>((sender, e) => SQLTraceEventArgs.WriteSQLTranslation(writer, e));
      Delegate.CreateDelegate(eventInfo.EventHandlerType, writer, handler4.Method);

      var llblGenStaticDriver = new LLBLGenStaticDriver();
      llblGenStaticDriver.SubscribeToSqlTraceEvent(dataAccessAdapter, eventInfo, writer);
    }
  }
}