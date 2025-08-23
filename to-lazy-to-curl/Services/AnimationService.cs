using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using to_lazy_to_curl.Components;

namespace to_lazy_to_curl.Services;

static public class AnimationService
{
    private const int _durationMs = 500;

    private static readonly List<EasingDoubleKeyFrame> _shakeKeyFrames =
    [
        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(0)),
        new EasingDoubleKeyFrame(-5, KeyTime.FromPercent(0.1)),
        new EasingDoubleKeyFrame(5, KeyTime.FromPercent(0.2)),
        new EasingDoubleKeyFrame(-5, KeyTime.FromPercent(0.3)),
        new EasingDoubleKeyFrame(5, KeyTime.FromPercent(0.4)),
        new EasingDoubleKeyFrame(-5, KeyTime.FromPercent(0.5)),
        new EasingDoubleKeyFrame(5, KeyTime.FromPercent(0.6)),
        new EasingDoubleKeyFrame(-5, KeyTime.FromPercent(0.7)),
        new EasingDoubleKeyFrame(5, KeyTime.FromPercent(0.8)),
        new EasingDoubleKeyFrame(0, KeyTime.FromPercent(1.0)),
    ];

    public static async Task InvalidBorderAnimationAsync(Border border)
    {
        if (border == null) return;

        var animation = GetAnimation();
        border.BorderBrush = (Brush)Application.Current.Resources["Failure"];
        border.RenderTransform = new TranslateTransform();
        border.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);

        await Task.Delay(_durationMs);
        border.ClearValue(Border.BorderBrushProperty);
    }

    public static async Task InvalidButtonAnimationAsync(Button button)
    {
        if (button == null) return;

        button.ApplyTemplate();
        if (button.Template.FindName("border", button) is Border border)
        {
            await InvalidBorderAnimationAsync(border);
        }
    }

    public static async Task InvalidHeaderAnimationAsync(EditorInput editorInput)
    {
        if (editorInput == null) return;

        Button target = AppState.IsNarrow
            ? editorInput.HeaderButton 
            : editorInput.SplitHeaderButton;

        if (target == null) return;

        target.ApplyTemplate();

        if (target.Template.FindName("border", target) is Border border)
        {
            border.BorderThickness = new Thickness(1, 1, 1, 0);
            await InvalidBorderAnimationAsync(border);
            border.ClearValue(Border.BorderThicknessProperty);
        }
    }

    private static DoubleAnimationUsingKeyFrames GetAnimation()
    {
        var animation = new DoubleAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromMilliseconds(_durationMs)
        };

        foreach (var frame in _shakeKeyFrames)
        {
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(frame.Value, frame.KeyTime));
        }

        return animation;
    }
}
