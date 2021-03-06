﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using AW.Helper;
using AW.Helper.Annotations;
using AW.LLBLGen.DataContextDriver.Properties;
using AW.Winforms.Helpers;
using AW.Winforms.Helpers.ConnectionUI;
using AW.Winforms.Helpers.Forms;
using AW.Winforms.Helpers.WPF;
using LINQPad.Extensibility.DataContext;
using LINQPad.Extensibility.DataContext.UI;
using Microsoft.Data.ConnectionUI;
using Microsoft.Win32;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using Application = System.Windows.Forms.Application;
using FieldInfo = System.Reflection.FieldInfo;

namespace AW.LLBLGen.DataContextDriver.Static
{
  /// <summary>
  ///   Interaction logic for ConnectionDialog.xaml
  /// </summary>
  [Serializable]
  public partial class ConnectionDialog : INotifyPropertyChanged
  {
    #region Fields

    /// <summary>
    ///   AdapterType
    /// </summary>
    public const string ElementNameAdapterType = "AdapterType";

    public const string ElementNameAdapterAssembly = "AdapterAssembly";

    /// <summary>
    ///   AdditionalAssemblies
    /// </summary>
    public const string ElementNameAdditionalAssemblies = "AdditionalAssemblies";

    public const string ElementNameAdditionalNamespaces = "AdditionalNamespaces";

    public const string ElementNameConnectionType = "ConnectionType";
    public const string FunctionMappingStoreMember = "FunctionMappingStoreMember";
    public const string ElementNameFactoryMethod = "FactoryMethod";
    public const string ElementNameFactoryType = "FactoryType";
    public const string ElementNameFactoryAssembly = "FactoryAssembly";
    public const string ElementNameUseFields = "UseFields";
    public const string ElementNameMembersUseSchema = "UseSchema";
    public const string ElementNameMembersTablePrefixesToGroupBy = "TablePrefixesToGroupBy";
    public const string ElementNameMembersTablePrefixDelimiterToGroupBy = "TablePrefixDelimiterToGroupBy";
    private const string ElementNameDisplayInGrid = "DisplayInGrid";
    private const string ElementNameMembersToExclude = "MembersToExclude";
    public const string TitleChooseLLBLEntityAssembly = "Choose LLBL entity assembly";
    public const string TitleChooseCustomType = "Choose LinqMetaData or ElementCreatorCore Type";
    public const string TitleChooseDataAccessAdapterAssembly = "Choose Data Access Adapter assembly";
    public const string TitleChooseFactoryAssembly = "Choose the assembly containing the factory method";
    public const string TitleChooseApplicationConfigFile = "Choose application config file";
    public const string TitleChooseExtraAssembly = "Choose extra assembly";
    public const string TitleChooseFactoryMethod = "Choose factory method";

    public static readonly string AdditionalAssembliesToolTipF = "The {0} adds these assemblies to the ones LINQPad provides"
                                                                 + Environment.NewLine +
                                                                 LLBLGenStaticDriver.AdditionalAssemblies.JoinAsString()
                                                                 + Environment.NewLine +
                                                                 "If you want any additional assemblies add them in here, with or with out a path.";

    public static readonly string AdditionalAssembliesToolTip = string.Format(AdditionalAssembliesToolTipF, "driver");
    public static readonly string AdditionalAssembliesToolTipCnxt = string.Format(AdditionalAssembliesToolTipF, "connection");

    public static readonly string AdditionalNamespacesToolTipF = "The {0} adds these namespaces to the ones LINQPad provides"
                                                                 + Environment.NewLine +
                                                                 LLBLGenStaticDriver.AdditionalNamespaces.JoinAsString()
                                                                 + Environment.NewLine +
                                                                 "If you want any additional namespaces add them in here.";

    public static readonly string AdditionalNamespacesToolTip = string.Format(AdditionalNamespacesToolTipF, "driver");
    public static readonly string AdditionalNamespacesToolTipCnxt = string.Format(AdditionalNamespacesToolTipF, "connection");

    private static readonly LLBLConnectionType[] AdapterConnectionTypes =
    {
      LLBLConnectionType.Adapter,
      LLBLConnectionType.AdapterFactory
    };

    private readonly bool _isNewConnection;
    private ObservableCollection<ValueTypeWrapper<string>> _additionalAssemblies;
    private bool _providerHasBeenSet;
    private static readonly Type IdataAccessAdapterType = typeof (IDataAccessAdapter);
    private static readonly Type IlinqMetaDataType = typeof (ILinqMetaData);
    private static readonly Type IelementCreatorCoreType = typeof (IElementCreatorCore);
    private static readonly string ORMProfilerPath;

    #endregion

    #region Properties

    public IConnectionInfo CxInfo { get; private set; }

    /// <summary>
    ///   Gets the CustomTypeNameVisibility.
    /// </summary>
    /// <value> The CustomTypeNameVisibility. </value>
    public Visibility CustomTypeNameVisibility
    {
      get { return String.IsNullOrEmpty(txtAssemblyPath.Text) ? Visibility.Collapsed : Visibility.Visible; }
    }

    /// <summary>
    ///   Gets the ConnectionTypeVisibility.
    /// </summary>
    /// <value> The ConnectionTypeVisibility. </value>
    public Visibility ConnectionTypeVisibility
    {
      get { return AdapterConnectionTypes.Contains(LLBLConnectionType) ? Visibility.Visible : Visibility.Collapsed; }
    }

    public LLBLConnectionType LLBLConnectionType
    {
      get
      {
        if (String.IsNullOrEmpty(CxInfo.CustomTypeInfo.CustomTypeName))
          return LLBLConnectionType = LLBLConnectionType.Unknown;
        int connectionTypeIndex;
        if (Int32.TryParse(GetDriverDataValue(ElementNameConnectionType), out connectionTypeIndex))
          return (LLBLConnectionType) connectionTypeIndex;
        return LLBLConnectionType.Unknown;
      }
      set
      {
        var connectionType = ((int) value).ToString();
        if (connectionType != GetDriverDataValue(ElementNameConnectionType))
        {
          SetDriverDataValue(ElementNameConnectionType, connectionType);
          OnPropertyChanged("LLBLConnectionType");
        }
      }
    }

