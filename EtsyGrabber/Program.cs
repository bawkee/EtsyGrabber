using System;

namespace EtsyGrabber
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var grabber = new EtsyGrabber("EtsyListingsDownload.csv", ".\\Output")
			{
				Progress = image =>
				{
					Console.Clear();
					Console.Write($"Processed {image} so far.");
				}
			};

			grabber.Grab().GetAwaiter().GetResult();
		}
	}
}
