using to_lazy_to_curl.Models;

namespace to_lazy_to_curl.State;

static public class States
{
    private static HttpAction _selectedAction = HttpAction.None;
    public static HttpAction SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (_selectedAction == value) return;
            _selectedAction = value;
            SelectedActionChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    public static event EventHandler? SelectedActionChanged;
}
