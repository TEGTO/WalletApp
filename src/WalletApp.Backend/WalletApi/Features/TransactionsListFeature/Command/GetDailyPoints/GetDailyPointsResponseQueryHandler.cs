using MediatR;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetDailyPoints
{
    public class GetDailyPointsResponseQueryHandler : IRequestHandler<GetDailyPointsResponseQuery, DailyPointsResponse>
    {
        public Task<DailyPointsResponse> Handle(GetDailyPointsResponseQuery request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.Date;
            var firstDayOfSeason = GetFirstDayOfSeason(currentDate);
            int seasonDay = (currentDate - firstDayOfSeason).Days + 1;

            decimal points = CalculateDailyPoints(seasonDay);

            string formattedPoints = FormatPoints(points);

            return Task.FromResult(new DailyPointsResponse
            {
                Points = formattedPoints
            });
        }

        private static DateTime GetFirstDayOfSeason(DateTime currentDate)
        {
            int year = currentDate.Year;

            if (currentDate.Month >= 3 && currentDate.Month <= 5) // Spring
                return new DateTime(year, 3, 1);
            if (currentDate.Month >= 6 && currentDate.Month <= 8) // Summer
                return new DateTime(year, 6, 1);
            if (currentDate.Month >= 9 && currentDate.Month <= 11) // Autumn
                return new DateTime(year, 9, 1);
            return new DateTime(year, 12, 1); // Winter
        }

        private static decimal CalculateDailyPoints(int seasonDay)
        {
            if (seasonDay == 1)
                return 2;
            if (seasonDay == 2)
                return 3;

            decimal pointsDay1 = 2;
            decimal pointsDay2 = 3;

            for (int day = 3; day <= seasonDay; day++)
            {
                decimal newPoints = Math.Floor(pointsDay1 + pointsDay2 * 0.6M);
                pointsDay1 = pointsDay2;
                pointsDay2 = newPoints;
            }

            return pointsDay2;
        }
        private string FormatPoints(decimal points)
        {
            if (points >= 1000000)
                return $"{Math.Floor(points / 1000000)}M";
            else if (points >= 1000)
                return $"{Math.Floor(points / 1000)}K";

            return points.ToString();
        }
    }
}
