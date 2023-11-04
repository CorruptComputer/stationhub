using Avalonia.Controls;
using Avalonia.Input;

namespace UnitystationLauncher.Views;

public partial class BlogPostView : UserControl
{
    public BlogPostView()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerLeave(object? sender, PointerEventArgs e)
    {
        this.GetControl<Panel>("TitlePanel").IsVisible = true;
        this.GetControl<Panel>("SummaryPanel").IsVisible = false;
    }

    private void InputElement_OnPointerEnter(object? sender, PointerEventArgs e)
    {
        this.GetControl<Panel>("TitlePanel").IsVisible = false;
        this.GetControl<Panel>("SummaryPanel").IsVisible = true;
    }
}