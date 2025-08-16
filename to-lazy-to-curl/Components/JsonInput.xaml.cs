using System.Windows.Controls;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    public JsonInput()
    {
        InitializeComponent();
        JsonTextBox.Options.EnableHyperlinks = false;
        JsonTextBox.Options.EnableEmailHyperlinks = false;
        JsonTextBox.Text = Config.JsonSampleData;
    }
}
