﻿///////////////////////////////////////////////////////////////
// This is generated code. 
//////////////////////////////////////////////////////////////
// Code is generated using LLBLGen Pro version: 4.2
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
// Templates version: 
//////////////////////////////////////////////////////////////
using System;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Runtime.Serialization;
using AW.Data.HelperClasses;
using AW.Data.DaoClasses;
using AW.Data.EntityClasses;
using AW.Data.FactoryClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;


namespace AW.Data.TypedListClasses
{
	// __LLBLGENPRO_USER_CODE_REGION_START AdditionalNamespaces
	// __LLBLGENPRO_USER_CODE_REGION_END
	/// <summary>Typed datatable for the list 'CustomerList'.<br/><br/></summary>
	[Serializable, System.ComponentModel.DesignerCategory("Code")]
	[ToolboxItem(true)]
	[DesignTimeVisible(true)]
	public partial class CustomerListTypedList : TypedListBase<CustomerListRow>
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfacesList
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		#region Class Member Declarations
		private DataColumn _columnAddressLine1;
		private DataColumn _columnAddressLine2;
		private DataColumn _columnAddressType;
		private DataColumn _columnCity;
		private DataColumn _columnCountryRegionName;
		private DataColumn _columnCustomerId;
		private DataColumn _columnDemographics;
		private DataColumn _columnEmailAddress;
		private DataColumn _columnEmailPromotion;
		private DataColumn _columnFirstName;
		private DataColumn _columnLastName;
		private DataColumn _columnMiddleName;
		private DataColumn _columnPhone;
		private DataColumn _columnPostalCode;
		private DataColumn _columnStateProvinceName;
		private DataColumn _columnSuffix;
		private DataColumn _columnTitle;
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static Hashtable	_customProperties;
		private static Hashtable	_fieldsCustomProperties;
		#endregion

		#region Class Constants
		private const int AmountOfFields = 17;
		#endregion

		/// <summary>Static CTor for setting up custom property hashtables.</summary>
		static CustomerListTypedList()
		{
			SetupCustomPropertyHashtables();
		}
		
		/// <summary>CTor</summary>
		public CustomerListTypedList():base("CustomerList")
		{
			InitClass(false);
		}
		
		/// <summary>CTor</summary>
		/// <param name="obeyWeakRelations">The flag to signal the typed list what kind of join statements to generate in the query statement. Weak relationships are relationships which are optional, for example a
		/// customer with no orders is possible, because the relationship between customer and order is based on a field in order. When this property is set to true (default: false), weak relationships will result in LEFT JOIN statements. When
		/// set to false (which is the default), INNER JOIN statements are used.</param>
		public CustomerListTypedList(bool obeyWeakRelations):base("CustomerList")
		{
			InitClass(obeyWeakRelations);
		}

		/// <summary>Protected constructor for deserialization.</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CustomerListTypedList(SerializationInfo info, StreamingContext context):base(info, context)
		{
			InitMembers();
		}

		/// <summary>Clones this instance.</summary>
		/// <returns>A clone of this instance</returns>
		public override DataTable Clone() 
		{
			CustomerListTypedList cloneToReturn = ((CustomerListTypedList)(base.Clone()));
			cloneToReturn.InitMembers();
			return cloneToReturn;
		}
				
		/// <summary>Gets the QuerySpec query which fetches this typed list.</summary>
		/// <param name="qf">The queryfactory.</param>
		/// <returns>ready to use DynamicQuery instance to be used with FetchAsDataTable in the QuerySpec API</returns>
		public DynamicQuery GetQuerySpecQuery(QueryFactory qf)
		{
			return qf.Create().Select(this.BuildResultset().GetAsEntityFieldCoreArray()).From(this.BuildRelationSet());
		}

		/// <summary>Creates a new TypedList dao instance</summary>
		protected override IDao CreateDAOInstance()
		{
			return DAOFactory.CreateTypedListDAO();
		}