    public string Provider
    {
      get { return _providerHasBeenSet || !_isNewConnection ? CxInfo.DatabaseInfo.Provider : ""; }
      set
      {
        if (CxInfo.DatabaseInfo.Provider != value ||
            String.Equals(value, "System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
        {
          _providerHasBeenSet = value != "";
          CxInfo.DatabaseInfo.Provider = value;
          OnPropertyChanged("Provider");
        }
      }
    }

    public IEnumerable<string> DbProviderFactoryNames { get; set; }

    private DisplayInGrid? HowToDisplayInGrid
    {
      get { return GetHowToDisplayInGrid(CxInfo); }
      set { SetDriverDataValue(CxInfo, ElementNameDisplayInGrid, value.ToString()); }
    }

    public ObservableCollection<ValueTypeWrapper<string>> AdditionalNamespaces { get; set; }

    public ObservableCollection<ValueTypeWrapper<string>> AdditionalAssemblies
    {
      get
      {
        return _additionalAssemblies ??
               (_additionalAssemblies =
                 new ObservableCollection<ValueTypeWrapper<string>>(
                   Settings.Default.AdditionalAssemblies.CreateStringWrapperForBinding()));
      }
    }

    public ObservableCollection<ValueTypeWrapper<string>> AdditionalNamespacesCnxt { get; set; }

    public ObservableCollection<ValueTypeWrapper<string>> AdditionalAssembliesCnxt { get; set; }

    #endregion

    #region General

    static ConnectionDialog()
    {
      CommandManager.RegisterClassCommandBinding(typeof (DataGrid), new CommandBinding(ApplicationCommands.Paste, OnExecutedPaste));
      ORMProfilerPath = ProfilerHelper.GetOrmProfilerPath();
    }

    public ConnectionDialog()
    {
      InitializeComponent();
    }

    public ConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
    {
      CxInfo = cxInfo;
      _isNewConnection = isNewConnection;
      try
      {
        DbProviderFactoryNames = (new[] {""}).Union(DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>().Select(r => r["InvariantName"])).
          Cast<string>().ToList();

        AdditionalNamespaces = new ObservableCollection<ValueTypeWrapper<string>>(Settings.Default.AdditionalNamespaces.CreateStringWrapperForBinding());
        if (isNewConnection)
        {
          CreateDriverDataElements(cxInfo);

          CxInfo.AppConfigPath = Settings.Default.DefaultApplicationConfig;
          CxInfo.DatabaseInfo.CustomCxString = Settings.Default.DefaultDatabaseConnection;
          Provider = Settings.Default.DefaultProvider;
          CxInfo.CustomTypeInfo.CustomAssemblyPath = Settings.Default.DefaultLinqMetaDataAssembly;
          CxInfo.CustomTypeInfo.CustomTypeName = Settings.Default.DefaultLinqMetaData;
          CxInfo.DisplayName = Settings.Default.DefaultDisplayName;
        }
        else
        {
          UpGradeDriverDataElements(cxInfo);
          if (HowToDisplayInGrid == null)
            HowToDisplayInGrid = DisplayInGrid.UseEditableGrid;
        }
        SetAdditionalAssembliesAndNamespaces(cxInfo);
      }
      catch (Exception e)
      {
        e.LogException();
        Application.OnThreadException(e);
      }
      DataContext = this;
      InitializeComponent();
    }

    private void SetAdditionalAssembliesAndNamespaces(IConnectionInfo cxInfo)
    {
      AdditionalNamespacesCnxt = new ObservableCollection<ValueTypeWrapper<string>>(GetDriverDataStringValues(cxInfo, ElementNameAdditionalNamespaces).CreateStringWrapperForBinding());
      AdditionalAssembliesCnxt = new ObservableCollection<ValueTypeWrapper<string>>(GetDriverDataStringValues(cxInfo, ElementNameAdditionalAssemblies).CreateStringWrapperForBinding());
    }

    private static void CreateDriverDataElements(IConnectionInfo cxInfo)
    {
      CreateElementIfNeeded(cxInfo, ElementNameAdapterType, Settings.Default.DefaultAdapterType);
      CreateElementIfNeeded(cxInfo, ElementNameAdapterAssembly, Settings.Default.DefaultAdapterAssembly);
      CreateElementIfNeeded(cxInfo, ElementNameFactoryMethod, Settings.Default.DefaultDataAccessAdapterFactoryMethod);
      CreateElementIfNeeded(cxInfo, ElementNameFactoryType, Settings.Default.DefaultDataAccessAdapterFactoryType);
      CreateElementIfNeeded(cxInfo, ElementNameFactoryAssembly, Settings.Default.DefaultDataAccessAdapterFactoryAssembly);
      CreateElementIfNeeded(cxInfo, ElementNameConnectionType, Settings.Default.DefaultConnectionType.ToString());
      CreateElementIfNeeded(cxInfo, ElementNameUseFields, true.ToString());
      CreateElementIfNeeded(cxInfo, ElementNameDisplayInGrid, DisplayInGrid.UseEditableGrid.ToString());
      CreateElementIfNeeded(cxInfo, ElementNameAdditionalAssemblies);
      CreateElementIfNeeded(cxInfo, ElementNameAdditionalNamespaces);
      CreateElementIfNeeded(cxInfo, ElementNameMembersToExclude);
      CreateElementIfNeeded(cxInfo, ElementNameMembersUseSchema);
      CreateElementIfNeeded(cxInfo, ElementNameMembersTablePrefixesToGroupBy);
      CreateElementIfNeeded(cxInfo, ElementNameMembersTablePrefixDelimiterToGroupBy);
    }

    public static void UpGradeDriverDataElements(IConnectionInfo cxInfo)
    {
      var adaptertypeName = GetDriverDataValue(cxInfo, ElementNameAdapterType);
      if (String.IsNullOrEmpty(adaptertypeName))
        CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.SelfServicing).ToString());
      else if (adaptertypeName == cxInfo.DatabaseInfo.DbVersion)
      {
        CreateElementIfNeeded(cxInfo, ElementNameFactoryMethod, cxInfo.DatabaseInfo.Provider);
        CreateElementIfNeeded(cxInfo, ElementNameFactoryType, cxInfo.DatabaseInfo.DbVersion);
        CreateElementIfNeeded(cxInfo, ElementNameFactoryAssembly, cxInfo.CustomTypeInfo.CustomMetadataPath);
        CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.AdapterFactory).ToString());
        cxInfo.DatabaseInfo.Provider = null;
        cxInfo.DatabaseInfo.DbVersion = null;
        var xElement = cxInfo.DriverData.Element(ElementNameAdapterType);
        if (xElement != null) xElement.Value = String.Empty;
      }
      else
      {
        CreateElementIfNeeded(cxInfo, ElementNameAdapterType);
        CreateElementIfNeeded(cxInfo, ElementNameAdapterAssembly, cxInfo.CustomTypeInfo.CustomMetadataPath);
        CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.Adapter).ToString());
      }
      CreateElementIfNeeded(cxInfo, ElementNameUseFields, true.ToString());
      CreateElementIfNeeded(cxInfo, ElementNameDisplayInGrid, DisplayInGrid.ExcludeEntityBaseProperties.ToString());
      cxInfo.CustomTypeInfo.CustomMetadataPath = null;
      CreateElementIfNeeded(cxInfo, ElementNameAdditionalAssemblies);
      CreateElementIfNeeded(cxInfo, ElementNameAdditionalNamespaces);
      CreateElementIfNeeded(cxInfo, ElementNameMembersToExclude);
      CreateElementIfNeeded(cxInfo, ElementNameMembersUseSchema);
      CreateElementIfNeeded(cxInfo, ElementNameMembersTablePrefixesToGroupBy);
      CreateElementIfNeeded(cxInfo, ElementNameMembersTablePrefixDelimiterToGroupBy);
    }

    private static void CreateElementIfNeeded(IConnectionInfo cxInfo, string elementName, string defaultValue = null)
    {
      if (cxInfo.DriverData.Element(elementName) == null)
      {
        var xElement = new XElement(elementName);
        if (defaultValue != null)
          xElement.Value = defaultValue;
        cxInfo.DriverData.Add(xElement);
      }
    }

    private void SetDriverDataValue(string elementName, string value)
    {
      SetDriverDataValue(CxInfo, elementName, value);
    }

    internal static void SetDriverDataValue(IConnectionInfo connectionInfo, string elementName, string value)
    {
      CreateElementIfNeeded(connectionInfo, elementName, null);
      // ReSharper disable PossibleNullReferenceException
      connectionInfo.DriverData.Element(elementName).Value = value;
      // ReSharper restore PossibleNullReferenceException
    }

    internal static string GetDriverDataValue(IConnectionInfo cxInfo, string elementName)
    {
      var xElement = cxInfo.DriverData.Element(elementName);
      return xElement != null ? xElement.Value : null;
    }

    internal static IEnumerable<string> GetDriverDataStringValues(IConnectionInfo cxInfo, string elementName)
    {
      var xElement = cxInfo.DriverData.Element(elementName);
      if (xElement != null)
      {
        var xElements = xElement.Elements();
        if (xElements.Any())
          return xElements.Select(xe => xe.Value);
        if (!string.IsNullOrWhiteSpace(xElement.Value))
        {
          //var stringBuilder = new StringBuilder(xElement.Value);
          return xElement.Value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
      }
      return Enumerable.Empty<string>();
    }

    internal static bool GetDriverDataBooleanValue(IConnectionInfo cxInfo, string elementName)
    {
      var usefieldsElement = GetDriverDataValue(cxInfo, elementName);
      return usefieldsElement == true.ToString(CultureInfo.InvariantCulture);
    }

    private string GetDriverDataValue(string elementName)
    {
      return GetDriverDataValue(CxInfo, elementName);
    }

    internal static IEnumerable<string> GetAdapterAssemblies(IConnectionInfo cxInfo)
    {
      return GeneralHelper.FilterByFileExists(GetDriverDataValue(cxInfo, ElementNameAdapterAssembly),
        GetDriverDataValue(cxInfo, ElementNameFactoryAssembly));
    }

    public static DisplayInGrid? GetHowToDisplayInGrid(IConnectionInfo cxInfo)
    {
      DisplayInGrid? howToDisplayInGrid = null;
      var useFieldsElement = cxInfo.DriverData.Element(ElementNameDisplayInGrid);
      if (useFieldsElement != null && !String.IsNullOrEmpty(useFieldsElement.Value))
        howToDisplayInGrid = useFieldsElement.Value.ToEnum<DisplayInGrid>();
      return howToDisplayInGrid;
    }

    internal static IEnumerable<string> GetMembersToExclude(IConnectionInfo cxInfo)
    {
      return GetDriverDataStringValues(cxInfo, ElementNameMembersToExclude);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      try
      {
        this.SetPlacement(Settings.Default.ConnectionDialogPlacement);
      }
      catch (Exception ex)
      {
        ex.LogException();
        Application.OnThreadException(ex);
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      base.OnSourceInitialized(e);
      try
      {
        this.SetPlacement(Settings.Default.ConnectionDialogPlacement);
        var settingsViewSource = ((CollectionViewSource) (FindResource("settingsViewSource")));
        settingsViewSource.Source = new List<Settings> {Settings.Default};
        if (!_isNewConnection) btnImportExisting.Visibility = Visibility.Hidden;     //
        SetUriToPath(DriverDirectory, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        SetUriToPath(LINQPadErrorLog, Environment.ExpandEnvironmentVariables(@"%localappdata%\linqpad\logs"));
        SetUriToPath(LINQPadConnections, Environment.ExpandEnvironmentVariables(@"%appdata%\linqpad\ConnectionsV2.xml"));

        AddORMProfilerCnxtButton.IsEnabled = !string.IsNullOrWhiteSpace(ORMProfilerPath);
        AddORMProfilerButton.IsEnabled = AddORMProfilerCnxtButton.IsEnabled;
      }
      catch (Exception ex)
      {
        ex.LogException();
        Application.OnThreadException(ex);
      }
    }

    private static void SetUriToPath(Hyperlink hyperlink, string location)
    {
      if (location != null)
      {
        var uriBuilder = new UriBuilder(location);
        hyperlink.NavigateUri = uriBuilder.Uri;
      }
    }

    private void btnSaveDefault_Click(object sender, RoutedEventArgs e)
    {
      Settings.Default.DefaultDataAccessAdapterFactoryMethod = GetDriverDataValue(ElementNameFactoryMethod);
      Settings.Default.DefaultDataAccessAdapterFactoryType = GetDriverDataValue(ElementNameFactoryType);
      Settings.Default.DefaultDataAccessAdapterFactoryAssembly = GetDriverDataValue(ElementNameFactoryAssembly);

      Settings.Default.DefaultApplicationConfig = CxInfo.AppConfigPath;
      Settings.Default.DefaultDatabaseConnection = CxInfo.DatabaseInfo.CustomCxString;
      Settings.Default.DefaultProvider = Provider;
      Settings.Default.DefaultLinqMetaDataAssembly = CxInfo.CustomTypeInfo.CustomAssemblyPath;
      Settings.Default.DefaultLinqMetaData = CxInfo.CustomTypeInfo.CustomTypeName;

      Settings.Default.DefaultDisplayName = CxInfo.DisplayName;

      Settings.Default.DefaultAdapterType = GetDriverDataValue(ElementNameAdapterType);
      Settings.Default.DefaultAdapterAssembly = GetDriverDataValue(ElementNameAdapterAssembly);
      int connectionTypeIndex;
      if (Int32.TryParse(GetDriverDataValue(ElementNameConnectionType), out connectionTypeIndex))
        Settings.Default.DefaultConnectionType = connectionTypeIndex;

      Settings.Default.Save();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      if (DialogResult.GetValueOrDefault() && !String.IsNullOrEmpty(CxInfo.DatabaseInfo.CustomCxString) &&
          String.IsNullOrEmpty(providerComboBox.Text))
        switch (MessageBox.Show("Database Provider has not been set!" + Environment.NewLine
                                + "This is required to execute SQL" + Environment.NewLine
                                + "Do you wish to close anyway?", "Do you wish to close?", MessageBoxButton.YesNo))
        {
          case MessageBoxResult.Yes:
            break;
          case MessageBoxResult.No:
            e.Cancel = true;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      Settings.Default.ConnectionDialogPlacement = this.GetPlacement();
      Settings.Default.Save();
    }

    private void ConnectionDialog_OnClosed(object sender, EventArgs e)
    {
      try
      {
        SyncDisplayInGrid();
      }
      catch (Exception ex)
      {
        Application.OnThreadException(ex);
      }
    }

    /// <summary>
    ///   Syncs the display in grid source with the target - because sometimes it isn't.
    /// </summary>
    private void SyncDisplayInGrid()
    {
      var displayInGridCxInfo = HowToDisplayInGrid.GetValueOrDefault();
      var displayInGridUI = ComboBoxDisplayInGrid.SelectedValue as DisplayInGrid?;
      if (displayInGridUI != null && displayInGridCxInfo != displayInGridUI)
      {
        ComboBoxDisplayInGrid.ClearValue(Selector.SelectedValueProperty);
        HowToDisplayInGrid = displayInGridUI;
      }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      if (AdditionalAssemblies != null)
      {
        Settings.Default.AdditionalAssemblies = new StringCollection();
        Settings.Default.AdditionalAssemblies.AddRange(AdditionalAssemblies.UnWrap().ToArray());
      }
      if (AdditionalNamespaces != null)
      {
        Settings.Default.AdditionalNamespaces = new StringCollection();
        Settings.Default.AdditionalNamespaces.AddRange(AdditionalNamespaces.UnWrap().ToArray());
      }
      var additionalAssembliesElement = CxInfo.DriverData.Element(ElementNameAdditionalAssemblies);
      if (additionalAssembliesElement != null)
      {
        additionalAssembliesElement.RemoveAll();
        foreach (var assembly in AdditionalAssembliesCnxt.UnWrap().Distinct())
          additionalAssembliesElement.Add(new XElement("Assembly", assembly));
      }

      var additionalNamespacesElement = CxInfo.DriverData.Element(ElementNameAdditionalNamespaces);
      if (additionalNamespacesElement != null)
      {
        additionalNamespacesElement.RemoveAll();
        foreach (var assembly in AdditionalNamespacesCnxt.UnWrap().Distinct())
          additionalNamespacesElement.Add(new XElement("Namespace", assembly));
      }

      try
      {
        DialogResult = true;
      }
      catch (Exception)
      {
        Close();
      }
    }

    #endregion

    #region LLBL Connection Tab

    private void BrowseAppConfig(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        Title = TitleChooseApplicationConfigFile,
        Filter = "Config files (*.config)|*.config|All files (*.*)|*.*",
        DefaultExt = ".config",
      };

      if (dialog.ShowDialog() == true)
        CxInfo.AppConfigPath = dialog.FileName;
    }

    private static void CheckCustomAssemblyIsTheRightVersion()
    {
      var customType = (string) AppDomain.CurrentDomain.GetData("customType");
      try
      {
        var customAssembly = LoadAssembly((string) AppDomain.CurrentDomain.GetData("CustomAssemblyPath"));
        var type = customAssembly.GetType(customType);
        if (!(IlinqMetaDataType.IsAssignableFrom(type) || IelementCreatorCoreType.IsAssignableFrom(type)))
        {
          var fullName = type.FullName.IndexOf("Linq", StringComparison.Ordinal) > -1 ? IlinqMetaDataType.FullName : IelementCreatorCoreType.FullName;
          AppDomain.CurrentDomain.SetData("errorMessage", String.Format("An implementation of {0} was found <{1}>, but it was not for {2}", fullName,
            type.FullName,
            Constants.LLBLGenNameVersion));
          AppDomain.CurrentDomain.SetData("customType", "");
        }
      }
      catch (FileLoadException e)
      {
        if (e.FileName.Contains(DomainIsolator.LlblgenProNameSpace) && !e.Message.Contains(Constants.LLBLVersion))
        {
          var assemblyName = new AssemblyName(e.FileName);
          AppDomain.CurrentDomain.SetData("errorMessage", String.Format("An implementation of {0} was found <{1}>, but it was not for {2} instead it was for version {3}",
            IlinqMetaDataType.FullName, customType, Constants.LLBLGenNameVersion,
            assemblyName.Version));
          AppDomain.CurrentDomain.SetData("customType", "");
        }
        e.LogException();
      }
    }

    private void BrowseAssembly(object sender, RoutedEventArgs e)
    {
      var dialog = new OpenFileDialog
      {
        Title = TitleChooseLLBLEntityAssembly,
        DefaultExt = ".dll",
        FileName = CxInfo.CustomTypeInfo.CustomAssemblyPath,
        Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*"
      };

      if (File.Exists(CxInfo.CustomTypeInfo.CustomAssemblyPath))
        dialog.InitialDirectory = Path.GetDirectoryName(CxInfo.CustomTypeInfo.CustomAssemblyPath);

      if (dialog.ShowDialog() == true)
      {
        CxInfo.CustomTypeInfo.CustomAssemblyPath = dialog.FileName;
        var customTypes = GetLinqMetaDataTypes();

        switch (customTypes.Length)
        {
          case 1:
            SetCustomTypeName(customTypes[0]);
            break;
          case 0:
            SetCustomTypeName("");
            break;
        }
      }
    }

    private void SetCustomTypeName(string customType)
    {
      CxInfo.CustomTypeInfo.CustomTypeName = customType;
      if (String.IsNullOrEmpty(CxInfo.CustomTypeInfo.CustomTypeName))
        LLBLConnectionType = LLBLConnectionType.Unknown;
      else
        LLBLConnectionType = IsSelfServicing(CxInfo)
          ? LLBLConnectionType == LLBLConnectionType.AdapterFactory
            ? LLBLConnectionType
            : LLBLConnectionType.Adapter
          : LLBLConnectionType.SelfServicing;
      OnPropertyChanged("ConnectionTypeVisibility");
    }

    private static bool IsSelfServicing(IConnectionInfo connectionInfo)
    {
      var selfServicingEntities =
        connectionInfo.CustomTypeInfo.GetCustomTypesInAssembly(DomainIsolator.LlblgenProNameSpace + ".ORMSupportClasses.EntityBase");
      return selfServicingEntities.IsNullOrEmpty();
    }

    private void ChooseType(object sender, RoutedEventArgs e)
    {
      var customTypes = GetLinqMetaDataTypes();
      if (customTypes.IsNullOrEmpty())
      {
        SetCustomTypeName("");
        return;
      }
      if (customTypes.Length == 1)
      {
        SetCustomTypeName(customTypes[0]);
        return;
      }
      var result = (string) Dialogs.PickFromList(TitleChooseCustomType, customTypes);
      if (result != null)
        SetCustomTypeName(CxInfo.CustomTypeInfo.CustomTypeName);
    }

    private string[] GetLinqMetaDataTypes()
    {
      if (CxInfo.CustomTypeInfo.CustomAssemblyPath.Length == 0)
      {
        MessageBox.Show("First enter a path to an assembly containing LLBL entities.");
        return null;
      }

      if (!File.Exists(CxInfo.CustomTypeInfo.CustomAssemblyPath))
      {
        MessageBox.Show("File '" + CxInfo.CustomTypeInfo.CustomAssemblyPath + "' does not exist.");
        return null;
      }
      var interfaceTypeName = IlinqMetaDataType.FullName;
      string[] customTypes;
      try
      {
        customTypes = CxInfo.CustomTypeInfo.GetCustomTypesInAssembly(interfaceTypeName);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error obtaining custom types");
        return null;
      }
      if (customTypes.Length == 0)
      {
        interfaceTypeName = IelementCreatorCoreType.FullName;
        customTypes = CxInfo.CustomTypeInfo.GetCustomTypesInAssembly(interfaceTypeName);
        if (customTypes.Length == 0)
          MessageBox.Show("There are no public types in that assembly that implement ILinqMetaData or IElementCreatorCore.",
            "Wrong Assembly chosen");
        else
        {
          MessageBox.Show("There are no public types in that assembly that implement ILinqMetaData but there is an implementation of IElementCreatorCore.");
        }
      }
      if (customTypes.Length == 1)
      {
        using (var domainIsolator = CreateDomainIsolator())
        {
          //domainIsolator.Domain.AssemblyResolve += CurrentDomainAssemblyResolve;
          domainIsolator.Domain.SetData("customType", customTypes[0]);
          domainIsolator.Domain.SetData("CustomAssemblyPath", CxInfo.CustomTypeInfo.CustomAssemblyPath);
          domainIsolator.Domain.DoCallBack(CheckCustomAssemblyIsTheRightVersion);
          var errorMessage = (string) domainIsolator.Domain.GetData("errorMessage");
          customTypes[0] = (string) domainIsolator.Domain.GetData("customType");
          if (!String.IsNullOrEmpty(errorMessage))
          {
            MessageBox.Show(errorMessage);
          }
        }
      }
      return customTypes;
    }

    private void BrowseDataAccessAdapterAssembly(object sender, RoutedEventArgs e)
    {
      var hl = sender as Hyperlink;
      if (hl == null) return;
      var element = CxInfo.DriverData.Element(hl.TargetName);
      if (element == null) return;
      var dialog = new OpenFileDialog
      {
        DefaultExt = ".dll",
        FileName = element.Value,
        Title =
          hl.TargetName.Equals(ElementNameFactoryAssembly)
            ? TitleChooseFactoryAssembly
            : TitleChooseDataAccessAdapterAssembly,
        Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*"
      };
      if (File.Exists(element.Value))
        dialog.InitialDirectory = Path.GetDirectoryName(element.Value);
      if (dialog.ShowDialog() == true)
      {
        element.Value = dialog.FileName;
        var typeName = hl.TargetName.Replace("Assembly", "Type");
        if (hl.TargetName.Contains("Type"))
        {
          using (var domainIsolator = CreateDomainIsolator())
          {
            domainIsolator.Domain.SetData("typeName", hl.TargetName);
            domainIsolator.Domain.SetData("assemblyFileName", dialog.FileName);
            domainIsolator.Domain.DoCallBack(LoadAdapterAssembly);
          }
        }
        else
        {
          ChooseAdapterOrFactoryClass(typeName);
        }
      }
    }

    private void LoadAdapterAssembly()
    {
      var assemblyFile = (string) AppDomain.CurrentDomain.GetData("assemblyFileName");
      var typeName = (string) AppDomain.CurrentDomain.GetData("typeName");
      LoadAdapterAssembly(typeName, assemblyFile);
    }

    private void LoadAdapterAssembly(string typeName, string assemblyFileName)
    {
      var dataAccessAdapterAssembly = LoadAssembly(assemblyFileName);
      try
      {
        var customTypes = GetDataAccessAdapterTypeNames(dataAccessAdapterAssembly);
        if (customTypes.Count() == 1)
          CxInfo.DriverData.SetElementValue(typeName, customTypes.First());
      }
      catch (Exception ex)
      {
        ex.LogException();
      }
    }

    private static IEnumerable<string> GetDataAccessAdapterTypeNames(Assembly dataAccessAdapterAssembly)
    {
      try
      {
        return GetDataAccessAdapterTypeNames(dataAccessAdapterAssembly.GetTypes());
      }
      catch (FileLoadException e)
      {
        if (e.FileName.Contains(DomainIsolator.LlblgenProNameSpace) && !e.Message.Contains(Constants.LLBLVersion))
        {
          var assemblyName = new AssemblyName(e.FileName);
          //MessageBox.Show(string.Format("An implementation of {0} was found <{1}>, but it was not for {2}", interfaceTypeName,
          //                    customType,
          //                    Constants.LLBLGenNameVersion));
          //MessageBox.Show(string.Format("An implementation of {0} was found <{1}>, but it was not for {2} instead it was for version {3}",
          //        IdataAccessAdapterType.FullName, customType, Constants.LLBLGenNameVersion,
          //        assemblyName.Version));
          return Enumerable.Empty<string>();
        }
        e.LogException();
      }
      catch (ReflectionTypeLoadException ex)
      {
        var loaderException = ex.LoaderExceptions[0];
        if (loaderException.Message.Contains(DomainIsolator.LlblgenProNameSpace) && !loaderException.Message.Contains(Constants.LLBLVersion))
          MessageBox.Show(String.Format("The assembly {0} is not for {1}", dataAccessAdapterAssembly.Location, Constants.LLBLGenNameVersion));
        else
          //Dialogs.PickFromList("An array of assemblies in this application domain.",
          //             AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToArray());
          MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine +
                          ex.LoaderExceptions.Select(le => le.Message).Distinct().JoinAsString(Environment.NewLine),
            "Error obtaining adapter types");
      }
      return Enumerable.Empty<string>();
    }

    private static IEnumerable<string> GetDataAccessAdapterTypeNames(IEnumerable<Type> types)
    {
      var implementers = types.FilterByClassIsAssignableTo(IdataAccessAdapterType);
      if (!implementers.Any())
      {
        implementers = types.FilterByImplements(IdataAccessAdapterType.FullName);
        if (implementers.Any())
        {
          var implementer = implementers.First();

          var assembly = implementer.GetInterface(IdataAccessAdapterType.FullName).Assembly;
          var assemblyName = new AssemblyName(assembly.FullName);
          MessageBox.Show(String.Format("An implementation of {0} was found <{1}>, but it was not for {2} instead it was for version {3}",
            IdataAccessAdapterType.FullName, implementer.FullName, Constants.LLBLGenNameVersion,
            assemblyName.Version));
          implementers = Enumerable.Empty<Type>();
        }
        else
        {
          MessageBox.Show("There are no public types in that assembly that implement IDataAccessAdapter.");
        }
      }
      return implementers.Select(t => t.FullName);
    }

    private void ChooseAdapterOrFactoryClass(object sender, RoutedEventArgs e)
    {
      var hl = sender as Hyperlink;
      if (hl != null)
      {
        var targetName = hl.TargetName;
        ChooseAdapterOrFactoryClass(targetName);
      }
    }

    public void ChooseAdapterOrFactoryClass(string targetName)
    {
      var assemPath = GetDriverDataValue(targetName.Replace("Type", "Assembly"));
      if (assemPath.Length == 0)
      {
        MessageBox.Show("First enter a path to an assembly.");
        return;
      }

      if (!File.Exists(assemPath))
      {
        MessageBox.Show("File '" + assemPath + "' does not exist.");
        return;
      }

      using (var domainIsolator = CreateDomainIsolator())
      {
        domainIsolator.Domain.SetData("assemblyFileName", assemPath);
        domainIsolator.Domain.SetData("LLBLConnectionType", LLBLConnectionType);
        try
        {
          domainIsolator.Domain.DoCallBack(GetAdapterOrFactoryClasses);
          var errorMessage = (string) domainIsolator.Domain.GetData("errorMessage");
          var errorMessageTitle = (string) domainIsolator.Domain.GetData("errorMessageTitle");
          var customTypes = (string[]) domainIsolator.Domain.GetData("types");
          if (!String.IsNullOrEmpty(errorMessage))
          {
            MessageBox.Show(errorMessage, errorMessageTitle);
            return;
          }
          if (customTypes.Count() == 1)
            CxInfo.DriverData.SetElementValue(targetName, customTypes.First());
          else if (!customTypes.Any())
          {
            CxInfo.DriverData.SetElementValue(targetName, "");
          }
          else
          {
            var result = (string) Dialogs.PickFromList("Choose " + targetName, customTypes.ToArray());
            if (result != null)
            {
              CxInfo.DriverData.SetElementValue(targetName, result);
              //GetAdapterFactoryMethod(domainIsolator);
              domainIsolator.Domain.SetData("CxInfo", CxInfo);
              domainIsolator.Domain.DoCallBack(GetAdapterFactoryMethod);
              var factoryMethodName = domainIsolator.Domain.GetData(ElementNameFactoryMethod);
              CxInfo.DriverData.SetElementValue(ElementNameFactoryMethod, factoryMethodName);
            }
          }
          FindFunctionMappingStoreMember(domainIsolator);
        }
        catch (Exception ex)
        {
          ex.LogException();
        }
      }
    }

    private void FindFunctionMappingStoreMember(DomainIsolator domainIsolator)
    {
      domainIsolator.Domain.SetData("CxInfo", CxInfo);
      domainIsolator.Domain.DoCallBack(GetFunctionMappingStoreMember);
      var functionMappingStoreMember = domainIsolator.Domain.GetData(FunctionMappingStoreMember);
      CxInfo.DriverData.SetElementValue(FunctionMappingStoreMember, functionMappingStoreMember);
    }

    private static void GetAdapterOrFactoryClasses()
    {
      var errorMessageTitle = "";

      var errorMessage = "";

      var assemPath = (string) AppDomain.CurrentDomain.GetData("assemblyFileName");
      var lLblConnectionType = (LLBLConnectionType) AppDomain.CurrentDomain.GetData("LLBLConnectionType");
      try
      {
        var dataAccessAdapterAssembly = LoadAssembly(assemPath);
        var types = dataAccessAdapterAssembly.GetTypes();
        var customTypes = lLblConnectionType == LLBLConnectionType.Adapter
          ? GetDataAccessAdapterTypeNames(types)
          : types.Select(t => t.FullName).OrderBy(s => s);
        AppDomain.CurrentDomain.SetData("types", customTypes.ToArray());
      }
      catch (ReflectionTypeLoadException ex)
      {
        var loaderException = ex.LoaderExceptions[0];
        if (loaderException.Message.Contains(DomainIsolator.LlblgenProNameSpace) && !loaderException.Message.Contains(Constants.LLBLVersion))
          errorMessage = String.Format("The assembly {0} is not for {1}", assemPath, Constants.LLBLGenNameVersion);
        else
          //Dialogs.PickFromList("An array of assemblies in this application domain.",
          //             AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToArray());
          errorMessage = ex.Message + Environment.NewLine + Environment.NewLine +
                         ex.LoaderExceptions.Select(le => le.Message).Distinct().JoinAsString(Environment.NewLine);
        errorMessageTitle = "Error obtaining adapter types";
      }
      catch (Exception ex)
      {
        errorMessage = "Error obtaining adapter types: " + ex.Message;
        BreakIntoDebugger();
      }
      AppDomain.CurrentDomain.SetData("errorMessageTitle", errorMessageTitle);
      AppDomain.CurrentDomain.SetData("errorMessage", errorMessage);
    }

    private static Assembly LoadAssembly(string assemPath)
    {
      try
      {
        return Assembly.LoadFile(assemPath); // LINQPad.Extensibility.DataContext.DataContextDriver.LoadAssemblySafely(assemPath);
      }
      catch (ReflectionTypeLoadException)
      {
        return Assembly.LoadFrom(assemPath);
      }
    }

    private DomainIsolator CreateDomainIsolator()
    {
      MemoryStream stream = null;
      var assembly = MetaDataHelper.GetAssembly("LINQPad.Common");
      if (assembly != null)
      {
        var formatter = new BinaryFormatter();
        stream = new MemoryStream();
        formatter.Serialize(stream, assembly); //Doesn't work - just does name
      }
      var domainIsolator = new DomainIsolator("Inspect Custom Assembly");
      domainIsolator.AddProbePaths(CxInfo.CustomTypeInfo.CustomAssemblyPath, GetDriverDataValue(CxInfo, ElementNameFactoryAssembly), GetDriverDataValue(CxInfo, ElementNameAdapterAssembly));
      return domainIsolator;
    }

    private void ChooseAdapterFactoryMethod(object sender, RoutedEventArgs e)
    {
      using (var domainIsolator = CreateDomainIsolator())
        GetAdapterFactoryMethod(domainIsolator);
    }

    private void GetAdapterFactoryMethod(DomainIsolator domainIsolator)
    {
      domainIsolator.Domain.SetData("CxInfo", CxInfo);
      domainIsolator.Domain.DoCallBack(GetAdapterFactoryMethod);
      var factoryMethodName = domainIsolator.Domain.GetData(ElementNameFactoryMethod);
      CxInfo.DriverData.SetElementValue(ElementNameFactoryMethod, factoryMethodName);
    }

    private static void GetAdapterFactoryMethod()
    {
      ChooseAdapterFactoryMethod((IConnectionInfo) AppDomain.CurrentDomain.GetData("CxInfo"));
    }

    private static void ChooseAdapterFactoryMethod(IConnectionInfo connectionInfo)
    {
      try
      {
        var factoryTypeName = GetDriverDataValue(connectionInfo, ElementNameFactoryType);
        if (!String.IsNullOrEmpty(factoryTypeName))
        {
          var validMethods = GetMethodsFromFactoryAssembly(connectionInfo, factoryTypeName);
          var count = validMethods.Count();
          if (count == 1)
          {
            var methodInfo = validMethods.Single();
            if (IdataAccessAdapterType.IsAssignableFrom(methodInfo.ReturnType))
            {
              AppDomain.CurrentDomain.SetData(ElementNameFactoryMethod, methodInfo.Name);
            }
            else
            {
              var assembly = methodInfo.ReturnType.GetInterface(IdataAccessAdapterType.FullName).Assembly;
              var assemblyName = new AssemblyName(assembly.FullName);
              MessageBox.Show(
                String.Format(
                  "A method with a return type of {0} was found <{1}>, but it was not for {2} instead it was for version {3}",
                  IdataAccessAdapterType.FullName, methodInfo.Name, Constants.LLBLGenNameVersion,
                  assemblyName.Version));
              AppDomain.CurrentDomain.SetData(ElementNameFactoryMethod, "");
            }
          }
          else
          {
            var result = (MethodInfo) Dialogs.PickFromList(TitleChooseFactoryMethod, validMethods.ToArray());
            if (result != null)
              AppDomain.CurrentDomain.SetData(ElementNameFactoryMethod, result.Name);
          }
        }
      }
      catch (Exception ex)
      {
        Application.OnThreadException(ex);
      }
    }

    private void ChooseLlblFunctionMappingStoreMember(object sender, RoutedEventArgs e)
    {
      using (var domainIsolator = CreateDomainIsolator())
      {
          FindFunctionMappingStoreMember(domainIsolator);
      }
    }
    private static IEnumerable<MethodInfo> GetMethodsFromFactoryAssembly(IConnectionInfo connectionInfo, string factoryTypeName)
    {
      var factoryAssemblyPath = GetDriverDataValue(connectionInfo, ElementNameFactoryAssembly);
      var factoryAdapterAssembly = LoadAssembly(factoryAssemblyPath);
      if (factoryAdapterAssembly == null)
        throw new ApplicationException("Factory assembly: " + factoryAssemblyPath + " could not be loaded!");
      var factoryType = factoryAdapterAssembly.GetType(factoryTypeName);
      if (factoryType == null)
      {
        factoryAdapterAssembly.GetTypes();
        throw new ApplicationException(String.Format("Factory type: {0} could not be loaded from: {1}!", factoryTypeName,
          factoryAssemblyPath));
      }
      var methodInfos = factoryType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
      var validMethods = from m in methodInfos
        let ps = m.GetParameters()
        where
          ps.Length == 1 && ps.Single().ParameterType == typeof (string) &&
          m.ReturnType.Implements(IdataAccessAdapterType)
        select m;
      return validMethods;
    }

    private static void GetFunctionMappingStoreMember()
    {
      ChooseFunctionMappingStoreMember((IConnectionInfo) AppDomain.CurrentDomain.GetData("CxInfo"));
    }

    private static void ChooseFunctionMappingStoreMember(IConnectionInfo connectionInfo)
    {
      try
      {
        var adapterTypeName = GetDriverDataValue(connectionInfo, ElementNameAdapterType);
        if (!String.IsNullOrEmpty(adapterTypeName))
        {
          var validMethods = GetFunctionMappingStoreMember(connectionInfo, adapterTypeName);
          if (validMethods != null)
          {
            AppDomain.CurrentDomain.SetData(FunctionMappingStoreMember, validMethods.Name);
          }
        }
      }
      catch (Exception ex)
      {
        Application.OnThreadException(ex);
      }
    }

    private static MemberInfo GetFunctionMappingStoreMember(IConnectionInfo connectionInfo, string adapterTypeName)
    {
      var factoryAssemblyPath = GetDriverDataValue(connectionInfo, ElementNameAdapterAssembly);
      var factoryAdapterAssembly = LoadAssembly(factoryAssemblyPath);
      if (factoryAdapterAssembly == null)
        throw new ApplicationException("Adapter assembly: " + factoryAssemblyPath + " could not be loaded!");
      var adapterType = factoryAdapterAssembly.GetType(adapterTypeName);
      if (adapterType == null)
      {
        factoryAdapterAssembly.GetTypes();
        throw new ApplicationException(String.Format("Factory type: {0} could not be loaded from: {1}!", adapterTypeName,
          factoryAssemblyPath));
      }
      var functionMappingStoreType = typeof (FunctionMappingStore);
      var memberInfos = adapterType.FindMembers(MemberTypes.Method | MemberTypes.Property | MemberTypes.Field,
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, ReturnTypeFilter, functionMappingStoreType);
      return memberInfos.FirstOrDefault(m => m.DeclaringType == adapterType);
    }

    private static bool ReturnTypeFilter(MemberInfo memberInfo, object filterCriteria)
    {
      var functionMappingStoreType = (Type) filterCriteria;
      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null && propertyInfo.PropertyType == functionMappingStoreType)
        return true;
      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null && methodInfo.ReturnType == functionMappingStoreType)
        return true;
      var fieldInfo = memberInfo as FieldInfo;
      return fieldInfo != null && fieldInfo.FieldType == functionMappingStoreType;
    }

    #endregion

    #region Other Tabs

    [Conditional("DEBUGX")]
    private static void BreakIntoDebugger()
    {
      Debugger.Break();
    }

    //http://www.codeproject.com/Tips/246855/An-alternate-implementation-to-Paste-data-into-the
    //http://blogs.msdn.com/b/vinsibal/archive/2008/09/25/pasting-content-to-new-rows-on-the-wpf-datagrid.aspx
    //http://stackoverflow.com/questions/4118617/wpf-datagrid-pasting
    private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs e)
    {
      var dataGrid = ((DataGrid) sender);
      // 2-dim array containing clipboard data
      var clipboardData = ((string) Clipboard.GetData(DataFormats.Text)).Split('\n')
        .Select(row =>
          row.Split('\t')
            .Select(cell =>
              cell.Length > 0 && cell[cell.Length - 1] == '\r' ?
                cell.Substring(0, cell.Length - 1) : cell).ToArray())
        .Where(a => a.Any(b => b.Length > 0)).ToArray();

      // call OnPastingCellClipboardContent for each cell
      var minRowIndex = dataGrid.Items.IndexOf(dataGrid.CurrentItem);
      var maxRowIndex = dataGrid.Items.Count - 1;
      var minColumnDisplayIndex = (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow) ? dataGrid.Columns.IndexOf(dataGrid.CurrentColumn) : 0;
      var maxColumnDisplayIndex = dataGrid.Columns.Count - 1;
      var rowDataIndex = 0;
      for (var i = minRowIndex; i <= maxRowIndex && rowDataIndex < clipboardData.Count(); i++, rowDataIndex++)
      {
        // dataGrid.CurrentItem = dataGrid.Items[i];
        DataGrid.BeginEditCommand.Execute(null, dataGrid);
        if (dataGrid.CanUserAddRows && i == maxRowIndex)
        {
          // add a new row to be pasted to
          var cv = CollectionViewSource.GetDefaultView(dataGrid.Items);
          var iecv = cv as IEditableCollectionView;
          if (iecv != null)
          {
            iecv.AddNew();
            if (rowDataIndex + 1 < clipboardData.Count())
            {
              // still has more items to paste, update the maxRowIndex
              maxRowIndex = dataGrid.Items.Count - 1;
            }
          }
        }

        var columnDataIndex = 0;
        for (var j = minColumnDisplayIndex; j <= maxColumnDisplayIndex && columnDataIndex < clipboardData[rowDataIndex].Length; j++, columnDataIndex++)
        {
          var column = dataGrid.ColumnFromDisplayIndex(j);
          column.OnPastingCellClipboardContent(dataGrid.Items[i], clipboardData[rowDataIndex][columnDataIndex]);
        }

        DataGrid.CommitEditCommand.Execute(dataGrid, dataGrid);
        if (i == maxRowIndex)
          maxRowIndex++;
      }
    }

    private void ChooseAssemblies(object sender, RoutedEventArgs e)
    {
      ChooseAdditionalAssemblies(AdditionalAssemblies);
    }

    private void ChooseAssembliesCnxt(object sender, RoutedEventArgs e)
    {
      ChooseAdditionalAssemblies(AdditionalAssembliesCnxt);
    }

    private static void ChooseAdditionalAssemblies(ICollection<ValueTypeWrapper<string>> additionalAssemblies)
    {
      var dialog = new OpenFileDialog
      {
        Title = TitleChooseExtraAssembly,
        DefaultExt = ".dll",
        Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*",
        Multiselect = true
      };

      if (dialog.ShowDialog() == true)
      {
        foreach (var fileName in dialog.FileNames
          .Where(fileName => !LLBLGenStaticDriver.AdditionalAssemblies.Contains(Path.GetFileName(fileName))).
          CreateStringWrapperForBinding()
          .Where(fileName => !additionalAssemblies.Contains(fileName))
          )
          additionalAssemblies.Add(fileName);
      }
    }

    private void AddHumanizerCnxt(object sender, RoutedEventArgs e)
    {
      ValueTypeWrapper<string>.Add(AdditionalAssembliesCnxt, "Humanizer.dll");
      ValueTypeWrapper<string>.Add(AdditionalNamespacesCnxt, "Humanizer");
    }

    private void AddORMProfiler(object sender, RoutedEventArgs e)
    {
      AddORMProfiler(AdditionalAssemblies);
    }

    private void AddORMProfilerCnxt(object sender, RoutedEventArgs e)
    {
      AddORMProfiler(AdditionalAssembliesCnxt);
    }

    private static void AddORMProfiler(ObservableCollection<ValueTypeWrapper<string>> additionalAssemblies)
    {
      var ormProfilerPathAssemblies = new[] {ProfilerHelper.OrmProfilerAssemblyFileName, ProfilerHelper.OrmProfilerAssemblyFileName45, "SD.Tools.OrmProfiler.Shared.dll", "SD.Tools.BCLExtensions.dll ", "SD.Tools.Algorithmia.dll"};
   if (AddORMProfiler(additionalAssemblies, ormProfilerPathAssemblies))
        return;
      ValueTypeWrapper<string>.Add(additionalAssemblies, ormProfilerPathAssemblies);
    }

    private static bool AddORMProfiler(ObservableCollection<ValueTypeWrapper<string>> additionalAssemblies, IEnumerable<string> ormProfilerPathAssemblies)
    {
      if (!string.IsNullOrWhiteSpace(ORMProfilerPath))
      {
        if (AddormProfilerPathAssemblies(ORMProfilerPath, ormProfilerPathAssemblies, additionalAssemblies))
          return true;
      }
      return false;
    }

    private static bool AddormProfilerPathAssemblies(string folderPath, IEnumerable<string> ormProfilerPathAssemblies, ObservableCollection<ValueTypeWrapper<string>> additionalAssemblies)
    {
      var result = false;
      foreach (var ormProfilerPathAssemblyPath in ormProfilerPathAssemblies.Select(ormProfilerPathAssembly => Path.Combine(folderPath, ormProfilerPathAssembly)))
      {
        result = File.Exists(ormProfilerPathAssemblyPath);
        if (result)
        {
          ValueTypeWrapper<string>.Add(additionalAssemblies, ormProfilerPathAssemblyPath);
        }
        else
          return false;
      }
      return result;
    }

    private void DataBaseConnectionDialog(object sender, RoutedEventArgs e)
    {
      var dcd = new DataConnectionDialog();
      var dcs = new DataConnectionConfiguration(null);
      dcs.LoadConfiguration(dcd);

      DataConnectionConfiguration.SelectDataProvider(dcd, Provider);

      if (dcd.SelectedDataProvider != null)
        dcd.ConnectionString = CxInfo.DatabaseInfo.CustomCxString;
      if (DataConnectionDialog.Show(dcd) == System.Windows.Forms.DialogResult.OK)
      {
        CxInfo.DatabaseInfo.CustomCxString = dcd.ConnectionString;
        if (dcd.SelectedDataProvider != null)
          Provider = dcd.SelectedDataProvider.Name;
      }
      //dcs.SaveConfiguration(dcd);
    }

    private void buttonClear_Click(object sender, RoutedEventArgs e)
    {
      Settings.Default.Reset();
    }

    private void buttonClearAdditionalClick(object sender, RoutedEventArgs e)
    {
      AdditionalNamespaces.Clear();
      AdditionalAssemblies.Clear();
    }

    private void buttonClearAdditionalNamespacesCnxtClick(object sender, RoutedEventArgs e)
    {
      AdditionalNamespacesCnxt.Clear();
    }

    private void buttonClearAdditionalAssembliesCnxtClick(object sender, RoutedEventArgs e)
    {
      AdditionalAssembliesCnxt.Clear();
    }

    private void AddNamespacesFromAssemblies(object sender, RoutedEventArgs e)
    {
      foreach (var selectedItem in AdditionalAssembliesDataGridCnxt.SelectedItems.OfType<ValueTypeWrapper<string>>())
        ValueTypeWrapper<string>.AddRange(AdditionalNamespacesCnxt, MetaDataHelper.GetNamespaces(selectedItem.Value));
      ValueTypeWrapper<string>.AddRange(AdditionalNamespacesCnxt, MetaDataHelper.GetNamespaces(CxInfo.CustomTypeInfo.CustomAssemblyPath));
      if (LLBLConnectionType != LLBLConnectionType.SelfServicing)
        ValueTypeWrapper<string>.AddRange(AdditionalNamespacesCnxt, MetaDataHelper.GetNamespaces(GetAdapterAssemblies(CxInfo)));
    }

    private void txtAssemblyPath_TextChanged(object sender, TextChangedEventArgs e)
    {
      OnPropertyChanged("CustomTypeNameVisibility");
    }

    private void SetConnectionString_Click(object sender, RoutedEventArgs e)
    {
      CxInfo.DatabaseInfo.CustomCxString = CxInfo.DatabaseInfo.GetCxString();
    }

    private void GetConnectionString_Click(object sender, RoutedEventArgs e)
    {
      if (String.IsNullOrEmpty(providerComboBox.Text))
        MessageBox.Show("Provider has not been set" + Environment.NewLine + "Choose from list.");
      else
        try
        {
          var dbProviderFactory = DbProviderFactories.GetFactory(CxInfo.DatabaseInfo.Provider);
          var connectionStringBuilder = dbProviderFactory.CreateConnectionStringBuilder();
          if (connectionStringBuilder == null)
            connectionStringBuilder = new DbConnectionStringBuilder {ConnectionString = CxInfo.DatabaseInfo.CustomCxString};
          else
            connectionStringBuilder.ConnectionString = CxInfo.DatabaseInfo.CustomCxString;
          if (connectionStringBuilder.ContainsKey("data source"))
            CxInfo.DatabaseInfo.Server = Convert.ToString(connectionStringBuilder["data source"]);
          if (connectionStringBuilder.ContainsKey("initial catalog"))
            CxInfo.DatabaseInfo.Database = Convert.ToString(connectionStringBuilder["initial catalog"]);
          if (connectionStringBuilder.ContainsKey("initial file name"))
            CxInfo.DatabaseInfo.AttachFileName = Convert.ToString(connectionStringBuilder["initial file name"]);
          if (connectionStringBuilder.ContainsKey("user id"))
            CxInfo.DatabaseInfo.UserName = Convert.ToString(connectionStringBuilder["user id"]);
          if (connectionStringBuilder.ContainsKey("password"))
            CxInfo.DatabaseInfo.Password = Convert.ToString(connectionStringBuilder["password"]);
        }
        catch (Exception ex)
        {
          Application.OnThreadException(ex);
        }
    }

    private void ButtonCopyToPlugins_Click(object sender, RoutedEventArgs e)
    {
      var thisDir = Path.GetDirectoryName(GetType().Assembly.Location);
      var userOptionsType = typeof (IConnectionInfo).Assembly.GetType("LINQPad.UserOptions");
      var propertyInfo = userOptionsType.GetProperty("Instance");
      var value = propertyInfo.GetValue(null, null);
      var methodInfo = userOptionsType.GetMethod("GetPluginsFolder");
      var pluginsFolder = methodInfo.Invoke(value, new object[] {true}) as string;
      // string pluginsFolder = value.GetPluginsFolder(true); // LINQPad.UserOptions.Instance.GetPluginsFolder(true);
      if (thisDir != null && pluginsFolder != null)
      {
        var filesToExclude = new[] { "Microsoft.Data.ConnectionUI", "CSScriptLibrary", "Mono.CSharp", "LLBL", "SqlLocalDb" };
        var files = Directory.GetFiles(thisDir, "*.dll");
        foreach (var file in files)
        {
          var fileName = Path.GetFileName(file);
          if (fileName != null)
          {
            if (!filesToExclude.Any(fileName.Contains))
              File.Copy(file, Path.Combine(pluginsFolder, fileName), true);
          }
        }
      }
    }

    private void btnImportExisting_Click(object sender, RoutedEventArgs e)
    {
      var userOptionsType = typeof (IConnectionInfo).Assembly.GetType("LINQPad.Repository");
      var methodInfo = userOptionsType.GetMethod("FromDisk");
      var repositories = methodInfo.Invoke(null, new object[] {}) as IList;
      if (repositories != null)
      {
        var connections = repositories.OfType<IConnectionInfo>();
        var llblConnections = from c in connections
          let driverDataString = c.DriverData.ToString()
          where driverDataString.Contains("Adapter") || driverDataString.Contains("SelfService")
          orderby c.DisplayName
          select c;
        var result = Dialogs.PickFromList("Existing LLBL connection", llblConnections.ToArray()) as IConnectionInfo;
        if (result != null && _isNewConnection)
        {
          CxInfo.DisplayName = result.DisplayName + " Imported";
          CxInfo.DriverData = result.DriverData;
          CxInfo.AppConfigPath = result.AppConfigPath;
          CxInfo.CustomTypeInfo.CustomAssemblyPath = result.CustomTypeInfo.CustomAssemblyPath;
          CxInfo.CustomTypeInfo.CustomTypeName = result.CustomTypeInfo.CustomTypeName;
          CxInfo.CustomTypeInfo.CustomMetadataPath = result.CustomTypeInfo.CustomMetadataPath;
          CxInfo.DatabaseInfo.CustomCxString = result.DatabaseInfo.CustomCxString;
          Provider = result.DatabaseInfo.Provider;
          CxInfo.DatabaseInfo.Server = result.DatabaseInfo.Server;
          CxInfo.DatabaseInfo.Database = result.DatabaseInfo.Database;
          OnPropertyChanged("LLBLConnectionType");
          OnPropertyChanged("ConnectionTypeVisibility");
         // SetAdditionalAssembliesAndNamespaces(CxInfo);
          ValueTypeWrapper<string>.AddRange(AdditionalNamespacesCnxt, GetDriverDataStringValues(result, ElementNameAdditionalNamespaces));
          ValueTypeWrapper<string>.AddRange(AdditionalAssembliesCnxt, GetDriverDataStringValues(result, ElementNameAdditionalAssemblies));

          foreach (var visualChild in AWVisualTreeHelper.GetVisualChildren<TextBox>(this))
          {
            var bindingExpression = visualChild.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null) 
              bindingExpression.UpdateTarget();
          }
        }
      }
    }

    #endregion

    #region Implementation of INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    private void GridSplitter_DragStarted(object sender, DragStartedEventArgs e)
    {
      var scrollView = AWVisualTreeHelper.FindVisualChild<ScrollViewer>(AdditionalAssembliesDataGridCnxt);
      var verticalVisibility = scrollView.ComputedVerticalScrollBarVisibility;
      RowDefAdditionalAssemblies.MaxHeight = verticalVisibility == Visibility.Visible ? double.PositiveInfinity : AdditionalAssembliesDataGridCnxt.ActualHeight;
    }

    private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (RowDefAdditionalAssemblies.Height.Value <= AdditionalAssembliesDataGridCnxt.ActualHeight)
        RowDefAdditionalAssemblies.MaxHeight = double.PositiveInfinity;
      else
      {
        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Tick += SetRowMaxHeightToGrid;
        dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        dispatcherTimer.Start();
      }
    }

    private void SetRowMaxHeightToGrid(object sender, EventArgs eventArgs)
    {
      if (AdditionalAssembliesDataGridCnxt.ActualHeight == RowDefAdditionalAssemblies.Height.Value)
      {
        RowDefAdditionalAssemblies.MaxHeight = double.PositiveInfinity;
        var dispatcherTimer = sender as DispatcherTimer;
        if (dispatcherTimer != null) dispatcherTimer.IsEnabled = false;
      }
      else
        RowDefAdditionalAssemblies.MaxHeight = AdditionalAssembliesDataGridCnxt.ActualHeight;
      gridSplitterAdditionalAssemblies.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
      {
        RoutedEvent = Mouse.MouseDownEvent,
        Source = gridSplitterAdditionalAssemblies,
      });
    }

    private void Url_Click(object sender, RoutedEventArgs e)
    {
      Process.Start(((Hyperlink)sender).NavigateUri.ToString()); 
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      AboutBox.ShowAboutBox();
    }

  }

  public enum LLBLConnectionType
  {
    Unknown,
    SelfServicing,
    Adapter,
    [Description("Adapter Factory")] AdapterFactory
  }
}