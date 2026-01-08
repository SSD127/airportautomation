namespace AirportAutomation.UI.Helpers
{
    public static class ServiceHelper
    {
        public static IServiceProvider? Services { get; set; }

        public static T GetRequiredService<T>() where T : notnull
        {
            if (Services == null) throw new InvalidOperationException("Service provider not initialized.");
            return Services.GetService(typeof(T)) is T service
                ? service
                : throw new InvalidOperationException($"Service {typeof(T)} not found.");
        }
    }
}

