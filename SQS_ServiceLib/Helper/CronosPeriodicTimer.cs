using Cronos;

namespace SQS_ServiceLib.Helper
{
    public sealed class CronosPeriodicTimer : IDisposable
    {
        private readonly CronExpression _cronExpression; // Also used as the locker
        private PeriodicTimer _activeTimer;
        public CronosPeriodicTimer(string expression, CronFormat format)
        {
            _cronExpression = CronExpression.Parse(expression, format);
        }

        public async ValueTask<bool> WaitForNextTickAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var currentTime = new DateTime(DateTime.Now.Ticks, DateTimeKind.Utc);
            var nextSchedule = _cronExpression.GetNextOccurrence(currentTime.AddMilliseconds(500), true);
            if (nextSchedule is null)
            {
                throw new InvalidOperationException("Invalid date - date is unrachable");
            }
            var timeSpan = (TimeSpan)(nextSchedule - currentTime);
            Console.WriteLine($"next execution is in {timeSpan}");
            _activeTimer = _activeTimer ?? new PeriodicTimer(timeSpan);
            return await _activeTimer.WaitForNextTickAsync(cancellationToken);
        }

        public void Dispose()
        {
            _activeTimer?.Dispose();
        }
    }
}
