/*
 * ToDo:
 *
 * Make pagination local?
 *
 * Resolve issue with SSL cert, likely just needs an approved signed cert in the API
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Toolkit.Mvvm.Input;
using SWGEmuModManager.Models;
using SWGEmuModManager.Util;
using SWGEmuModManager.Views;

namespace SWGEmuModManager.ViewModels;

internal class MainWindowViewModel : MainWindowViewModelProperties
{
    public IAsyncRelayCommand GenerateModManifestMenuItem { get; }
    public IRelayCommand SetSwgDirectoryMenuItem { get; set; }
    public IAsyncRelayCommand DownloadModButton { get; }
    public IRelayCommand CloseButton { get; set; }
    public IAsyncRelayCommand FilterNameButton { get; set; } 
    public bool ConflictContinue { get; set; }
    public IAsyncRelayCommand NextPageButton { get; set; }
    public IAsyncRelayCommand PreviousPageButton { get; set; }
    public IRelayCommand SourceButton { get; set; }

    public MainWindowViewModel()
    {
        StartPage = 1;

        // Setting this will refresh the mod display
        TotalItems = 0;

        Task.Run(InitializeAsync);

        GenerateModManifestMenuItem = new AsyncRelayCommand(GenerateModManifestAsync);
        SetSwgDirectoryMenuItem = new RelayCommand(SetSwgDirectory);
        DownloadModButton = new AsyncRelayCommand<int>(GetModDataAsync);
        CloseButton = new RelayCommand(() => Environment.Exit(0));
        FilterNameButton = new AsyncRelayCommand(RefreshModDisplay);
        NextPageButton = new AsyncRelayCommand(NextPage);
        PreviousPageButton = new AsyncRelayCommand(PreviousPage);
        SourceButton = new RelayCommand<string>(GoToSourcePage);

        MainWindowModel.OnDownloadProgressUpdated += DownloadProgressUpdated;
        ZipArchiveExtension.OnInstallStarted += InstallStarted;
        ZipArchiveExtension.OnInstallProgressUpdated += InstallProgressUpdated;
        MainWindowModel.OnUninstallDone += UninstallDone;
        ConflictDialogWindowViewModel.ClickedContinueButton += ClickedContinueButton;
        ConflictDialogWindowViewModel.ClickedCancelButton += ClickedCancelButton;
    }

    private async Task InitializeAsync()
    {
        ProgressBarVisibility = Visibility.Collapsed;

        PaginatedResponse<List<Mod>> mods = await ApiHandler.GetModsCacheAsync();
        ModListCache = mods.Data;

        InstallButtonText = "Install";
        FilterWatermark = "Filter By Name";

        if (HasNextPage)
        {
            NextPageButtonVisibility = Visibility.Visible;
        }
        else
        {
            NextPageButtonVisibility = Visibility.Collapsed;
        }

        if (HasPreviousPage)
        {
            PreviousPageButtonVisibility = Visibility.Visible;
        }
        else
        {
            PreviousPageButtonVisibility = Visibility.Collapsed;
        }
    }

    private async Task GenerateModManifestAsync()
    {
        using FolderBrowserDialog dialog = new();
        DialogResult result = dialog.ShowDialog();

        if (result.ToString().Trim() == "OK")
        {
            await ManifestGenerator.GenerateModManifestAsync(
                modsDirectory: dialog.SelectedPath.Replace(oldValue: "\\", newValue: "/"));
        }
    }

    private void SetSwgDirectory()
    {
        MainWindowModel.SetSwgDirectory();
    }

    private async Task GetModDataAsync(int id)
    {
        ConfigFile config = ConfigFile.GetConfig()!;

        // Uninstall mod
        if (MainWindowModel.ModIsInstalled(id))
        {
            Response<UninstallRequestResponse> uninstallResponse = await ApiHandler.UninstallModAsync(id);

            ProgressBarVisibility = Visibility.Visible;

            /*List<int> allowedConflicts = MainWindowModel.CheckConflictList(uninstallResponse.ConflictList);

            allowedConflicts.ForEach(conflict =>
            {
                Trace.WriteLine(conflict);
            });*/

            if (!string.IsNullOrEmpty(config.SwgDirectory))
            {
                if (uninstallResponse.Succeeded)
                {
                    if (uninstallResponse.Data!.FileList!.Count > 0)
                    {
                        ProgressBarStatusLabel = "Uninstalling " + ModList!
                            .Where(x => x.Id == id)
                            .Select(x => x.Name)
                            .FirstOrDefault() + "...";

                        await MainWindowModel.UninstallMod(id, uninstallResponse.Data.FileList);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(text: "Uninstall request response status: " +
                        $"{uninstallResponse.Succeeded} - Reason: {uninstallResponse.Errors![0]}", 
                        caption: "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    App.log.Error(message: $"Uninstall request response status: {uninstallResponse.Succeeded}" +
                        $" - Reason: {uninstallResponse.Errors[0]}");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(text: "No SWG directory set! " +
                    "Please set your SWG location in Config -> Set SWG Directory and try again.", 
                    caption: "No SWG Directory Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        // Install Mod
        Response<InstallRequestResponse> installResponse = await ApiHandler.InstallModAsync(id);

        // Set continue before getting conflicts, it will be set during conflict check
        // if conflicts are found
        ConflictContinue = true;

        List<string> conflictNames = MainWindowModel.GetConflictNames
            (installResponse.Data!.ConflictList!, ModListCache!.ToList());

        if (conflictNames.Count > 0)
        {
            new ConflictDialogWindow(conflictNames).ShowDialog();
        }

        if (MainWindowModel.HasFolderConflict(ModListCache!, id))
        {
            System.Windows.Forms.MessageBox.Show
                ("Non-mod manager controlled file conflict detected! " +
                 "Please uninstall all manually installed mods before using the mod manager!");
        }

        if (!ConflictContinue) return;

        ProgressBarVisibility = Visibility.Visible;

        if (!string.IsNullOrEmpty(config.SwgDirectory) && 
                !string.IsNullOrEmpty(installResponse.Data.DownloadUrl) && 
                !string.IsNullOrEmpty(installResponse.Data.Archive))
        {
            if (installResponse.Succeeded)
            {
                ProgressBarStatusLabel = "Downloading " + ModList!
                    .Where(x => x.Id == id)
                    .Select(x => x.Name)
                    .FirstOrDefault() + "...";

                await MainWindowModel.DownloadModAsync(id, installResponse.Data.DownloadUrl, installResponse.Data.Archive);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(text: "Install request response status: " +
                    $"{installResponse.Succeeded} - Reason: {installResponse.Errors![0]}",
                    caption: "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                App.log.Error(message: "Install request response status: " +
                    $"{installResponse.Succeeded} - Reason: {installResponse.Errors[0]}");
            }
        }
        else
        {
            System.Windows.Forms.MessageBox.Show(text: "No SWG directory set! " +
                "Please set your SWG location in Config -> Set SWG Directory and try again.",
                caption: "No SWG Directory Set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        await InstallDone(id);
    }

    private void DownloadProgressUpdated(long bytesReceived, long totalBytesToReceive, int progressPercentage)
    {
        ProgressBarPercentage = progressPercentage;
    }

    private void InstallStarted()
    {
        ProgressBarStatusLabel = ProgressBarStatusLabel!.Replace(oldValue: "Downloading", newValue: "Installing");
    }

    private void InstallProgressUpdated(int currentFile, int totalFiles)
    {
        ProgressBarPercentage = (currentFile / totalFiles) * 1000;
    }

    private async Task InstallDone(int id)
    {
        System.Threading.Thread.Sleep(millisecondsTimeout: 1000);
        ProgressBarVisibility = Visibility.Collapsed;
        Response<object> response = await ApiHandler.AddDownloadAsync(id);

        if (response.Succeeded) await RefreshModDisplay();
        // todo - else notify of failure
    }

    private async void UninstallDone()
    {
        ProgressBarVisibility = Visibility.Collapsed;

        await RefreshModDisplay();
    }

    private async Task RefreshModDisplay()
    {
        PaginatedResponse<List<Mod>> mods = await ApiHandler.GetModsAsync
            (StartPage, TotalItems, SortType, SortOrder!, FilterValue!);

        HasNextPage = mods.HasNextPage;
        HasPreviousPage = mods.HasPreviousPage;
        FirstItemOnPage = mods.FirstItemOnPage;
        LastItemOnPage = mods.LastItemOnPage;
        IsFirstPage = mods.IsFirstPage;
        IsLastPage = mods.IsLastPage;
        PageCount = mods.PageCount;
        PageNumber = mods.PageNumber;
        PageSize = mods.PageSize;
        TotalItemCount = mods.TotalItemCount;

        ModList = await MainWindowModel.SetModDisplay(mods);
    }

    private void ClickedContinueButton()
    {
        ConflictContinue = true;
    }

    private void ClickedCancelButton()
    {
        ConflictContinue = false;
    }

    private void GoToSourcePage(string? url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    internal static void WatermarkIntercept(MainWindowViewModelProperties vmp)
    {
        if (vmp.FilterValue is not null)
        {
            vmp.FilterWatermark = (vmp.FilterValue!.Length < 1) ? "Filter By Name" : string.Empty;
        }
    }

    internal static void UpdateFilterMargin(MainWindowViewModelProperties vmp)
    {
        vmp.FilterMargin = vmp.ProgressBarVisibility switch
        {
            Visibility.Visible => new Thickness(left: 0, top: 0, right: 8, bottom: 13),
            Visibility.Collapsed => new Thickness(left: 0, top: 7, right: 8, bottom: -5),
            _ => new Thickness(left: 0, top: 7, right: 8, bottom: -5),
        };
    }

    internal static async void Sort(MainWindowViewModelProperties vmp)
    {
        PaginatedResponse<List<Mod>> mods = await ApiHandler.GetModsAsync
            (vmp.StartPage, vmp.TotalItems, vmp.SortType, vmp.SortOrder!, vmp.FilterValue!);

        vmp.HasNextPage = mods.HasNextPage;
        vmp.HasPreviousPage = mods.HasPreviousPage;
        vmp.FirstItemOnPage = mods.FirstItemOnPage;
        vmp.LastItemOnPage = mods.LastItemOnPage;
        vmp.IsFirstPage = mods.IsFirstPage;
        vmp.IsLastPage = mods.IsLastPage;
        vmp.PageCount = mods.PageCount;
        vmp.PageNumber = mods.PageNumber;
        vmp.PageSize = mods.PageSize;
        vmp.TotalItemCount = mods.TotalItemCount;

        vmp.ModList = await MainWindowModel.SetModDisplay(mods);
    }

    internal static void OnModListUpdated(MainWindowViewModelProperties vmp)
    {
        if (vmp.HasNextPage)
        {
            vmp.NextPageButtonVisibility = Visibility.Visible;
        }
        else
        {
            vmp.NextPageButtonVisibility = Visibility.Collapsed;
        }

        if (vmp.HasPreviousPage)
        {
            vmp.PreviousPageButtonVisibility = Visibility.Visible;
        }
        else
        {
            vmp.PreviousPageButtonVisibility = Visibility.Collapsed;
        }
    }

    private async Task NextPage()
    {
        StartPage += 1;
        await RefreshModDisplay();
    }

    private async Task PreviousPage()
    {
        StartPage -= 1;
        await RefreshModDisplay();
    }
}
