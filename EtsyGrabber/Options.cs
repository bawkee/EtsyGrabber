using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace EtsyGrabber
{
	class Options
	{
		[Option("outDir", Required = true, HelpText = "The output path for grabbed images.")]
		public string OutputDir { get; set; }

		[Option("csv", Required = true, HelpText = "The path to the CSV file containing the urls.")]
		public string CsvPath { get; set; }

		[Option("throttle", HelpText = "How much time in milliseconds (1000ms=1sec) to pause between each download. Default is 250ms.")]
		public int Throttle { get; set; } = 250;
	}
}
