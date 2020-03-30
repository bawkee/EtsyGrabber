using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace EtsyGrabber
{
	public class EtsyGrabber
	{
		private readonly string _file;
		private readonly string _outputDir;
		private readonly int _throttleSeconds;

		public Action<int> Progress { get; set; }

		public EtsyGrabber(string file, string outputDir, int throttleSeconds = 2)
		{
			_file = file;
			_outputDir = outputDir;
			_throttleSeconds = throttleSeconds;
		}

		public async Task Grab()
		{
			using TextFieldParser csv = new TextFieldParser(_file);

			csv.SetDelimiters(",");
			csv.HasFieldsEnclosedInQuotes = true;

			const int imgColStart = 7;
			const int imgColCnt = 10;

			csv.ReadLine();

			var totalImages = 0;

			while (!csv.EndOfData)
			{
				var data = csv.ReadFields();

				for (var imgCol = imgColStart; imgCol < imgColStart + imgColCnt; imgCol++)
				{
					var image = data[imgCol];

					if (string.IsNullOrEmpty(image))
						continue;
					var title = data[0];
					await DownloadImage(image, $"{title}_{imgCol - imgColStart}");

					Progress?.Invoke(++totalImages);
				}
			}
		}

		static readonly IList<char> invalidFileChars = Path.GetInvalidFileNameChars();

		private async Task DownloadImage(string url, string title)
		{
			var extension = url.Split('.').Last();
			var fileName = new string(title.Select(ch =>
				invalidFileChars.Contains(ch) ? Convert.ToChar(invalidFileChars.IndexOf(ch) + 65) : ch).ToArray()) + $".{extension}";
			var filePath = Path.Combine(_outputDir, fileName);

			if (File.Exists(filePath))
				return;

			await Task.Delay(_throttleSeconds);

			using var client = new WebClient();

			client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

			client.DownloadFile(url, filePath);
		}
	}
}
