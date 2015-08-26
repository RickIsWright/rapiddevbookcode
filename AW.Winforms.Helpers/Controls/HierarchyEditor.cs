﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using AW.Helper;

namespace AW.Winforms.Helpers.Controls
{
  public partial class HierarchyEditor : UserControl, ISupportInitialize
  {
// ReSharper disable once MemberCanBePrivate.Global
    public HierarchyEditor()
    {
      InitializeComponent();
    }

    private HierarchyEditor(IEnumerable hierarchicalData, string nameColumn)
      : this()
    {
      if (hierarchicalData == null) throw new ArgumentNullException("hierarchicalData");
      bindingSourceHierarchicalData.BindEnumerable(hierarchicalData, false);
      bindingNavigatorDeleteItem.Enabled = bindingSourceHierarchicalData.AllowRemove;
      dataTreeView.NameColumn = nameColumn;
      dataTreeView.Sorted = true;
    }

    private HierarchyEditor(IEnumerable hierarchicalData, string iDPropertyName, string parentIDPropertyName, string nameColumn)
      : this(hierarchicalData, nameColumn)
    {
      dataTreeView.IDColumn = iDPropertyName;
      dataTreeView.ParentIDColumn = parentIDPropertyName;
    }

    private HierarchyEditor(IEnumerable hierarchicalData, string nameColumn, string childCollectionPropertyName)
      : this(hierarchicalData, nameColumn)
    {
      dataTreeView.ChildCollectionPropertyName = childCollectionPropertyName;
    }

    public HierarchyEditor(IEnumerable hierarchicalData, string nameColumn, IDataEditorPersister dataEditorPersister, params string[] membersToExclude)
      : this(hierarchicalData, nameColumn)
    {
      gridDataEditor.DataEditorPersister = dataEditorPersister;
      gridDataEditor.MembersToExclude = membersToExclude;
      saveToolStripButton.Enabled = gridDataEditor.DataEditorPersister != null;
      toolStripButtonCancelEdit.Enabled = saveToolStripButton.Enabled;
    }

    public HierarchyEditor(IEnumerable hierarchicalData, string iDPropertyName, string parentIDPropertyName, string nameColumn, IDataEditorPersister dataEditorPersister, params string[] membersToExclude)
      : this(hierarchicalData, nameColumn, dataEditorPersister, membersToExclude)
    {
      dataTreeView.IDColumn = iDPropertyName;
      dataTreeView.ParentIDColumn = parentIDPropertyName;
    }

    public HierarchyEditor(IEnumerable hierarchicalData, string nameColumn, string childCollectionPropertyName, IDataEditorPersister dataEditorPersister, params string[] membersToExclude)
      : this(hierarchicalData, nameColumn, dataEditorPersister, membersToExclude)
    {
      dataTreeView.ChildCollectionPropertyName = childCollectionPropertyName;
    }

    public static HierarchyEditor HierarchyEditorFactory<T, TId, TParentId, TName>(IEnumerable<T> enumerable, Expression<Func<T, TId>> iDPropertyExpression,
      Expression<Func<T, TParentId>> parentIDPropertyExpression, Expression<Func<T, TName>> namePropertyExpression,
      IDataEditorPersister dataEditorPersister, params string[] membersToExclude)
    {
      return new HierarchyEditor(enumerable, MemberName.For(iDPropertyExpression), MemberName.For(parentIDPropertyExpression), MemberName.For(namePropertyExpression), dataEditorPersister, membersToExclude);
    }

    private void dataTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
      toolStripStatusLabelSelectePath.Text = ((dataTreeView).SelectedNode).FullPath;
      propertyGrid1.SelectedObject = e.Node.Tag;
      splitContainerHorizontal.Panel2Collapsed = e.Node.Nodes.Count == 0;
      if (splitContainerHorizontal.Panel2Collapsed)
        gridDataEditor.DataSource = null;
      else
        gridDataEditor.BindEnumerable(e.Node.Nodes.Cast<TreeNode>().Select(tn => tn.Tag).ToList());
    }

    private void toolStripButtonExpandAll_Click(object sender, EventArgs e)
    {
      dataTreeView.ExpandAll();
    }

    private void toolStripButtonCollapseAll_Click(object sender, EventArgs e)
    {
      dataTreeView.CollapseAll();
    }

    private void saveToolStripButton_Click(object sender, EventArgs e)
    {
      if (gridDataEditor.DataEditorPersister != null)
        gridDataEditor.DataEditorPersister.Save(bindingSourceHierarchicalData.DataSource);
    }

    private void toolStripButtonCancelEdit_Click(object sender, EventArgs e)
    {
      bindingSourceHierarchicalData.CancelEdit();
      if (gridDataEditor.DataEditorPersister != null && gridDataEditor.DataEditorPersister.Undo(bindingSourceHierarchicalData.List))
      {
        bindingSourceHierarchicalData.ResetBindings(false);
        //saveToolStripButton.Enabled = false;
      }
      //toolStripButtonCancelEdit.Enabled = false;
    }

    public void BeginInit()
    {
      ((ISupportInitialize) (bindingSourceHierarchicalData)).BeginInit();
      ((ISupportInitialize) (bindingNavigatorHierarchicalData)).BeginInit();
      ((ISupportInitialize) (gridDataEditor)).BeginInit();
      ((ISupportInitialize) (dataTreeView)).BeginInit();
    }

    public void EndInit()
    {
      ((ISupportInitialize) (bindingSourceHierarchicalData)).EndInit();
      ((ISupportInitialize) (bindingNavigatorHierarchicalData)).EndInit();
      ((ISupportInitialize) (dataTreeView)).EndInit();
      ((ISupportInitialize) (gridDataEditor)).EndInit();
    }
  }
}