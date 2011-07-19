﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml.Linq;
using AW.Helper;
using AW.LLBLGen.DataContextDriver.Properties;
using AW.Winforms.Helpers;
using AW.Winforms.Helpers.ConnectionUI;
using LINQPad.Extensibility.DataContext;
using LINQPad.Extensibility.DataContext.UI;
using Microsoft.Data.ConnectionUI;
using Microsoft.Win32;
using SD.LLBLGen.Pro.ORMSupportClasses;
using Application = System.Windows.Forms.Application;

namespace AW.LLBLGen.DataContextDriver.Static
{
	/// <summary>
	/// 	Interaction logic for ConnectionDialog.xaml
	/// </summary>
	public partial class ConnectionDialog : INotifyPropertyChanged
	{
		private readonly bool _isNewConnection;
		private bool _providerHasBeenSet;

		/// <summary>
		/// AdapterType
		/// </summary>
		public const string ElementNameAdaptertype = "AdapterType";

		public const string ElementNameAdapterAssembly = "AdapterAssembly";

		/// <summary>
		/// AdditionalAssemblies
		/// </summary>
		public const string ElementNameAdditionalassemblies = "AdditionalAssemblies";

		public const string ElementNameConnectionType = "ConnectionType";
		public const string ElementNameFactoryMethod = "FactoryMethod";
		public const string ElementNameFactoryType = "FactoryType";
		public const string ElementNameFactoryAssembly = "FactoryAssembly";
		public const string ElementNameUseFields = "UseFields";
		public const string TitleChooseLLBLEntityAssembly = "Choose LLBL entity assembly";
		public const string TitleChooseCustomType = "Choose LinqMetaData or ElementCreatorCore Type";
		public const string TitleChooseDataAccessAdapterAssembly = "Choose Data Access Adapter assembly";
		public const string TitleChooseFactoryAssembly = "Choose the assembly containing the factory method";
		public const string TitleChooseApplicationConfigFile = "Choose application config file";
		public const string TitleChooseExtraAssembly = "Choose extra assembly";
		public const string TitleChooseFactoryMethod = "Choose factory method";

		public static readonly string AdditionalAssembliesToolTip = "The driver adds these assemblies to the ones LINQPad provides"
		                                                            + Environment.NewLine + LLBLGenStaticDriver.AdditionalAssemblies.JoinAsString()
		                                                            + Environment.NewLine + "If you want any additional assemblies add them in here, with or with out a path.";

		public static readonly string AdditionalNamespacesToolTip = "The driver adds these namespaces to the ones LINQPad provides"
		                                                            + Environment.NewLine + LLBLGenStaticDriver.AdditionalNamespaces.JoinAsString()
		                                                            + "If you want any additional namespaces add them in here.";

		public IConnectionInfo CxInfo { get; private set; }

		//static ConnectionDialog()
		//{
		//  AppDomain.CurrentDomain. ReflectionOnlyAssemblyResolve  += MyResolveEventHandler;
		//}

		private Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			var shortAssemblyName = new AssemblyName(args.Name).Name;
			if (File.Exists(CxInfo.CustomTypeInfo.CustomAssemblyPath))
			{
				var directoryName = Path.GetDirectoryName(CxInfo.CustomTypeInfo.CustomAssemblyPath);
				var path = Path.Combine(directoryName, shortAssemblyName) + ".dll";
				if (File.Exists(path))
					return Assembly.ReflectionOnlyLoadFrom(path);
			}
			//  // Look in the GAC.  It may not be there, so we need to catch
			//  // FileNotFoundException.
			//  // As an optimization, don't probe in the GAC if the public
			//  // key token is null, because by definition the assembly
			//  // can't be in the GAC.
			if (!args.Name.Contains("PublicKeyToken=null"))
			{
				try
				{
					return Assembly.ReflectionOnlyLoad(args.Name);
				}
				catch (FileNotFoundException)
				{
				}

				try
				{
					// As a final effort, look in the GAC after appying loader policy.
					// This lets us resolves references for assemblies built against an older version of the framework.
					return Assembly.ReflectionOnlyLoad(AppDomain.CurrentDomain.ApplyPolicy(args.Name));
				}
				catch (FileNotFoundException)
				{
				}
			}
			return null;
		}

