﻿using System;
using System.Configuration;
using System.Net;
using System.Windows.Forms;
using AW.Helper;
using AW.Winforms.Helpers;
using Northwind.DAL.Linq;
using Northwind.DAL.SqlServer;
//using Northwind.Win.NorthwindODataSR;
using Northwind.Win.NorthwindODataSR;

namespace Northwind.Win
{
  public partial class FrmEntityBrowser : FrmPersistantLocation
  {
    public FrmEntityBrowser()
    {
      InitializeComponent();
      try
      {
      var baseAddress = ConfigurationManager.AppSettings["WcfDataServiceUrl"];
      var uri = new Uri(baseAddress);// + typeof(LLBLGenProODataService).Name);
      var llblGenProODataService = new LLBLGenProODataService(uri);
      //var webClient = new WebClient();
      //var openRead = webClient.OpenRead(llblGenProODataService.Employee.RequestUri);
      //  llblGenProODataService.BeginExecute<Northwind.DAL.EntityClasses.EmployeeEntity>(llblGenProODataService.Employee.RequestUri, handleResult, null);
     // llblGenProODataService.Execute<object>(llblGenProODataService.Employee.RequestUri);


      var employee = llblGenProODataService.CreateQuery<object>("Employee");
      var employeeEntities = employee.Execute();
      //  var categoryEntities = llblGenProODataService.Category.Execute();
      }
      catch (Exception e)
      {
        Application.OnThreadException(e);
      }

      var dataAccessAdapter = new DataAccessAdapter();
      usrCntrlEntityBrowser1.Initialize(new LinqMetaData(dataAccessAdapter));
      //Done in CommonEntityBase  CacheController.RegisterCache(dataAccessAdapter.ConnectionString, new ResultsetCache()); 
      Text += string.Format(" - {0}", ProfilerHelper.OrmProfilerStatus);
    }

    private void handleResult(IAsyncResult ar)
    {
     // ar.
    }
  }
}