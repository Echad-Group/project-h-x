using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectHX.Mobile.ViewModels;

public abstract class ProfileSectionViewModelBase : ObservableObject
{
    private readonly BaseViewModel _pageState;

    protected ProfileSectionViewModelBase(BaseViewModel pageState)
    {
        _pageState = pageState;
    }

    protected bool IsPageBusy => _pageState.IsBusy;

    protected void ClearMessages()
    {
        _pageState.ErrorMessage = null;
        _pageState.InfoMessage = null;
    }

    protected void SetInfo(string? message)
    {
        _pageState.InfoMessage = message;
    }

    protected void SetError(string? message)
    {
        _pageState.ErrorMessage = message;
    }

    protected bool TryStartOperation()
    {
        if (_pageState.IsBusy)
        {
            return false;
        }

        _pageState.IsBusy = true;
        return true;
    }

    protected void FinishOperation()
    {
        _pageState.IsBusy = false;
    }
}