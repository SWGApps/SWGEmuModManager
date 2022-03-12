namespace SWGEmuModManager.ViewModels;

internal class ExamineWindowViewModel : ExamineWindowViewModelProperties
{
    public ExamineWindowViewModel(MainWindowViewModelResponses.ModsDisplay modDisplay)
    {
        ModData = modDisplay;
    }
}
