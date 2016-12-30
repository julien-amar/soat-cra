using Colorful;
using Newtonsoft.Json;
using Soat.Cra.Interfaces;
using Soat.Cra.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace Soat.Cra.Signing
{
    public class CraDownloader : ICraDownloader
    {
        private readonly IPdfWatermarker _pdfWatermarker;

        private readonly string _craListUri;
        private readonly string _craDownloadUri;
        private readonly string _craSignature;

        public CraDownloader(IPdfWatermarker pdfWatermarker)
        {
            _pdfWatermarker = pdfWatermarker;

            _craListUri = ConfigurationManager.AppSettings["CRA.List"];
            _craDownloadUri = ConfigurationManager.AppSettings["CRA.Download"];

            _craSignature = ConfigurationManager.AppSettings["CRA.Signature"];
        }

        private async Task<CRA> GetCraForMonth(HttpClient client, int year, int month)
        {
            var data = new StringContent(string.Format("{{\"month\":\"{0}-{1:00}-01T00:00:00.000Z\" }}", year, month), Encoding.UTF8, "application/json");

            var result = await client.PostAsync(_craListUri, data);
            var content = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CRA>(content);
        }

        private async Task<string> DownloadCra(HttpClient client, CRA cra, KeyValuePair<int, Mission> mission)
		{
            var downloadUrl = string.Format(_craDownloadUri,
                cra.IdCol,
                mission.Value.IdClient,
                cra.Month.Year,
                cra.Month.Month);

            var filename = Path.GetTempFileName();

            using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var file = await client.GetAsync(downloadUrl);

                await file.Content
                    .CopyToAsync(fileStream)
                    .ContinueWith(_ => { fileStream.Close(); });
            }

            return filename;
        }

        public async Task<IEnumerable<string>> SignAllMissionsCra(HttpClient client, int year, int month)
        {
            var signedCra = new List<string>();

            var cra = await GetCraForMonth(client, year, month);

            foreach (var mission in cra.Missions)
            {
                var baseDirectory = AppContext.BaseDirectory;

                var targetFilename = string.Format("{0}-{1:00}_CRA_Client_{2}.pdf", year, month, mission.Value.Client);

                string targetFile = Path.Combine(baseDirectory, targetFilename);
                string tempFile = await DownloadCra(client, cra, mission);

                _pdfWatermarker.AddWatermark(tempFile, targetFile, _craSignature);

                Console.WriteLineFormatted("{0} Sign: {1}", Color.White, new Formatter(">>", Color.Yellow), new Formatter(targetFilename, Color.White));

                signedCra.Add(targetFile);
            }

            return signedCra;
        }
    }
}