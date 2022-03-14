using System.Collections.Generic;
using System.Windows;

namespace SWGEmuModManager.ViewModels;

internal class MainWindowViewModelProperties : MainWindowViewModelResponses
{
    private List<ModsDisplay>? _modList;
    private int? _progressBarPercentage;
    private Visibility? _progessBarVisibility;
    private string? _progressBarStatusLabel;
    private string? _installButtonText;
    private int _sortType;
    private int _sortOrder;
    private Thickness? _filterMargin;
    private string? _filterWatermark;
    private string? _filterValue;

    public List<ModsDisplay>? ModList
    {
        get => _modList;
        set => SetProperty(field: ref _modList, value);
    }

    public int? ProgressBarPercentage
    {
        get => _progressBarPercentage;
        set => SetProperty(field: ref _progressBarPercentage, value);
    }

    public Visibility? ProgressBarVisibility
    {
        get => _progessBarVisibility;
        set
        {
            SetProperty(field: ref _progessBarVisibility, value);
            MainWindowViewModel.UpdateFilterMargin(this);
        } 
    }

    public string? ProgressBarStatusLabel
    {
        get => _progressBarStatusLabel;
        set => SetProperty(field: ref _progressBarStatusLabel, value);
    }

    public string? InstallButtonText
    {
        get => _installButtonText;
        set => SetProperty(field: ref _installButtonText, value);
    }

    public int SortType
    {
        get => _sortType;
        set
        {
            SetProperty(field: ref _sortType, value);
            MainWindowViewModel.Sort(this);
        }
    }

    public int SortOrder
    {
        get => _sortOrder;
        set
        {
            SetProperty(field: ref _sortOrder, value);
            MainWindowViewModel.Sort(this);
        }
    }

    public string? FilterWatermark
    {
        get => _filterWatermark;
        set => SetProperty(field: ref _filterWatermark, value);
    }

    public string? FilterValue
    {
        get => _filterValue;
        set
        {
            SetProperty(field: ref _filterValue, value);
            MainWindowViewModel.WatermarkIntercept(this);
        }
    }

    public Thickness? FilterMargin
    {
        get => _filterMargin;
        set => SetProperty(field: ref _filterMargin, value);
    }
}