		// Soon, this assembly resolve event won't be used during inspection.
		// But we'll need this exact same code during activation.
		//internal Assembly ResolveAssembly(Object sender, ResolveEventArgs args)
		//{
		//  String assemblyRef = args.Name;
		//  // I need to look in both the directory structure on disk for add-in
		//  // pipeline components, as well as in the GAC for assemblies like
		//  // System.dll, which I haven't pre-loaded into the ReflectionOnly
		//  // loader context.
		//  // LoadFrom respects publisher policy, but ReflectionOnlyLoadFrom doesn't,
		//  // but that's OK.
		//  // Do I have to look in the GAC _first_ to mimic behavior
		//  // at runtime, which would respect policy when calling LoadFrom?
		//  // The problem with that is I'll be looking in the GAC for assemblies
		//  // that won't exist some portion of the time, meaning we have
		//  // to throw and ---- FileNotFoundExceptions.  The debugging experience
		//  // and performance are horrible.

		//  String simpleName = assemblyRef.Substring(0, assemblyRef.IndexOf(','));
		//  if (String.Equals(simpleName, "System.AddIn"))
		//    return SystemAddInInReflectionOnlyContext;
		//  if (String.Equals(simpleName, "System.AddIn.Contract"))
		//    return SystemAddInContractsInReflectionOnlyContext;
		//  String rootDir = Path.GetDirectoryName(Path.GetDirectoryName(_assemblyFileName));
		//  if (_currentComponentType == PipelineComponentType.AddIn)
		//    rootDir = Path.GetDirectoryName(rootDir);

		//  List<string> dirsToLookIn = new List<string>();
		//  switch (_currentComponentType)
		//  {
		//    case PipelineComponentType.HostAdapter:
		//      // Look in contract directory.  For loading the HAV,
		//      // we can't do that at discovery time since we don't know
		//      // which directory contains the HAV.  This is why we're
		//      // writing our own metadata parser in managed code.
		//      // At activation time, the HAV should already be loaded
		//      // within the host's AppDomain.
		//      dirsToLookIn.Add(Path.Combine(rootDir, AddInStore.ContractsDirName));
		//      break;

		//    case PipelineComponentType.Contract:
		//      break;

		//    case PipelineComponentType.AddInAdapter:
		//      // Look in contract directory and addin base directory.
		//      dirsToLookIn.Add(Path.Combine(rootDir, AddInStore.ContractsDirName));
		//      dirsToLookIn.Add(Path.Combine(rootDir, AddInStore.AddInBasesDirName));
		//      break;

		//    case PipelineComponentType.AddInBase:
		//      // look for other assemblies in the same folder
		//      dirsToLookIn.Add(Path.Combine(rootDir, AddInStore.AddInBasesDirName));
		//      // @


		//      break;

		//    /*
		//case PipelineComponentType.AddIn:
		//    // Look in both the add-in's directory and the add-in base's
		//    // directory.  We do the first by setting the app base for the AppDomain,
		//    // but we may not be able to do that if the user created the appdomain.
		//    dirsToLookIn.Add(Path.Combine(rootDir, AddInBasesDirName));
		//    dirsToLookIn.Add(Path.GetDirectoryName(_assemblyFileName));
		//    break;
		//    */

		//    default:
		//      System.Diagnostics.Contracts.Contract.Assert(false, "Fell through switch in the inspection assembly resolve event!");
		//      break;
		//  }

		//  List<string> potentialFileNames = new List<string>(dirsToLookIn.Count * 2);
		//  foreach (String path in dirsToLookIn)
		//  {
		//    String simpleFileName = Path.Combine(path, simpleName);
		//    String dllName = simpleFileName + ".dll";
		//    if (File.Exists(dllName))
		//      potentialFileNames.Add(dllName);
		//    else if (File.Exists(simpleFileName + ".exe"))
		//      potentialFileNames.Add(simpleFileName + ".exe");
		//  }

		//  foreach (String fileName in potentialFileNames)
		//  {
		//    try
		//    {
		//      Assembly a = Assembly.ReflectionOnlyLoadFrom(fileName);
		//      // We should at least be comparing the public key token
		//      // for the two assemblies here.  The version numbers may
		//      // potentially be different, dependent on publisher policy.
		//      if (Utils.AssemblyRefEqualsDef(assemblyRef, a.FullName))
		//        return a;
		//    }
		//    catch (BadImageFormatException)
		//    {
		//    }
		//  }

