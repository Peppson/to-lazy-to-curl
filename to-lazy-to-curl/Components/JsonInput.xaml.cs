using System.Windows.Controls;
using to_lazy_to_curl.Services;

namespace to_lazy_to_curl.Components;

public partial class JsonInput : UserControl
{
    public JsonInput()
    {
        InitializeComponent();
        JsonTextBox.Text = StartupData.GetJsonSampleData();
    }




}
