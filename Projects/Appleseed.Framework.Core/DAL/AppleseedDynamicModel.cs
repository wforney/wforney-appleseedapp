using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appleseed.Framework.DAL
{
    class AppleseedDynamicModel: Massive.DynamicModel
    {
        public AppleseedDynamicModel(string tableName = "", string primaryKeyField = "")
            : base(connectionStringName: "ConnectionString", 
            tableName: tableName, 
            tableNamePrefix: "rb_", 
            primaryKeyField: primaryKeyField)
        {

        }
    }
}
