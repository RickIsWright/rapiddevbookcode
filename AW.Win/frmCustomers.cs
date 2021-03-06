using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AW.Data;
using AW.Data.Linq.Filters;
using AW.Data.Queries;
using AW.Helper.LLBL;
using AW.Winforms.Helpers;
using AW.Winforms.Helpers.EntityViewer;
using Serilog;

namespace AW.Win
{
  public partial class FrmCustomers : FrmPersistantLocation
  {
    private CancellationTokenSource cancellationToken;

    public FrmCustomers()
    {
      InitializeComponent();
    }

    private int MaxNumberOfItemsToReturn
    {
      get { return Convert.ToInt32(numericUpDownNumRows.Value); }
      set { numericUpDownNumRows.Value = value; }
    }

    private void frmCustomers_FormClosing(object sender, FormClosingEventArgs e)
    {
    }

    private void frmCustomers_Load(object sender, EventArgs e)
    {
      dgvResults.AutoGenerateColumns = true;
    }

    /// <summary> 
    ///   Handles the Click event of the toolStripButtonPlaintypedview control. Example 5.18. pg59.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private async void toolStripButtonPlaintypedview_ClickAsync(object sender, EventArgs e)
    {
      progressBar1.Show();
      labelProgress.Text = "GetCriteria "+ DateTime.Now.ToLongTimeString();
      Log.Logger.Debug("GetCriteria ");
      cancellationToken = new CancellationTokenSource();
      var token = cancellationToken.Token;
      Application.DoEvents();
      try
      {
        var orderSearchCriteria = orderSearchCriteria1.GetCriteria();
        var maxNumberOfItemsToReturn = MaxNumberOfItemsToReturn;
      labelProgress.Text += Environment.NewLine+ "Fetching " +DateTime.Now.ToLongTimeString();
        Log.Logger.Debug("Fetching ");
        var customerViewTypedView = await Task.Run(() => CustomerQueries.GetCustomerViewTypedView(orderSearchCriteria, maxNumberOfItemsToReturn), token);
      labelProgress.Text += Environment.NewLine + "Rendering " + DateTime.Now.ToLongTimeString();
        Log.Logger.Debug("Rendering ");
        Application.DoEvents();
        token.ThrowIfCancellationRequested();
        bindingSourceCustomerList.DataSource = customerViewTypedView;
        Log.Logger.Debug("Done ");
        labelProgress.Text += Environment.NewLine + "Done" + DateTime.Now.ToLongTimeString(); ;
      }
      catch (OperationCanceledException)
      {
        labelProgress.Text += Environment.NewLine + "Cancelled. " + DateTime.Now.ToLongTimeString(); ;
      }

      progressBar1.Hide();
    }

    private void toolStripButtonTypedViewQuerySpec_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerViewTypedViewQuerySpec(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
    }

    private void toolStripButtonTypedViewQuerySpecPoco_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerViewTypedViewQuerySpecPoco(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
    }

    /// <summary>
    ///   Handles the Click event of the toolStripButtonViewAsEntity control. Example 5.27 pg63.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void toolStripButtonViewAsEntity_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerViewRelatedCollection();
    }

    private void toolStripButtonViewAsEntity_Click_1(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerViewViaEntity(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
    }

    private void toolStripButtonViewAsEntityQuerySpec_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerViewViaEntityQuerySpec(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
    }

    /// <summary>
    ///   Handles the Click event of the toolStripButtonTypedList control. Example 5.29. pg64.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void toolStripButtonTypedList_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerListTypedList(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
    }

    private async void toolStripButtonTypedListQuerySpec_Click(object sender, EventArgs e)
    {
      await bindingSourceCustomerList.BindEnumerableAsync(CustomerQueries.GetCustomerListTypedListQuerySpec(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn), true, cancellationToken.Token, true);
    }

    private async void toolStripButtonTypedListQuerySpecPoco_Click(object sender, EventArgs e)
    {
      await bindingSourceCustomerList.BindEnumerableAsync(CustomerQueries.GetCustomerListTypedListQuerySpecPoco(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn), true, cancellationToken.Token, true);
    }

    /// <summary>
    ///   Handles the Click event of the toolStripButtonLinq control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void toolStripButtonLinq_Click(object sender, EventArgs e)
    {
      BindingListHelper.BindEnumerable(bindingSourceCustomerList, CustomerQueries.GetCustomerListLinqedTypedList(orderSearchCriteria1.GetCriteria(), 0).ToList().Distinct().Take(MaxNumberOfItemsToReturn));
    }