		/// <summary>Creates a new typed row during the build of the datatable during a Fill session by a dataadapter.</summary>
		/// <param name="rowBuilder">supplied row builder to pass to the typed row</param>
		/// <returns>the new typed datarow</returns>
		protected override DataRow NewRowFromBuilder(DataRowBuilder rowBuilder) 
		{
			return new CustomerListRow(rowBuilder);
		}

		/// <summary>Builds the relation set for this typed list.</summary>
		/// <returns>ready to use relation set</returns>
		protected override IRelationCollection BuildRelationSet()
		{
			IRelationCollection toReturn = new RelationCollection();
			toReturn.ObeyWeakRelations = this.ObeyWeakRelations;
			toReturn.Add(StateProvinceEntity.Relations.AddressEntityUsingStateProvinceID, "", "", JoinHint.Inner);
			toReturn.Add(CustomerAddressEntity.Relations.AddressEntityUsingAddressID, "", "", JoinHint.Inner);
			toReturn.Add(CountryRegionEntity.Relations.StateProvinceEntityUsingCountryRegionCode, "", "", JoinHint.Inner);
			toReturn.Add(AddressTypeEntity.Relations.CustomerAddressEntityUsingAddressTypeID, "", "", JoinHint.Inner);
			toReturn.Add(CustomerEntity.Relations.CustomerAddressEntityUsingCustomerID, "", "", JoinHint.Inner);
			toReturn.Add(IndividualEntity.Relations.CustomerAddressEntityUsingCustomerID, "", "", JoinHint.Inner);
			toReturn.Add(ContactEntity.Relations.IndividualEntityUsingContactID, "", "", JoinHint.Inner);
			// __LLBLGENPRO_USER_CODE_REGION_START AdditionalRelations
			// __LLBLGENPRO_USER_CODE_REGION_END
			OnRelationSetBuilt(toReturn);
			return toReturn;
		}

		/// <summary>Builds the resultset fields.</summary>
		/// <returns>ready to use resultset</returns>
		protected override IEntityFields BuildResultset()
		{
			ResultsetFields toReturn = new ResultsetFields(AmountOfFields);
			toReturn.DefineField(AddressFields.AddressLine1, 0, "AddressLine1", "", AggregateFunction.None);
			toReturn.DefineField(AddressFields.AddressLine2, 1, "AddressLine2", "", AggregateFunction.None);
			toReturn.DefineField(AddressTypeFields.Name, 2, "AddressType", "", AggregateFunction.None);
			toReturn.DefineField(AddressFields.City, 3, "City", "", AggregateFunction.None);
			toReturn.DefineField(StateProvinceFields.Name, 4, "CountryRegionName", "", AggregateFunction.None);
			toReturn.DefineField(IndividualFields.CustomerID, 5, "CustomerId", "", AggregateFunction.None);
			toReturn.DefineField(IndividualFields.Demographics, 6, "Demographics", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.EmailAddress, 7, "EmailAddress", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.EmailPromotion, 8, "EmailPromotion", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.FirstName, 9, "FirstName", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.LastName, 10, "LastName", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.MiddleName, 11, "MiddleName", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.Phone, 12, "Phone", "", AggregateFunction.None);
			toReturn.DefineField(AddressFields.PostalCode, 13, "PostalCode", "", AggregateFunction.None);
			toReturn.DefineField(CountryRegionFields.Name, 14, "StateProvinceName", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.Suffix, 15, "Suffix", "", AggregateFunction.None);
			toReturn.DefineField(ContactFields.Title, 16, "Title", "", AggregateFunction.None);
			// __LLBLGENPRO_USER_CODE_REGION_START AdditionalFields
			// be sure to call toReturn.Expand(number of new fields) first. 
			// __LLBLGENPRO_USER_CODE_REGION_END
			OnResultsetBuilt(toReturn);
			return toReturn;
		}

