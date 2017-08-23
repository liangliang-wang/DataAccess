using DataAccess.Attributes;
using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Test.Model
{
    [Entity("aof_futurescompany")]
    public class TestEntity : BaseEntity<TestEntity>
    {
        public string DBName = "artoffuture";

        [DBField(FieldName = "PK")]
        [PrimaryKey]
        public string Pk { set; get; }

        [DBField(FieldName = "BrokerID")]
        public string Password { set; get; }

        [DBField(FieldName = "BrokerName")]
        public string Email { set; get; }

        [DBField(FieldName = "Trading")]
        public string Mobile { set; get; }

        [DBField(FieldName = "MarketData")]
        public string Name { set; get; }
    }
}
