using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.Test.Model;
using DataAccess.FrameWork;
using DataAccess.Enums;

namespace DataAccess.Test
{
    [TestClass]
    public class UnitTest1
    {
        string dbConnection = @"Server=121.42.226.233;Database=artoffuture;Uid=root;Pwd=Qwer1234;Charset=utf8;";
        [TestMethod]
        public void TestMethod1()
        {
            TestEntity t = new TestEntity { Name = "tcp://180.169.101.177:41213", Password = "1036", Mobile = "tcp://180.169.101.177:41205", Pk = "1234", Email = "中信期货" };
            var en = new EntityFrameWork<TestEntity>(DBType.MYSQL);
            var s1 = en.Add(t, dbConnection);
        }
    }
}
