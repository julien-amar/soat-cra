using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Soat.Cra.Interfaces
{
    public interface ICraDownloader
    {
        Task<IEnumerable<string>> SignAllMissionsCra(HttpClient client, int year, int month);
    }
}