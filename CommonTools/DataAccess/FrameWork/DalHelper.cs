using DataAccess.Enums;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.FrameWork
{
    public class DalHelper<T> where T : BaseEntity<T>, new()
    {
        private DBType dbType;
        public DalHelper(DBType dbType)
        {
            this.dbType = dbType;
        }


    }
}
