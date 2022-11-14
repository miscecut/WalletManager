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
    }
}
