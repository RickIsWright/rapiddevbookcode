//css_ref SD.LLBLGen.Pro.ORMSupportClasses.dll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AW.Data;
using AW.Data.Linq;
using AW.Data.CollectionClasses;
using AW.Data.EntityClasses;
using AW.Winforms.Helpers.QueryRunner;

public class Script : MarshalByRefObject, IQueryScript
{
  public static IEnumerable Query()
  {
    return EmployeeEntity.GetEmployees();
  }

}