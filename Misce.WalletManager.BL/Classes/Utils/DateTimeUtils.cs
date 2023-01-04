using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.Enums;

namespace Misce.WalletManager.BL.Classes.Utils
{
    public class DateTimeUtils
    {
        public static DateTime GetDateTimeForAmountHistory(DateTime dateTimeToTransform, GroupByPeriod period)
        {
            var dateTime = dateTimeToTransform;
            switch (period)
            {
                case GroupByPeriod.DAY: //12/03/2022 15:40 becomes 12/03/2022 23:59
                    dateTime = new DateTime(dateTimeToTransform.Year, dateTimeToTransform.Month, dateTimeToTransform.Day, 23, 59, 59);
                    break;
                case GroupByPeriod.WEEK: //the date becomes next sunday (or the same day if it's already a sunday)
                    var start = dateTimeToTransform.DayOfWeek;
                    dateTime = dateTime.AddDays(start == DayOfWeek.Sunday ? 0 : (7 - (int)start));
                    dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
                    break;
                case GroupByPeriod.MONTH: //13/05/2021 becomes 31/05/2021
                    var lastDayNumberInMonth = DateTime.DaysInMonth(dateTimeToTransform.Year, dateTimeToTransform.Month);
                    dateTime = new DateTime(dateTimeToTransform.Year, dateTimeToTransform.Month, lastDayNumberInMonth, 23, 59, 59);
                    break;
                case GroupByPeriod.YEAR:
                    dateTime = new DateTime(dateTimeToTransform.Year, 12, 31, 23, 59, 59);
                    break;
            }

            return dateTime;
        }

        public static DateTime GetNextValue(DateTime dateTime, GroupByPeriod period)
        {
            var nextDateTime = dateTime;
            switch (period)
            {
                case GroupByPeriod.DAY: //the next day
                    nextDateTime = dateTime.AddDays(1);
                    break;
                case GroupByPeriod.WEEK: //the same day of the week next week
                    nextDateTime = dateTime.AddDays(7);
                    break;
                case GroupByPeriod.MONTH: //the last day of next month
                    nextDateTime = dateTime.AddMonths(1);
                    var lastDayNumberInMonth = DateTime.DaysInMonth(nextDateTime.Year, nextDateTime.Month);
                    nextDateTime = new DateTime(nextDateTime.Year, nextDateTime.Month, lastDayNumberInMonth, nextDateTime.Hour, nextDateTime.Minute, nextDateTime.Second); //adjust day to the last one of the month
                    break;
                case GroupByPeriod.YEAR: //the 31st of december, next year
                    nextDateTime = new DateTime(dateTime.Year + 1, 12, 31, dateTime.Hour, dateTime.Minute, dateTime.Second);
                    break;
            }
            return nextDateTime;
        }

        public static DateTime GetOldestStartingDateForAccount(IEnumerable<TransactionDTOOut> transactions, DateTime accountCreatedDate)
        {
            var oldestDate = accountCreatedDate;

            foreach(var transactionDate in transactions.Select(t => t.DateTime))
            {
                if (transactionDate < oldestDate)
                    oldestDate = transactionDate;
            }

            return oldestDate;
        }
    }
}
