﻿///////////////////////////////////////////////////////////////
// This is generated code. 
//////////////////////////////////////////////////////////////
// Code is generated using LLBLGen Pro version: 4.2
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
//////////////////////////////////////////////////////////////
using System;
using System.ComponentModel;
using Northwind.DAL.DTO;	

namespace Northwind.DAL.TypedViewClasses
{
	/// <summary>Class which represents a row in the typed view 'SummaryOfSalesByYear'.</summary>
	/// <remarks>This class is a result class for a query, which is produced with the method <see cref="Northwind.DAL.Linq.LinqMetaData.GetSummaryOfSalesByYearTypedView"/>.
	/// Custom Properties: <br/>
	/// </remarks>
	[Serializable]
	public partial class SummaryOfSalesByYearRow : PocoBase
	{
		#region Extensibility Method Definitions
		partial void OnCreated();
		#endregion
		
		/// <summary>Initializes a new instance of the <see cref="SummaryOfSalesByYearRow"/> class.</summary>
		public SummaryOfSalesByYearRow()
		{
			OnCreated();
		}

		#region Class Property Declarations
		/// <summary>Gets or sets the OrderId field.</summary>
		public System.Int32 OrderId { get; set; }
		/// <summary>Gets or sets the ShippedDate field.</summary>
		public Nullable<System.DateTime> ShippedDate { get; set; }
		/// <summary>Gets or sets the Subtotal field.</summary>
		public Nullable<System.Decimal> Subtotal { get; set; }
		#endregion
	}
}

