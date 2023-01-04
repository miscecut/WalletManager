using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.DTO.DTO.Transaction;
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

        [TestMethod]
        public void TestGetNextDateTime()
        {
            //DAY

            var nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2022, 11, 30, 23, 59, 59), GroupByPeriod.DAY);
            Assert.AreEqual(nextDateTime.Year, 2022);
            Assert.AreEqual(nextDateTime.Month, 12);
            Assert.AreEqual(nextDateTime.Day, 1); //the next day
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);

            //WEEK

            nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2022, 12, 11, 23, 59, 59), GroupByPeriod.WEEK);
            Assert.AreEqual(nextDateTime.Year, 2022);
            Assert.AreEqual(nextDateTime.Month, 12);
            Assert.AreEqual(nextDateTime.Day, 18); //the next sunday
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);

            //MONTH

            nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2022, 6, 30, 23, 59, 59), GroupByPeriod.MONTH);
            Assert.AreEqual(nextDateTime.Year, 2022);
            Assert.AreEqual(nextDateTime.Month, 7);
            Assert.AreEqual(nextDateTime.Day, 31); //the next last day of the month
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);

            nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2022, 1, 31, 23, 59, 59), GroupByPeriod.MONTH);
            Assert.AreEqual(nextDateTime.Year, 2022);
            Assert.AreEqual(nextDateTime.Month, 2);
            Assert.AreEqual(nextDateTime.Day, 28); //the next last day of the month
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);

            nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2022, 7, 31, 23, 59, 59), GroupByPeriod.MONTH);
            Assert.AreEqual(nextDateTime.Year, 2022);
            Assert.AreEqual(nextDateTime.Month, 8);
            Assert.AreEqual(nextDateTime.Day, 31); //the next last day of the month
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);

            //YEAR

            nextDateTime = DateTimeUtils.GetNextValue(new DateTime(2021, 12, 31, 23, 59, 59), GroupByPeriod.YEAR);
            Assert.AreEqual(nextDateTime.Year, 2022); //the next last day of the year
            Assert.AreEqual(nextDateTime.Month, 12);
            Assert.AreEqual(nextDateTime.Day, 31);
            Assert.AreEqual(nextDateTime.Hour, 23);
            Assert.AreEqual(nextDateTime.Minute, 59);
            Assert.AreEqual(nextDateTime.Second, 59);
        }

        [TestMethod]
        public void TestGetOldestStartingDateForAccount()
        {
            var accountCreationDate = new DateTime(2022, 12, 11);

            var transactions = new List<TransactionDTOOut>
            {
                new TransactionDTOOut
                {
                    Amount = 3,
                    DateTime = new DateTime(2022, 12, 12),
                    Id = new Guid()
                },
                new TransactionDTOOut
                {
                    Amount = 6,
                    DateTime = new DateTime(2023, 1, 3),
                    Id = new Guid()
                }
            };

            Assert.AreEqual(DateTimeUtils.GetOldestStartingDateForAccount(transactions, accountCreationDate), accountCreationDate);

            transactions.Add(new TransactionDTOOut
            {
                Amount = 3,
                DateTime = new DateTime(2022, 5, 2),
                Id = new Guid()
            });

            Assert.AreEqual(DateTimeUtils.GetOldestStartingDateForAccount(transactions, accountCreationDate), new DateTime(2022, 5, 2));
        }
    }
}
