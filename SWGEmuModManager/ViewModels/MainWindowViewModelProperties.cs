using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SWGEmuModManager.ViewModels;

internal class MainWindowViewModelProperties : MainWindowViewModelResponses
{
    private ObservableCollection<ModsDisplay>? _modList;
    private int? _progressBarPercentage;
    private Visibility? _progessBarVisibility;
    private string? _progressBarStatusLabel;
    private string? _installButtonText;
    private int _sortType;
    private int _sortOrder;
    private Thickness? _filterMargin;
    private string? _filterWatermark;
    private string? _filterValue;
    private bool _hasNextPage;
    private bool _hasPreviousPage;
    private int _firstItemOnPage;
    private int _lastItemOnPage;
    private bool _isFirstPage;
    private bool _isLastPage;
    private int _pageCount;
    private int _pageNumber;
    private int _pageSize;
    private int _totalItemCount;
    private Visibility? _nextPageButtonVisibility;
    private Visibility? _previousPageButtonVisibility;

    public ObservableCollection<ModsDisplay>? ModList
    {
        get => _modList;
        set
        {
            SetProperty(field: ref _modList, value);
            MainWindowViewModel.OnModListUpdated(this);
        }
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

    public bool HasNextPage
    {
        get => _hasNextPage;
        set => SetProperty(ref _hasNextPage, value);
    }

    public bool HasPreviousPage
    {
        get => _hasPreviousPage;
        set => SetProperty(ref _hasPreviousPage, value);
    }

    public int FirstItemOnPage
    {
        get => _firstItemOnPage;
        set => SetProperty(ref _firstItemOnPage, value);
    }

    public int LastItemOnPage
    {
        get => _lastItemOnPage;
        set => SetProperty(ref _lastItemOnPage, value);
    }

    public bool IsFirstPage
    {
        get => _isFirstPage;
        set => SetProperty(ref _isFirstPage, value);
    }

    public bool IsLastPage
    {
        get => _isLastPage;
        set => SetProperty(ref _isLastPage, value);
    }

    public int PageCount
    {
        get => _pageCount;
        set => SetProperty(ref _pageCount, value);
    }

    public int PageNumber
    {
        get => _pageNumber;
        set => SetProperty(ref _pageNumber, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int TotalItemCount
    {
        get => _totalItemCount;
        set => SetProperty(ref _totalItemCount, value);
    }

    public Visibility? NextPageButtonVisibility
    {
        get => _nextPageButtonVisibility;
        set => SetProperty(ref _nextPageButtonVisibility, value);
    }

    public Visibility? PreviousPageButtonVisibility
    {
        get => _previousPageButtonVisibility;
        set => SetProperty(ref _previousPageButtonVisibility, value);
    }
}
