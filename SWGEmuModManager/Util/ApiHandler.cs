using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SWGEmuModManager.ViewModels;

namespace SWGEmuModManager.Util;

internal class ApiHandler : MainWindowViewModelResponses
{
    private static readonly string _apiUrl = "https://localhost:7193";

    public static async Task<T?> GetDeserializedResponse<T>(Uri uri)
    {
        HttpClient client = new();

        try
        {
            using HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException(e.StackTrace);
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException(e.StackTrace);
        }
        catch (TaskCanceledException e)
        {
            throw new TaskCanceledException(e.StackTrace);
        }
        finally
        {
            client.Dispose();
        }

        return (T)Activator.CreateInstance(typeof(T))!;
    }

    public static async Task<PaginatedResponse<List<Mod>>> GetModsAsync(int startPage, int totalItems)
    {
        return await GetDeserializedResponse<PaginatedResponse<List<Mod>>>
            (new Uri($"{_apiUrl}/Mods/{startPage}/{totalItems}")) ?? new PaginatedResponse<List<Mod>>();
    }

    public static async Task<Response<object>> AddDownloadAsync(int id)
    {
        return await GetDeserializedResponse<Response<object>>
            (new Uri($"{_apiUrl}/Mod/AddDownload/{id}")) ?? new Response<object>();
    }

    public static async Task<Response<InstallRequestResponse>> InstallModAsync(int id)
    {
        return await GetDeserializedResponse<Response<InstallRequestResponse>>
            (new Uri($"{_apiUrl}/Mod/Install/{id}")) ?? new Response<InstallRequestResponse>();
    }

    public static async Task<Response<UninstallRequestResponse>> UninstallModAsync(int id)
    {
        return await GetDeserializedResponse<Response<UninstallRequestResponse>>
            (new Uri($"{_apiUrl}/Mod/Uninstall/{id}")) ?? new Response<UninstallRequestResponse>();
    }
}
