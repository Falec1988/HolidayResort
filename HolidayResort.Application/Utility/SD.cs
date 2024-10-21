using HolidayResort.Domain.Entities;
using HolidayResort.Web.ViewModels;

namespace HolidayResort.Application.Utility;

public static class SD
{
    public const string Role_Customer = "Customer";
    public const string Role_Admin = "Admin";

    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusCheckedIn = "CheckedIn";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    public static int AccommodationRoomsAvailableCount(int accommodationId,
        List<AccommodationNumber> accommodationNoList, DateOnly checkInDate, int nights,
        List<Booking> bookings)
    {
        List<int> bookingInDate = new();

        int finalAvailableRoomForAllNights = int.MaxValue;

        var roomsInAccommodation = accommodationNoList.Where(x => x.AccommodationId == accommodationId).Count();

        for (int i = 0; i < nights; i++)
        {
            var accommodationBooked = bookings.Where(x => x.CheckInDate <= checkInDate.AddDays(i)
            && x.CheckOutDate > checkInDate.AddDays(i) && x.AccommodationId == accommodationId);

            foreach (var booking in accommodationBooked)
            {
                if (!bookingInDate.Contains(booking.Id))
                {
                    bookingInDate.Add(booking.Id);
                }
            }

            var totalAvailableRooms = roomsInAccommodation - bookingInDate.Count;

            if (totalAvailableRooms == 0)
            {
                return 0;
            }
            else
            {
                if (finalAvailableRoomForAllNights > totalAvailableRooms)
                {
                    finalAvailableRoomForAllNights = totalAvailableRooms;
                }
            }
        }

        return finalAvailableRoomForAllNights;
    }

    public static RadialBarChartDto GetRadialChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
    {
        RadialBarChartDto RadialBarChartDto = new();

        int increaseDecreaseRatio = 100;

        if (prevMonthCount != 0)
        {
            increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
        }

        RadialBarChartDto.TotalCount = totalCount;

        RadialBarChartDto.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);

        RadialBarChartDto.HasRatioIncreased = currentMonthCount > prevMonthCount;

        RadialBarChartDto.Series = new int[] { increaseDecreaseRatio };

        return RadialBarChartDto;
    }
}