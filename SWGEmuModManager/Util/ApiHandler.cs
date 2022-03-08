using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Util
{
    internal class ApiHandler
    {
        private static readonly string _apiUrl = "https://localhost:7193";

        public static async Task<List<Mod>> GetModsAsync()
        {
            var client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync(new Uri($"{_apiUrl}/Mods"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Mod>>() ?? new List<Mod>();
        }

        public static async Task<InstallRequestResponse> InstallModAsync(int id)
        {
            var client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync(new Uri($"{_apiUrl}/Mod/Install/{id}"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<InstallRequestResponse>() ?? new InstallRequestResponse();
        }
    }
}
