using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerilogAndApplicationInsightsDemo
{
    class Program
    {
        private const string ApplicationInsightsInstrumentationKey = "YOU SHOULD STORE ME IN APP.CONFIG";
        static void Main(string[] args)
        {
            ConfigureApplicationInsightsLogging();

            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var dto = new SomeDTO(random.Next(0, 10), random.Next(0, 10));

                DoSomething(dto);
            }
            Serilog.Log.CloseAndFlush(); // This line is important, logs are not being sent immediately, so sinks should be "flushed" safely before closing app
            Console.ReadKey();
        }

        private static void DoSomething(SomeDTO dto)
        {
            try
            {
                if (dto.SomeValue1 > 5)
                    throw new ArgumentOutOfRangeException($"Incoming dto parameter {nameof(dto.SomeValue1)} should never ever be bigger than 5!");

                Serilog.Log.Information("Just received and processed dto. Incoming arguments: {@dto}", dto);
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e, "Faulty args: {@dto}", dto);
            }
        }

        private static void ConfigureApplicationInsightsLogging()
        {
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                    .MinimumLevel.Verbose() // Minimum severity to log
                    .WriteTo
                    .ApplicationInsightsEvents(ApplicationInsightsInstrumentationKey)
                    .CreateLogger();
        }
    }

    class SomeDTO
    {

        public int SomeValue1 { get; }
        public int SomeValue2 { get; }
        public SomeDTO(int someValue1, int someValue2)
        {
            SomeValue1 = someValue1;
            SomeValue2 = someValue2;
        }
    }
}
