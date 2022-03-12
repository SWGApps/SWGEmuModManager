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

        public static async Task<MainWindowViewModelResponses.PaginatedResponse<List<MainWindowViewModelResponses.Mod>>> GetModsAsync(int startPage, int totalItems)
        {
            var client = new HttpClient();

            using HttpResponseMessage response = await client.GetAsync(new Uri($"{_apiUrl}/Mods/{startPage}/{totalItems}"));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MainWindowViewModelResponses.PaginatedResponse<List<MainWindowViewModelResponses.Mod>>>() 
                       ?? new MainWindowViewModelResponses.PaginatedResponse<List<MainWindowViewModelResponses.Mod>>();
            }

            return new MainWindowViewModelResponses.PaginatedResponse<List<MainWindowViewModelResponses.Mod>>();
        }

        public static async Task<MainWindowViewModelResponses.InstallRequestResponse> InstallModAsync(int id)
        {
            using HttpResponseMessage response = await GetResponseAsync(new Uri($"{_apiUrl}/Mod/Install/{id}"));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MainWindowViewModelResponses.InstallRequestResponse>() 
                       ?? new MainWindowViewModelResponses.InstallRequestResponse();
            }

            return new MainWindowViewModelResponses.InstallRequestResponse();
        }

        public static async Task<MainWindowViewModelResponses.UninstallRequestResponse> UninstallModAsync(int id)
        {
            using HttpResponseMessage response = await GetResponseAsync(new Uri($"{_apiUrl}/Mod/Uninstall/{id}"));

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MainWindowViewModelResponses.UninstallRequestResponse>() 
                       ?? new MainWindowViewModelResponses.UninstallRequestResponse();
            }

            return new MainWindowViewModelResponses.UninstallRequestResponse();
        }

        private static async Task<HttpResponseMessage> GetResponseAsync(Uri uri)
        {
            var client = new HttpClient();

            try
            {
                return await client.GetAsync(uri);
            }
            catch (Exception e)
            {
                throw new Exception(e.StackTrace);
            }
        }
    }
}
