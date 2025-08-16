using System.Windows.Controls;
using to_lazy_to_curl.Services;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    public string JsonText
    {
        get => JsonTextBox.Text;
        set => JsonTextBox.Text = value;
    }

    public JsonInput()
    {
        InitializeComponent();
        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Options.EnableEmailHyperlinks = false;
        JsonTextBox.Text = Config.JsonSampleData;
        HttpService.JsonInputEditor = JsonTextBox;
        //UiService.JsonInputEditor = JsonTextBox; // todo border instead
    }
}