		//  // Look in the GAC.  It may not be there, so we need to catch
		//  // FileNotFoundException.
		//  // As an optimization, don't probe in the GAC if the public
		//  // key token is null, because by definition the assembly
		//  // can't be in the GAC.
		//  if (!assemblyRef.Contains("PublicKeyToken=null"))
		//  {
		//    try
		//    {
		//      return Assembly.ReflectionOnlyLoad(assemblyRef);
		//    }
		//    catch (FileNotFoundException) { }

		//    try
		//    {
		//      // As a final effort, look in the GAC after appying loader policy.
		//      // This lets us resolves references for assemblies built against an older version of the framework.
		//      return Assembly.ReflectionOnlyLoad(AppDomain.CurrentDomain.ApplyPolicy(assemblyRef));
		//    }
		//    catch (FileNotFoundException) { }
		//  }

		//  //Console.WriteLine("Couldn't resolve assembly {0} while loading a {1}", simpleName, _currentComponentType);
		//  return null;
		//} 

		public ConnectionDialog()
		{
			InitializeComponent();
		}

		public ConnectionDialog(IConnectionInfo cxInfo, bool isNewConnection)
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += MyResolveEventHandler;
			_isNewConnection = isNewConnection;
			CxInfo = cxInfo;
			DbProviderFactoryNames = (new[] {""}).Union(DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>().Select(r => r["InvariantName"])).Cast<string>().ToList();

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
			}
			DataContext = this;
			InitializeComponent();
		}

		/// <summary>
		/// Gets the CustomTypeNameVisibility.
		/// </summary>
		/// <value>The CustomTypeNameVisibility.</value>
		public Visibility CustomTypeNameVisibility
		{
			get { return string.IsNullOrEmpty(txtAssemblyPath.Text) ? Visibility.Collapsed : Visibility.Visible; }
		}

		/// <summary>
		/// Gets the ConnectionTypeVisibility.
		/// </summary>
		/// <value>The ConnectionTypeVisibility.</value>
		public Visibility ConnectionTypeVisibility
		{
			get { return AdapterConnectionTypes.Contains(LLBLConnectionType) ? Visibility.Visible : Visibility.Collapsed; }
		}

		public LLBLConnectionType LLBLConnectionType
		{
			get
			{
				if (string.IsNullOrEmpty(CxInfo.CustomTypeInfo.CustomTypeName))
					return LLBLConnectionType = LLBLConnectionType.Unknown;
				int connectionTypeIndex;
				if (int.TryParse(GetDriverDataValue(ElementNameConnectionType), out connectionTypeIndex))
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
				if (CxInfo.DatabaseInfo.Provider != value || string.Equals(value, "System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
				{
					_providerHasBeenSet = value != "";
					CxInfo.DatabaseInfo.Provider = value;
					OnPropertyChanged("Provider");
				}
			}
		}

		public IEnumerable<string> DbProviderFactoryNames { get; set; }

		public ObservableCollection<ValueTypeWrapper<string>> AdditionalNamespaces { get; set; }

		private ObservableCollection<ValueTypeWrapper<string>> _additionalAssemblies;
		private static readonly LLBLConnectionType[] AdapterConnectionTypes = new[] {LLBLConnectionType.Adapter, LLBLConnectionType.AdapterFactory};

		public ObservableCollection<ValueTypeWrapper<string>> AdditionalAssemblies
		{
			get { return _additionalAssemblies ?? (_additionalAssemblies = new ObservableCollection<ValueTypeWrapper<string>>(Settings.Default.AdditionalAssemblies.CreateStringWrapperForBinding())); }
		}

		private static void CreateDriverDataElements(IConnectionInfo cxInfo)
		{
			CreateElementIfNeeded(cxInfo, ElementNameAdaptertype, Settings.Default.DefaultAdapterType);
			CreateElementIfNeeded(cxInfo, ElementNameAdapterAssembly, Settings.Default.DefaultAdapterAssembly);
			CreateElementIfNeeded(cxInfo, ElementNameFactoryMethod, Settings.Default.DefaultDataAccessAdapterFactoryMethod);
			CreateElementIfNeeded(cxInfo, ElementNameFactoryType, Settings.Default.DefaultDataAccessAdapterFactoryType);
			CreateElementIfNeeded(cxInfo, ElementNameFactoryAssembly, Settings.Default.DefaultDataAccessAdapterFactoryAssembly);
			CreateElementIfNeeded(cxInfo, ElementNameConnectionType, Settings.Default.DefaultConnectionType.ToString());
			CreateElementIfNeeded(cxInfo, ElementNameUseFields, true.ToString());
		}

		public static void UpGradeDriverDataElements(IConnectionInfo cxInfo)
		{
			var adaptertypeName = GetDriverDataValue(cxInfo, ElementNameAdaptertype);
			if (string.IsNullOrEmpty(adaptertypeName))
				CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.SelfServicing).ToString());
			else if (adaptertypeName == cxInfo.DatabaseInfo.DbVersion)
			{
				CreateElementIfNeeded(cxInfo, ElementNameFactoryMethod, cxInfo.DatabaseInfo.Provider);
				CreateElementIfNeeded(cxInfo, ElementNameFactoryType, cxInfo.DatabaseInfo.DbVersion);
				CreateElementIfNeeded(cxInfo, ElementNameFactoryAssembly, cxInfo.CustomTypeInfo.CustomMetadataPath);
				CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.AdapterFactory).ToString());
				cxInfo.DatabaseInfo.Provider = null;
				cxInfo.DatabaseInfo.DbVersion = null;
				cxInfo.DriverData.Element(ElementNameAdaptertype).Value = string.Empty;
			}
			else
			{
				CreateElementIfNeeded(cxInfo, ElementNameAdaptertype, null);
				CreateElementIfNeeded(cxInfo, ElementNameAdapterAssembly, cxInfo.CustomTypeInfo.CustomMetadataPath);
				CreateElementIfNeeded(cxInfo, ElementNameConnectionType, ((int) LLBLConnectionType.Adapter).ToString());
			}
			CreateElementIfNeeded(cxInfo, ElementNameUseFields, true.ToString());
			cxInfo.CustomTypeInfo.CustomMetadataPath = null;
		}

		private static void CreateElementIfNeeded(IConnectionInfo cxInfo, string elementName, string defaultValue)
		{
			if (cxInfo.DriverData.Element(elementName) == null)
			{
				var xElement = new XElement(elementName);
				if (defaultValue != null)
					xElement.Value = defaultValue;
				cxInfo.DriverData.Add(xElement);
			}
		}

		public void SetDriverDataValue(string elementName, string value)
		{
			CreateElementIfNeeded(CxInfo, elementName, null);
// ReSharper disable PossibleNullReferenceException
			CxInfo.DriverData.Element(elementName).Value = value;
// ReSharper restore PossibleNullReferenceException
		}

		public static string GetDriverDataValue(IConnectionInfo cxInfo, string elementName)
		{
			var xElement = cxInfo.DriverData.Element(elementName);
			return xElement != null ? xElement.Value : null;
		}

		public string GetDriverDataValue(string elementName)
		{
			return GetDriverDataValue(CxInfo, elementName);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			this.SetPlacement(Settings.Default.ConnectionDialogPlacement);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			base.OnSourceInitialized(e);
			this.SetPlacement(Settings.Default.ConnectionDialogPlacement);
			var settingsViewSource = ((CollectionViewSource) (FindResource("settingsViewSource")));
			settingsViewSource.Source = new List<Settings> {Settings.Default};
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

			Settings.Default.DefaultAdapterType = GetDriverDataValue(ElementNameAdaptertype);
			Settings.Default.DefaultAdapterAssembly = GetDriverDataValue(ElementNameAdaptertype);
			int connectionTypeIndex;
			if (int.TryParse(GetDriverDataValue(ElementNameConnectionType), out connectionTypeIndex))
				Settings.Default.DefaultConnectionType = connectionTypeIndex;

			Settings.Default.Save();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (DialogResult.GetValueOrDefault() && !string.IsNullOrEmpty(CxInfo.DatabaseInfo.CustomCxString) && string.IsNullOrEmpty(providerComboBox.Text))
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

			try
			{
				DialogResult = true;
			}
			catch (Exception)
			{
				Close();
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
			LLBLConnectionType = IsSelfServicing(CxInfo) ? LLBLConnectionType == LLBLConnectionType.AdapterFactory ? LLBLConnectionType : LLBLConnectionType.Adapter : LLBLConnectionType.SelfServicing;
			OnPropertyChanged("ConnectionTypeVisibility");
		}

		public static bool IsSelfServicing(IConnectionInfo connectionInfo)
		{
			var selfServicingEntities = connectionInfo.CustomTypeInfo.GetCustomTypesInAssembly("SD.LLBLGen.Pro.ORMSupportClasses.EntityBase");
			return selfServicingEntities.IsNullOrEmpty();
		}

		private void ChooseType(object sender, RoutedEventArgs e)
		{
			var customTypes = GetLinqMetaDataTypes();
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
			var assemPath = CxInfo.CustomTypeInfo.CustomAssemblyPath;
			if (assemPath.Length == 0)
			{
				MessageBox.Show("First enter a path to an assembly containing LLBL entities.");
				return null;
			}

			if (!File.Exists(assemPath))
			{
				MessageBox.Show("File '" + assemPath + "' does not exist.");
				return null;
			}

			string[] customTypes;
			try
			{
				customTypes = CxInfo.CustomTypeInfo.GetCustomTypesInAssembly("SD.LLBLGen.Pro.LinqSupportClasses.ILinqMetaData");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error obtaining custom types");
				BreakIntoDebugger();
				return null;
			}
			if (customTypes.Length == 0)
			{
				customTypes = CxInfo.CustomTypeInfo.GetCustomTypesInAssembly("SD.LLBLGen.Pro.ORMSupportClasses.IElementCreatorCore");
				if (customTypes.Length == 0)
					MessageBox.Show("There are no public types in that assembly that implement ILinqMetaData or IElementCreatorCore.", "Wrong Assembly chosen");
				else
				{
					MessageBox.Show("There are no public types in that assembly that implement ILinqMetaData but there is an implementation of IElementCreatorCore.");
				}
				return customTypes;
			}
			return customTypes;
		}

		private void BrowseDataAccessAdapterAssembly(object sender, RoutedEventArgs e)
		{
			var hl = sender as Hyperlink;
			if (hl != null)
			{
				var element = CxInfo.DriverData.Element(hl.TargetName);
				if (element != null)
				{
					var dialog = new OpenFileDialog
					             	{
					             		DefaultExt = ".dll",
					             		FileName = element.Value,
					             		Title = hl.TargetName.Equals(ElementNameFactoryAssembly) ? TitleChooseFactoryAssembly : TitleChooseDataAccessAdapterAssembly,
					             		Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*"
					             	};
					if (File.Exists(element.Value))
						dialog.InitialDirectory = Path.GetDirectoryName(element.Value);
					if (dialog.ShowDialog() == true)
					{
						element.Value = dialog.FileName;
						var dataAccessAdapterAssembly = Assembly.ReflectionOnlyLoadFrom(dialog.FileName);
						try
						{
							var customTypes = GetDataAccessAdapterTypeNames(dataAccessAdapterAssembly);
							if (customTypes.Count() == 1)
								CxInfo.DriverData.SetElementValue(hl.TargetName.Replace("Assembly", "Type"), customTypes.First());
						}
						catch (Exception)
						{
							BreakIntoDebugger();
							return;
						}
					}
				}
			}
		}

		private static IEnumerable<string> GetDataAccessAdapterTypeNames(Assembly dataAccessAdapterAssembly)
		{
			return GetDataAccessAdapterTypeNamesBothWays(dataAccessAdapterAssembly.GetTypes());
		}

		private static IEnumerable<string> GetDataAccessAdapterTypeNamesBothWays(IEnumerable<Type> types)
		{
			return types.GetInterfaceImplementersBothWays(typeof (IDataAccessAdapter)).Select(t => t.FullName);
		}

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

		private void ChooseAdapter(object sender, RoutedEventArgs e)
		{
			var hl = sender as Hyperlink;
			if (hl != null)
			{
				var assemPath = GetDriverDataValue(hl.TargetName.Replace("Type", "Assembly"));
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

				IEnumerable<string> customTypes;
				try
				{
					var dataAccessAdapterAssembly = Assembly.ReflectionOnlyLoadFrom(assemPath);
					var types = dataAccessAdapterAssembly.GetTypes();
					customTypes = LLBLConnectionType == LLBLConnectionType.Adapter ? GetDataAccessAdapterTypeNamesBothWays(types) : types.Select(t => t.FullName).OrderBy(s => s);
				}
				catch (ReflectionTypeLoadException ex)
				{
					customTypes = GetDataAccessAdapterTypeNamesBothWays(ex.Types);
					if (!customTypes.Any())
					{
						MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine +
						                ex.LoaderExceptions.Select(le => le.Message).Distinct().JoinAsString(Environment.NewLine), "Error obtaining adapter types");
						BreakIntoDebugger();
						return;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error obtaining adapter types: " + ex.Message);
					BreakIntoDebugger();
					return;
				}
				if (!customTypes.Any())
				{
					MessageBox.Show("There are no public types in that assembly that implement IDataAccessAdapter.");
					BreakIntoDebugger();
					return;
				}
				if (customTypes.Count() == 1)
					CxInfo.DriverData.SetElementValue(hl.TargetName, customTypes.First());
				else
				{
					var result = (string) Dialogs.PickFromList("Choose " + hl.TargetName, customTypes.ToArray());
					if (result != null)
					{
						CxInfo.DriverData.SetElementValue(hl.TargetName, result);
						ChooseAdapterFactoryMethod(sender, e);
					}
				}
			}
		}

		private void ChooseAdapterFactoryMethod(object sender, RoutedEventArgs e)
		{
			try
			{
				var factoryTypeName = GetDriverDataValue(CxInfo, ElementNameFactoryType);
				var factoryAssemblyPath = GetDriverDataValue(CxInfo, ElementNameFactoryAssembly);
				var factoryAdapterAssembly = Assembly.ReflectionOnlyLoadFrom(factoryAssemblyPath);
				if (factoryAdapterAssembly == null)
					throw new ApplicationException("Factory assembly: " + factoryAssemblyPath + " could not be loaded!");
				var factoryType = factoryAdapterAssembly.GetType(factoryTypeName);
				if (factoryType == null)
				{
					factoryAdapterAssembly.GetTypes();
					throw new ApplicationException(string.Format("Factory type: {0} could not be loaded from: {1}!", factoryTypeName, factoryAssemblyPath));
				}
				var methodInfos = factoryType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				var validMethods = from m in methodInfos
				                   let ps = m.GetParameters()
				                   where ps.Length == 1
				                   where ps.Single().ParameterType == typeof (string) && m.ReturnType.Implements(typeof (IDataAccessAdapter))
				                   select m;
				var count = validMethods.Count();
				if (count == 1)
					CxInfo.DriverData.SetElementValue(ElementNameFactoryMethod, validMethods.Single().Name);
				else
				{
					var result = (MethodInfo) Dialogs.PickFromList(TitleChooseFactoryMethod, validMethods.ToArray());
					if (result != null)
						CxInfo.DriverData.SetElementValue(ElementNameFactoryMethod, result.Name);
					else
					{
						Dialogs.PickFromList("An array of assemblies in this application domain.", AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName).ToArray());
					}
				}
			}
			catch (Exception ex)
			{
				Application.OnThreadException(ex);
			}
		}

		[Conditional("DEBUGX")]
		private static void BreakIntoDebugger()
		{
			Debugger.Break();
		}

		private void ChooseAssemblies(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			             	{
			             		Title = TitleChooseExtraAssembly,
			             		DefaultExt = ".dll",
											Filter = "Assemblies (*.dll)|*.dll|All files (*.*)|*.*",
			             		Multiselect = true
			             	};

			if (dialog.ShowDialog() == true)
				foreach (var fileName in dialog.FileNames
					.Where(fileName => !LLBLGenStaticDriver.AdditionalAssemblies.Contains(Path.GetFileName(fileName))).CreateStringWrapperForBinding()
					.Where(fileName => !AdditionalAssemblies.Contains(fileName))
					)
				{
					AdditionalAssemblies.Add(fileName);
				}
		}

		private void AddQuerySpec(object sender, RoutedEventArgs e)
		{
			ValueTypeWrapper<string>.Add(AdditionalAssemblies, "SD.LLBLGen.Pro.QuerySpec.dll");
			ValueTypeWrapper<string>.Add(AdditionalNamespaces, "SD.LLBLGen.Pro.QuerySpec", "SD.LLBLGen.Pro.QuerySpec.SelfServicing", "SD.LLBLGen.Pro.QuerySpec.Adapter");
		}

		private void DataBaseConnectionDialog(object sender, RoutedEventArgs e)
		{
			var dcd = new DataConnectionDialog();
			var dcs = new DataConnectionConfiguration(null);
			dcs.LoadConfiguration(dcd);

			var dataProvider = dcd.UnspecifiedDataSource.Providers.FirstOrDefault(p => p.Name == Provider);
			if (dataProvider != null)
			{
				dcd.SelectedDataSource = dcd.UnspecifiedDataSource;
				dcd.SelectedDataProvider = dataProvider;
			}

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

		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

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
			if (string.IsNullOrEmpty(providerComboBox.Text))
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
	}

	public enum LLBLConnectionType
	{
		Unknown,
		SelfServicing,
		Adapter,
		[Description("Adapter Factory")] AdapterFactory
	}
}