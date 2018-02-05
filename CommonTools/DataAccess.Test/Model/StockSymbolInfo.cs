using DataAccess.Attributes;
using DataAccess.Model;
using System;

namespace DataAccess.Test.Model
{
    [Entity("StockSymbolInfo")]
    [DataBase("artofstock")]
    public class StockSymbolInfo : BaseEntity<StockSymbolInfo>
    {
        /// <summary>
        /// 股票
        /// </summary>
        [DBField(FieldName = "Id")]
        [PrimaryKey(IsIdentity = true)]
        public long Id { set; get; }

        /// <summary>
        /// 股票
        /// </summary>
        [DBField(FieldName = "SecuritiesCode")]
        public string SecuritiesCode { set; get; }

        /// <summary>
        /// 交易日
        /// </summary>
        [DBField(FieldName = "Tradingday")]
        public string Tradingday { set; get; }

        /// <summary>
        /// 开盘价
        /// </summary>
        [DBField(FieldName = "Open")]
        public decimal Open { set; get; }

        /// <summary>
        /// 昨收
        /// </summary>
        [DBField(FieldName = "Preclose")]
        public decimal Preclose { set; get; }

        /// <summary>
        /// 最高价
        /// </summary>
        [DBField(FieldName = "Upperlimit")]
        public decimal Upperlimit { set; get; }

        /// <summary>
        /// 最低价
        /// </summary>
        [DBField(FieldName = "Lowerlimit")]
        public decimal Lowerlimit { set; get; }

    }
}
