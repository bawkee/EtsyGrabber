using System;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace EtsyGrabber
{
	class Program
	{
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed(Do)
				.WithNotParsed(errors =>
				{
					if (errors.IsVersion() || errors.IsHelp())
					{
						Console.WriteLine(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description);
						return;
					}
					foreach (var err in errors.Select(e => e.GetType())
						.Where(t => !new[]
						{
							typeof(UnknownOptionError), 
							typeof(MissingRequiredOptionError)
						}.Contains(t)))
						Console.WriteLine(err.ToString());
				});
		}

		static void Do(Options options)
		{
			Console.WriteLine("Press CTRL+C to cancel.");

			var grabber = new EtsyGrabber(options.CsvPath, options.OutputDir, options.Throttle)
			{
				Progress = image =>
				{
					Console.CursorLeft = 0;
					Console.Write($"Processed {image} so far.".PadRight(30));
				}
			};

			grabber.Grab().GetAwaiter().GetResult();

			Console.WriteLine();
			Console.WriteLine("Done.");
		}
	}
}
