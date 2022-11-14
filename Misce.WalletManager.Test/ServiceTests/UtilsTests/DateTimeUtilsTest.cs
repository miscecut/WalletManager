using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.DTO.Enums;

namespace Misce.WalletManager.Test.ServiceTests.UtilsTests
{
    [TestClass]
    public class DateTimeUtilsTest
    {
        [TestMethod]
        public void TestGetDateForAmountMap()
        {
            //DAY

            var changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022,11,14,15,23,56), GroupByPeriod.DAY); //11/14/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 11);
            Assert.AreEqual(changedDate.Day, 14);
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            //WEEK

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 11, 14, 15, 23, 56), GroupByPeriod.WEEK); //mon 11/14/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 11);
            Assert.AreEqual(changedDate.Day, 20); //next sunday
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 11, 30, 15, 23, 56), GroupByPeriod.WEEK); //wed 30/11/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 12);
            Assert.AreEqual(changedDate.Day, 4); //next sunday
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 10, 9, 15, 23, 56), GroupByPeriod.WEEK); //sun 09/10/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 10);
            Assert.AreEqual(changedDate.Day, 9); //the same day, a sunday
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            //MONTH

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 5, 3, 15, 23, 56), GroupByPeriod.MONTH); //03/05/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 5);
            Assert.AreEqual(changedDate.Day, 31); //the 31st of may, the last day of the month
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 11, 1, 15, 23, 56), GroupByPeriod.MONTH); //01/11/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 11);
            Assert.AreEqual(changedDate.Day, 30); //the 30th of november, the last day of the month
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 11, 30, 15, 23, 56), GroupByPeriod.MONTH); //30/11/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 11);
            Assert.AreEqual(changedDate.Day, 30); //the 30th of november, the (same) last day of the month
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);

            //YEAR

            changedDate = DateTimeUtils.GetDateTimeForAmountHistory(new DateTime(2022, 11, 30, 15, 23, 56), GroupByPeriod.YEAR); //30/11/2022 15:23:56
            Assert.AreEqual(changedDate.Year, 2022);
            Assert.AreEqual(changedDate.Month, 12);
            Assert.AreEqual(changedDate.Day, 31);
            Assert.AreEqual(changedDate.Hour, 23);
            Assert.AreEqual(changedDate.Minute, 59);
            Assert.AreEqual(changedDate.Second, 59);
        }
    }
}
