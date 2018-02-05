using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.Test.Model;
using DataAccess.FrameWork;
using DataAccess.Enums;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataAccess.Test
{
    [TestClass]
    public class EntityFrameWorkTest
    {
        string dbConnection = @"";
        [TestMethod]
        public void TestAdd()
        {
            TestEntity t = new TestEntity { Name = null, Password = "1036", Mobile = "tcp://180.169.101.177:41205", Pk = "12340", Email = "中信期货" };
            var en = new EntityFrameWork<TestEntity>(DBType.MYSQL);
            var s1 = en.Add(t, dbConnection);
        }

        [TestMethod]
        public void TestUpdate()
        {
            TestEntity t = new TestEntity { Name = "tcp://180.169.101.177:41213", Password = "1036", Mobile = "tcp://180.169.101.177:41205", Pk = "1234", Email = "中信期货0" };
            var en = new EntityFrameWork<TestEntity>(DBType.MYSQL);
            var s1 = en.Update(t, dbConnection);
            Assert.AreEqual(s1, 1);
        }

        [TestMethod]
        public void TestOnlyUpdate()
        {
            TestEntity t = new TestEntity { Name = "tcp://180.169.101.177", Password = "16", Mobile = "tcp://180.169.101.177", OldPk = "1234", Email = "中信期货1" };
            var en = new EntityFrameWork<TestEntity>(DBType.MYSQL);
            var fileds = new List<string>() { "Email" };
            var s1 = en.Update(t, dbConnection, fileds);
            Assert.AreEqual(s1, 1);
        }

        [TestMethod]
        public void TestSelectNoPage()
        {
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            var en = new EntityFrameWork<StockSymbolInfo>(DBType.MYSQL);
            var parm = new Dictionary<string, object>();
            //parm.Add("Email", "中信期货");
            var s1 = en.List(dbConnection, parm);
            watch.Stop();
            var s = watch.ElapsedMilliseconds;
            //Assert.AreEqual(s1.Count, 1);
        }
        [TestMethod]
        public void TestSelectPage()
        {
            var en = new EntityFrameWork<TestEntity>(DBType.MYSQL);
            var parm = new Dictionary<string, object>();
            //parm.Add("Email>=", "Simnow模拟");
            var page = new DataAccess.Model.DataPage { PageIndex = 1, PageSize = 1 };
            var s1 = en.List(dbConnection, parm, page);
            Assert.AreEqual(s1.Count, 1);
        }


        [TestMethod]
        public void TestSelectTimee()
        {
            var en = new EntityFrameWork<StockSymbolInfo>(DBType.MYSQL);
            var parm = new Dictionary<string, object>();
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            //parm.Add("Email", "中信期货");
            var s1 = en.List(dbConnection, parm);
            watch.Stop();
            StockSymbolInfo asd = new StockSymbolInfo();


            //var s2 = en.SelectDataTable(new FrameWorkItem
            //{
            //    ConnectionString = dbConnection,
            //    SqlParam = parm,
            //    Sql = asd.GetSelectSql()
            //});
            var s = watch.ElapsedMilliseconds;
            //Assert.AreEqual(s1.Count, 1);
        }
    }
}