    private async void toolStripButtonLinqFilterFirst_Click(object sender, EventArgs e)
    {
      progressBar1.Show();
      await bindingSourceCustomerList.BindEnumerableAsync(CustomerQueries.GetCustomerListLinqedTypedListFilterFirst(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn), true, cancellationToken.Token, true);
      progressBar1.Hide();
    }

    private async void toolStripButtonLinqTypedview_Click(object sender, EventArgs e)
    {
      labelProgress.Text = "GetCriteria";
      Application.DoEvents();
      var orderSearchCriteria = orderSearchCriteria1.GetCriteria();
      var maxNumberOfItemsToReturn = MaxNumberOfItemsToReturn;
      labelProgress.Text = "Fetching";
      var customerViewTypedView = await CustomerQueries.GetCustomerViewTypedViewLinqAsync(orderSearchCriteria, maxNumberOfItemsToReturn);
      labelProgress.Text = "Rendering";
      Application.DoEvents();
      bindingSourceCustomerList.DataSource = customerViewTypedView;
      labelProgress.Text = "Done";
      progressBar1.Hide();
    }

    private void toolStripButtonTypedListLinq_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = CustomerQueries.GetCustomerListGeneratedLinqTypedList(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn);
      ;
    }

    /// <summary>
    ///   Handles the Click event of the toolStripButtonLinqBarf control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private async void toolStripButtonLinqBarf_Click(object sender, EventArgs e)
    {
      await bindingSourceCustomerList.BindEnumerableAsync(CustomerQueries.GetCustomerListAnonymousLinq(orderSearchCriteria1.GetCriteria(), MaxNumberOfItemsToReturn), true, cancellationToken.Token, true);
    }

    private void toolStripButtonViewAsEntityLinq_Click(object sender, EventArgs e)
    {
      bindingSourceCustomerList.DataSource = MetaSingletons.MetaData.CustomerViewRelated
        .FilterByDateOrderIDOrderNumberCustomerNameAddress(orderSearchCriteria1.GetCriteria())
        .Take(MaxNumberOfItemsToReturn).ToEntityCollection();
    }

    private void View()
    {
      ((FrmMain) MdiParent).LaunchChildForm(typeof (FrmEntityViewer), bindingSourceCustomerList.Current);
    }

    private void dgvResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      View();
    }

    private void dgvResults_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      View();
    }

    private void dgvResults_SortStringChanged(object sender, EventArgs e)
    {
      bindingSourceCustomerList.Sort = dgvResults.SortString;
    }

    private void dgvResults_FilterStringChanged(object sender, EventArgs e)
    {
      if (bindingSourceCustomerList.SupportsFiltering)
        bindingSourceCustomerList.Filter = dgvResults.FilterString;
    }

    private void toolStripButtonClearSort_Click(object sender, EventArgs e)
    {
      dgvResults.ClearSort(true);
      if (bindingSourceCustomerList.SupportsSorting)
        bindingSourceCustomerList.RemoveSort();
    }

    private void toolStripButtonClearFilters_Click(object sender, EventArgs e)
    {
      dgvResults.ClearFilter(true);
      if (bindingSourceCustomerList.SupportsFiltering)
        bindingSourceCustomerList.RemoveFilter();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      if (cancellationToken != null)
        cancellationToken.Cancel();
    }

    private void dgvResults_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      Log.Logger.Debug("Row {0}", e.RowIndex);
      Application.DoEvents();
    }

    private void dgvResults_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
    {
      Log.Logger.Debug("DataBindingComplete {0}", e.ListChangedType);
      Application.DoEvents();
    }

    private void dgvResults_DataSourceChanged(object sender, EventArgs e)
    {
      Log.Logger.Debug("dgvResults_DataSourceChanged {0}", e);
      Application.DoEvents();
    }

    private void bindingSourceCustomerList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      Log.Logger.Debug("dgvResults_DataSourceChanged {0}", e);
      Application.DoEvents();
    }

    private void bindingSourceCustomerList_DataSourceChanged(object sender, EventArgs e)
    {
      Log.Logger.Debug("dgvResults_DataSourceChanged {0}", e);
      Application.DoEvents();
    }
  }
}