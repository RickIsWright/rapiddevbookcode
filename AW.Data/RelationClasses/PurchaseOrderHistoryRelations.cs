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
using System.Collections;
using System.Collections.Generic;
using AW.Data;
using AW.Data.FactoryClasses;
using AW.Data.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace AW.Data.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: PurchaseOrderHistory. </summary>
	public partial class PurchaseOrderHistoryRelations : TransactionHistoryRelations
	{
		/// <summary>CTor</summary>
		public PurchaseOrderHistoryRelations()
		{
		}

		/// <summary>Gets all relations of the PurchaseOrderHistoryEntity as a list of IEntityRelation objects.</summary>
		/// <returns>a list of IEntityRelation objects</returns>
		public override List<IEntityRelation> GetAllRelations()
		{
			List<IEntityRelation> toReturn = base.GetAllRelations();
			toReturn.Add(this.PurchaseOrderDetailEntityUsingReferenceOrderLineIDReferenceOrderID);
			toReturn.Add(this.PurchaseOrderHeaderEntityUsingReferenceOrderID);
			return toReturn;
		}

		#region Class Property Declarations


		/// <summary>Returns a new IEntityRelation object, between PurchaseOrderHistoryEntity and PurchaseOrderDetailEntity over the 1:1 relation they have, using the relation between the fields:
		/// PurchaseOrderHistory.ReferenceOrderLineID - PurchaseOrderDetail.PurchaseOrderDetailID
		/// PurchaseOrderHistory.ReferenceOrderID - PurchaseOrderDetail.PurchaseOrderID
		/// </summary>
		public virtual IEntityRelation PurchaseOrderDetailEntityUsingReferenceOrderLineIDReferenceOrderID
		{
			get
			{
				IEntityRelation relation = new EntityRelation(SD.LLBLGen.Pro.ORMSupportClasses.RelationType.OneToOne, "PurchaseOrderDetail", false);




				relation.AddEntityFieldPair(PurchaseOrderDetailFields.PurchaseOrderDetailID, PurchaseOrderHistoryFields.ReferenceOrderLineID);



				relation.AddEntityFieldPair(PurchaseOrderDetailFields.PurchaseOrderID, PurchaseOrderHistoryFields.ReferenceOrderID);
				relation.InheritanceInfoPkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("PurchaseOrderDetailEntity", false);
				relation.InheritanceInfoFkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("PurchaseOrderHistoryEntity", true);
				return relation;
			}
		}

		/// <summary>Returns a new IEntityRelation object, between PurchaseOrderHistoryEntity and ProductEntity over the m:1 relation they have, using the relation between the fields:
		/// PurchaseOrderHistory.ProductID - Product.ProductID
		/// </summary>
		public override IEntityRelation ProductEntityUsingProductID
		{
			get
			{
				IEntityRelation relation = new EntityRelation(SD.LLBLGen.Pro.ORMSupportClasses.RelationType.ManyToOne, "Product", false);
				relation.AddEntityFieldPair(ProductFields.ProductID, PurchaseOrderHistoryFields.ProductID);
				relation.InheritanceInfoPkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("ProductEntity", false);
				relation.InheritanceInfoFkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("PurchaseOrderHistoryEntity", true);
				return relation;
			}
		}
		/// <summary>Returns a new IEntityRelation object, between PurchaseOrderHistoryEntity and PurchaseOrderHeaderEntity over the m:1 relation they have, using the relation between the fields:
		/// PurchaseOrderHistory.ReferenceOrderID - PurchaseOrderHeader.PurchaseOrderID
		/// </summary>
		public virtual IEntityRelation PurchaseOrderHeaderEntityUsingReferenceOrderID
		{
			get
			{
				IEntityRelation relation = new EntityRelation(SD.LLBLGen.Pro.ORMSupportClasses.RelationType.ManyToOne, "PurchaseOrder", false);
				relation.AddEntityFieldPair(PurchaseOrderHeaderFields.PurchaseOrderID, PurchaseOrderHistoryFields.ReferenceOrderID);
				relation.InheritanceInfoPkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("PurchaseOrderHeaderEntity", false);
				relation.InheritanceInfoFkSideEntity = InheritanceInfoProviderSingleton.GetInstance().GetInheritanceInfo("PurchaseOrderHistoryEntity", true);
				return relation;
			}
		}
		/// <summary>stub, not used in this entity, only for TargetPerEntity entities.</summary>
		public override IEntityRelation GetSubTypeRelation(string subTypeEntityName) { return null; }
		/// <summary>stub, not used in this entity, only for TargetPerEntity entities.</summary>
		public override IEntityRelation GetSuperTypeRelation() { return null;}
		#endregion

		#region Included Code

		#endregion
	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticPurchaseOrderHistoryRelations
	{
		internal static readonly IEntityRelation PurchaseOrderDetailEntityUsingReferenceOrderLineIDReferenceOrderIDStatic = new PurchaseOrderHistoryRelations().PurchaseOrderDetailEntityUsingReferenceOrderLineIDReferenceOrderID;
		internal static readonly IEntityRelation ProductEntityUsingProductIDStatic = new PurchaseOrderHistoryRelations().ProductEntityUsingProductID;
		internal static readonly IEntityRelation PurchaseOrderHeaderEntityUsingReferenceOrderIDStatic = new PurchaseOrderHistoryRelations().PurchaseOrderHeaderEntityUsingReferenceOrderID;

		/// <summary>CTor</summary>
		static StaticPurchaseOrderHistoryRelations()
		{
		}
	}
}