		/// <summary>Initializes the hashtables for the typed list type and typed list field custom properties. </summary>
		private static void SetupCustomPropertyHashtables()
		{
			_customProperties = new Hashtable();
			_fieldsCustomProperties = new Hashtable();
			Hashtable fieldHashtable = null;
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("AddressLine1", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("AddressLine2", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("AddressType", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("City", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("CountryRegionName", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("CustomerId", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("Demographics", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("EmailAddress", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("EmailPromotion", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("FirstName", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("LastName", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("MiddleName", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("Phone", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("PostalCode", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("StateProvinceName", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("Suffix", fieldHashtable);
			fieldHashtable = new Hashtable();
			_fieldsCustomProperties.Add("Title", fieldHashtable);
		}

		/// <summary>Initialize the datastructures.</summary>
		/// <param name="obeyWeakRelations">flag for the internal used relations object</param>
		protected override void InitClass(bool obeyWeakRelations)
		{
			_columnAddressLine1 = GeneralUtils.CreateTypedDataTableColumn("AddressLine1", @"AddressLine1", typeof(System.String), this.Columns);
			_columnAddressLine2 = GeneralUtils.CreateTypedDataTableColumn("AddressLine2", @"AddressLine2", typeof(System.String), this.Columns);
			_columnAddressType = GeneralUtils.CreateTypedDataTableColumn("AddressType", @"Name", typeof(System.String), this.Columns);
			_columnCity = GeneralUtils.CreateTypedDataTableColumn("City", @"City", typeof(System.String), this.Columns);
			_columnCountryRegionName = GeneralUtils.CreateTypedDataTableColumn("CountryRegionName", @"CountryRegionName", typeof(System.String), this.Columns);
			_columnCustomerId = GeneralUtils.CreateTypedDataTableColumn("CustomerId", @"CustomerID", typeof(System.Int32), this.Columns);
			_columnDemographics = GeneralUtils.CreateTypedDataTableColumn("Demographics", @"Demographics", typeof(System.String), this.Columns);
			_columnEmailAddress = GeneralUtils.CreateTypedDataTableColumn("EmailAddress", @"EmailAddress", typeof(System.String), this.Columns);
			_columnEmailPromotion = GeneralUtils.CreateTypedDataTableColumn("EmailPromotion", @"EmailPromotion", typeof(AW.Data.EmailPromotion), this.Columns);
			_columnFirstName = GeneralUtils.CreateTypedDataTableColumn("FirstName", @"FirstName", typeof(System.String), this.Columns);
			_columnLastName = GeneralUtils.CreateTypedDataTableColumn("LastName", @"LastName", typeof(System.String), this.Columns);
			_columnMiddleName = GeneralUtils.CreateTypedDataTableColumn("MiddleName", @"MiddleName", typeof(System.String), this.Columns);
			_columnPhone = GeneralUtils.CreateTypedDataTableColumn("Phone", @"Phone", typeof(System.String), this.Columns);
			_columnPostalCode = GeneralUtils.CreateTypedDataTableColumn("PostalCode", @"PostalCode", typeof(System.String), this.Columns);
			_columnStateProvinceName = GeneralUtils.CreateTypedDataTableColumn("StateProvinceName", @"StateProvinceName", typeof(System.String), this.Columns);
			_columnSuffix = GeneralUtils.CreateTypedDataTableColumn("Suffix", @"Suffix", typeof(System.String), this.Columns);
			_columnTitle = GeneralUtils.CreateTypedDataTableColumn("Title", @"Title", typeof(System.String), this.Columns);

			// __LLBLGENPRO_USER_CODE_REGION_START InitClass
			// __LLBLGENPRO_USER_CODE_REGION_END
			this.ObeyWeakRelations = obeyWeakRelations;
			OnInitialized();
		}

		/// <summary>Initializes the members, after a clone action.</summary>
		private void InitMembers()
		{
			_columnAddressLine1 = this.Columns["AddressLine1"];
			_columnAddressLine2 = this.Columns["AddressLine2"];
			_columnAddressType = this.Columns["AddressType"];
			_columnCity = this.Columns["City"];
			_columnCountryRegionName = this.Columns["CountryRegionName"];
			_columnCustomerId = this.Columns["CustomerId"];
			_columnDemographics = this.Columns["Demographics"];
			_columnEmailAddress = this.Columns["EmailAddress"];
			_columnEmailPromotion = this.Columns["EmailPromotion"];
			_columnFirstName = this.Columns["FirstName"];
			_columnLastName = this.Columns["LastName"];
			_columnMiddleName = this.Columns["MiddleName"];
			_columnPhone = this.Columns["Phone"];
			_columnPostalCode = this.Columns["PostalCode"];
			_columnStateProvinceName = this.Columns["StateProvinceName"];
			_columnSuffix = this.Columns["Suffix"];
			_columnTitle = this.Columns["Title"];

			// __LLBLGENPRO_USER_CODE_REGION_START InitMembers
			// __LLBLGENPRO_USER_CODE_REGION_END
			OnInitialized();
		}

		/// <summary>Creates a new instance of the DataTable class.</summary>
		/// <returns>a new instance of a datatable with this schema.</returns>
		protected override DataTable CreateInstance() 
		{
			return new CustomerListTypedList();
		}

		#region Class Property Declarations
		/// <summary>The custom properties for this TypedList type.</summary>
		/// <remarks>The data returned from this property should be considered read-only: it is not thread safe to alter this data at runtime.</remarks>
		public static Hashtable CustomProperties
		{
			get { return _customProperties;}
		}

		/// <summary>The custom properties for the type of this TypedList instance.</summary>
		/// <remarks>The data returned from this property should be considered read-only: it is not thread safe to alter this data at runtime.</remarks>
		[Browsable(false)]
		public virtual Hashtable CustomPropertiesOfType
		{
			get { return CustomerListTypedList.CustomProperties;}
		}

		/// <summary>The custom properties for the fields of this TypedList type. The returned Hashtable contains per fieldname a hashtable of name-value pairs. </summary>
		/// <remarks>The data returned from this property should be considered read-only: it is not thread safe to alter this data at runtime.</remarks>
		public static Hashtable FieldsCustomProperties
		{
			get { return _fieldsCustomProperties;}
		}

		/// <summary>The custom properties for the fields of the type of this TypedList instance. The returned Hashtable contains per fieldname a hashtable of name-value pairs. </summary>
		/// <remarks>The data returned from this property should be considered read-only: it is not thread safe to alter this data at runtime.</remarks>
		[Browsable(false)]
		public virtual Hashtable FieldsCustomPropertiesOfType
		{
			get { return CustomerListTypedList.FieldsCustomProperties;}
		}

		/// <summary>Returns the column object belonging to the TypedList field AddressLine1</summary>
		internal DataColumn AddressLine1Column 
		{
			get { return _columnAddressLine1; }
		}

		/// <summary>Returns the column object belonging to the TypedList field AddressLine2</summary>
		internal DataColumn AddressLine2Column 
		{
			get { return _columnAddressLine2; }
		}

		/// <summary>Returns the column object belonging to the TypedList field AddressType</summary>
		internal DataColumn AddressTypeColumn 
		{
			get { return _columnAddressType; }
		}

		/// <summary>Returns the column object belonging to the TypedList field City</summary>
		internal DataColumn CityColumn 
		{
			get { return _columnCity; }
		}

		/// <summary>Returns the column object belonging to the TypedList field CountryRegionName</summary>
		internal DataColumn CountryRegionNameColumn 
		{
			get { return _columnCountryRegionName; }
		}

		/// <summary>Returns the column object belonging to the TypedList field CustomerId</summary>
		internal DataColumn CustomerIdColumn 
		{
			get { return _columnCustomerId; }
		}

		/// <summary>Returns the column object belonging to the TypedList field Demographics</summary>
		internal DataColumn DemographicsColumn 
		{
			get { return _columnDemographics; }
		}

		/// <summary>Returns the column object belonging to the TypedList field EmailAddress</summary>
		internal DataColumn EmailAddressColumn 
		{
			get { return _columnEmailAddress; }
		}

		/// <summary>Returns the column object belonging to the TypedList field EmailPromotion</summary>
		internal DataColumn EmailPromotionColumn 
		{
			get { return _columnEmailPromotion; }
		}

		/// <summary>Returns the column object belonging to the TypedList field FirstName</summary>
		internal DataColumn FirstNameColumn 
		{
			get { return _columnFirstName; }
		}

		/// <summary>Returns the column object belonging to the TypedList field LastName</summary>
		internal DataColumn LastNameColumn 
		{
			get { return _columnLastName; }
		}

		/// <summary>Returns the column object belonging to the TypedList field MiddleName</summary>
		internal DataColumn MiddleNameColumn 
		{
			get { return _columnMiddleName; }
		}

		/// <summary>Returns the column object belonging to the TypedList field Phone</summary>
		internal DataColumn PhoneColumn 
		{
			get { return _columnPhone; }
		}

		/// <summary>Returns the column object belonging to the TypedList field PostalCode</summary>
		internal DataColumn PostalCodeColumn 
		{
			get { return _columnPostalCode; }
		}

		/// <summary>Returns the column object belonging to the TypedList field StateProvinceName</summary>
		internal DataColumn StateProvinceNameColumn 
		{
			get { return _columnStateProvinceName; }
		}

		/// <summary>Returns the column object belonging to the TypedList field Suffix</summary>
		internal DataColumn SuffixColumn 
		{
			get { return _columnSuffix; }
		}

		/// <summary>Returns the column object belonging to the TypedList field Title</summary>
		internal DataColumn TitleColumn 
		{
			get { return _columnTitle; }
		}

		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalColumnProperties
		// __LLBLGENPRO_USER_CODE_REGION_END
 		#endregion
		
		#region Custom TypedList code
		
		// __LLBLGENPRO_USER_CODE_REGION_START CustomTypedListCode
		// __LLBLGENPRO_USER_CODE_REGION_END
		#endregion

		#region Included Code

		#endregion
	}

	/// <summary>Typed datarow for the typed datatable CustomerList</summary>
	public partial class CustomerListRow : DataRow
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfacesRow
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		#region Class Member Declarations
		private CustomerListTypedList	_parent;
		#endregion

		/// <summary>CTor</summary>
		/// <param name="rowBuilder">Row builder object to use when building this row</param>
		protected internal CustomerListRow(DataRowBuilder rowBuilder) : base(rowBuilder) 
		{
			_parent = ((CustomerListTypedList)(this.Table));
		}

		#region Class Property Declarations
		/// <summary>Gets / sets the value of the TypedList field AddressLine1<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Address.AddressLine1</remarks>
		public System.String AddressLine1 
		{
			get { return IsAddressLine1Null() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.AddressLine1Column]; }
			set { this[_parent.AddressLine1Column] = value; }
		}

		/// <summary>Returns true if the TypedList field AddressLine1 is NULL, false otherwise.</summary>
		public bool IsAddressLine1Null() 
		{
			return IsNull(_parent.AddressLine1Column);
		}

		/// <summary>Sets the TypedList field AddressLine1 to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetAddressLine1Null() 
		{
			this[_parent.AddressLine1Column] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field AddressLine2<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Address.AddressLine2</remarks>
		public System.String AddressLine2 
		{
			get { return IsAddressLine2Null() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.AddressLine2Column]; }
			set { this[_parent.AddressLine2Column] = value; }
		}

		/// <summary>Returns true if the TypedList field AddressLine2 is NULL, false otherwise.</summary>
		public bool IsAddressLine2Null() 
		{
			return IsNull(_parent.AddressLine2Column);
		}

		/// <summary>Sets the TypedList field AddressLine2 to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetAddressLine2Null() 
		{
			this[_parent.AddressLine2Column] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field AddressType<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: AddressType.Name</remarks>
		public System.String AddressType 
		{
			get { return IsAddressTypeNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.AddressTypeColumn]; }
			set { this[_parent.AddressTypeColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field AddressType is NULL, false otherwise.</summary>
		public bool IsAddressTypeNull() 
		{
			return IsNull(_parent.AddressTypeColumn);
		}

		/// <summary>Sets the TypedList field AddressType to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetAddressTypeNull() 
		{
			this[_parent.AddressTypeColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field City<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Address.City</remarks>
		public System.String City 
		{
			get { return IsCityNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.CityColumn]; }
			set { this[_parent.CityColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field City is NULL, false otherwise.</summary>
		public bool IsCityNull() 
		{
			return IsNull(_parent.CityColumn);
		}

		/// <summary>Sets the TypedList field City to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetCityNull() 
		{
			this[_parent.CityColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field CountryRegionName<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: StateProvince.Name</remarks>
		public System.String CountryRegionName 
		{
			get { return IsCountryRegionNameNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.CountryRegionNameColumn]; }
			set { this[_parent.CountryRegionNameColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field CountryRegionName is NULL, false otherwise.</summary>
		public bool IsCountryRegionNameNull() 
		{
			return IsNull(_parent.CountryRegionNameColumn);
		}

		/// <summary>Sets the TypedList field CountryRegionName to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetCountryRegionNameNull() 
		{
			this[_parent.CountryRegionNameColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field CustomerId<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Customer.CustomerID</remarks>
		public System.Int32 CustomerId 
		{
			get { return IsCustomerIdNull() ? (System.Int32)TypeDefaultValue.GetDefaultValue(typeof(System.Int32)) : (System.Int32)this[_parent.CustomerIdColumn]; }
			set { this[_parent.CustomerIdColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field CustomerId is NULL, false otherwise.</summary>
		public bool IsCustomerIdNull() 
		{
			return IsNull(_parent.CustomerIdColumn);
		}

		/// <summary>Sets the TypedList field CustomerId to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetCustomerIdNull() 
		{
			this[_parent.CustomerIdColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field Demographics<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Individual.Demographics</remarks>
		public System.String Demographics 
		{
			get { return IsDemographicsNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.DemographicsColumn]; }
			set { this[_parent.DemographicsColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field Demographics is NULL, false otherwise.</summary>
		public bool IsDemographicsNull() 
		{
			return IsNull(_parent.DemographicsColumn);
		}

		/// <summary>Sets the TypedList field Demographics to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetDemographicsNull() 
		{
			this[_parent.DemographicsColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field EmailAddress<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.EmailAddress</remarks>
		public System.String EmailAddress 
		{
			get { return IsEmailAddressNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.EmailAddressColumn]; }
			set { this[_parent.EmailAddressColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field EmailAddress is NULL, false otherwise.</summary>
		public bool IsEmailAddressNull() 
		{
			return IsNull(_parent.EmailAddressColumn);
		}

		/// <summary>Sets the TypedList field EmailAddress to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetEmailAddressNull() 
		{
			this[_parent.EmailAddressColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field EmailPromotion<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.EmailPromotion</remarks>
		public AW.Data.EmailPromotion EmailPromotion 
		{
			get { return IsEmailPromotionNull() ? (AW.Data.EmailPromotion)TypeDefaultValue.GetDefaultValue(typeof(AW.Data.EmailPromotion)) : (AW.Data.EmailPromotion)this[_parent.EmailPromotionColumn]; }
			set { this[_parent.EmailPromotionColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field EmailPromotion is NULL, false otherwise.</summary>
		public bool IsEmailPromotionNull() 
		{
			return IsNull(_parent.EmailPromotionColumn);
		}

		/// <summary>Sets the TypedList field EmailPromotion to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetEmailPromotionNull() 
		{
			this[_parent.EmailPromotionColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field FirstName<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.FirstName</remarks>
		public System.String FirstName 
		{
			get { return IsFirstNameNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.FirstNameColumn]; }
			set { this[_parent.FirstNameColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field FirstName is NULL, false otherwise.</summary>
		public bool IsFirstNameNull() 
		{
			return IsNull(_parent.FirstNameColumn);
		}

		/// <summary>Sets the TypedList field FirstName to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetFirstNameNull() 
		{
			this[_parent.FirstNameColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field LastName<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.LastName</remarks>
		public System.String LastName 
		{
			get { return IsLastNameNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.LastNameColumn]; }
			set { this[_parent.LastNameColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field LastName is NULL, false otherwise.</summary>
		public bool IsLastNameNull() 
		{
			return IsNull(_parent.LastNameColumn);
		}

		/// <summary>Sets the TypedList field LastName to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetLastNameNull() 
		{
			this[_parent.LastNameColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field MiddleName<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.MiddleName</remarks>
		public System.String MiddleName 
		{
			get { return IsMiddleNameNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.MiddleNameColumn]; }
			set { this[_parent.MiddleNameColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field MiddleName is NULL, false otherwise.</summary>
		public bool IsMiddleNameNull() 
		{
			return IsNull(_parent.MiddleNameColumn);
		}

		/// <summary>Sets the TypedList field MiddleName to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetMiddleNameNull() 
		{
			this[_parent.MiddleNameColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field Phone<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.Phone</remarks>
		public System.String Phone 
		{
			get { return IsPhoneNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.PhoneColumn]; }
			set { this[_parent.PhoneColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field Phone is NULL, false otherwise.</summary>
		public bool IsPhoneNull() 
		{
			return IsNull(_parent.PhoneColumn);
		}

		/// <summary>Sets the TypedList field Phone to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetPhoneNull() 
		{
			this[_parent.PhoneColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field PostalCode<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Address.PostalCode</remarks>
		public System.String PostalCode 
		{
			get { return IsPostalCodeNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.PostalCodeColumn]; }
			set { this[_parent.PostalCodeColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field PostalCode is NULL, false otherwise.</summary>
		public bool IsPostalCodeNull() 
		{
			return IsNull(_parent.PostalCodeColumn);
		}

		/// <summary>Sets the TypedList field PostalCode to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetPostalCodeNull() 
		{
			this[_parent.PostalCodeColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field StateProvinceName<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: CountryRegion.Name</remarks>
		public System.String StateProvinceName 
		{
			get { return IsStateProvinceNameNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.StateProvinceNameColumn]; }
			set { this[_parent.StateProvinceNameColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field StateProvinceName is NULL, false otherwise.</summary>
		public bool IsStateProvinceNameNull() 
		{
			return IsNull(_parent.StateProvinceNameColumn);
		}

		/// <summary>Sets the TypedList field StateProvinceName to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetStateProvinceNameNull() 
		{
			this[_parent.StateProvinceNameColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field Suffix<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.Suffix</remarks>
		public System.String Suffix 
		{
			get { return IsSuffixNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.SuffixColumn]; }
			set { this[_parent.SuffixColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field Suffix is NULL, false otherwise.</summary>
		public bool IsSuffixNull() 
		{
			return IsNull(_parent.SuffixColumn);
		}

		/// <summary>Sets the TypedList field Suffix to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetSuffixNull() 
		{
			this[_parent.SuffixColumn] = System.Convert.DBNull;
		}
		/// <summary>Gets / sets the value of the TypedList field Title<br/><br/>
		/// </summary>
		/// <remarks>Mapped on: Contact.Title</remarks>
		public System.String Title 
		{
			get { return IsTitleNull() ? (System.String)TypeDefaultValue.GetDefaultValue(typeof(System.String)) : (System.String)this[_parent.TitleColumn]; }
			set { this[_parent.TitleColumn] = value; }
		}

		/// <summary>Returns true if the TypedList field Title is NULL, false otherwise.</summary>
		public bool IsTitleNull() 
		{
			return IsNull(_parent.TitleColumn);
		}

		/// <summary>Sets the TypedList field Title to NULL. Not recommended; a typed list should be used as a readonly object.</summary>
    	public void SetTitleNull() 
		{
			this[_parent.TitleColumn] = System.Convert.DBNull;
		}
		#endregion

		#region Custom Typed List Row Code
		
		// __LLBLGENPRO_USER_CODE_REGION_START CustomTypedListRowCode
		// __LLBLGENPRO_USER_CODE_REGION_END
		#endregion
		
		#region Included Row Code

		#endregion	
	}
}
