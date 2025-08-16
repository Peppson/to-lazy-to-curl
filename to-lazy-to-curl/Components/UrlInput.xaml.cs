using System.Windows.Controls;
using System.Windows.Media;

namespace to_lazy_to_curl.Components
{ 
    public partial class UrlInput : UserControl
    {
        public UrlInput()
        {
            InitializeComponent();
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBorderColors();
        }

        private void UpdateBorderColors()
        {
            Console.WriteLine("TEST");
            /*if (UrlTextBox == null) return;

            var UrlHasText =
                !string.IsNullOrWhiteSpace(UrlTextBox!.Text) &&
                Uri.IsWellFormedUriString(UrlTextBox!.Text, UriKind.Absolute);

            UrlTextBox.BorderBrush = UrlHasText ? Brushes.Green : Brushes.Gray;*/
        }


        /* public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(UrlInput));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(UrlInput));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(UrlInput));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        } */
    }

}