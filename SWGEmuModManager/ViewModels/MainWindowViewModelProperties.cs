using System.Collections.Generic;
using System.Windows;

namespace SWGEmuModManager.ViewModels;
enum FilterType
{
    Name,
    Author,
    Version,
    Downloads,
    Released
}

internal class MainWindowViewModelProperties : MainWindowViewModelResponses
{
    private List<ModsDisplay>? _modList;
    private int? _progressBarPercentage;
    private Visibility? _progessBarVisibility;
    private string? _progressBarStatusLabel;
    private string? _installButtonText;
    private int _filterType;
    private int _filterOrder;
    private Thickness? _filterMargin;
    private string? _sortWatermark;
    private string? _sortValue;

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

    public int FilterType
    {
        get => _filterType;
        set
        {
            SetProperty(field: ref _filterType, value);
            MainWindowViewModel.Filter(this);
        }
    }

    public int FilterOrder
    {
        get => _filterOrder;
        set
        {
            SetProperty(field: ref _filterOrder, value);
            MainWindowViewModel.Filter(this);
        }
    }

    public string? SortWatermark
    {
        get => _sortWatermark;
        set => SetProperty(field: ref _sortWatermark, value);
    }

    public string? SortValue
    {
        get => _sortValue;
        set
        {
            SetProperty(field: ref _sortValue, value);
            MainWindowViewModel.WatermarkIntercept(this);
        }
    }

    public Thickness? FilterMargin
    {
        get => _filterMargin;
        set => SetProperty(field: ref _filterMargin, value);
    }
}
