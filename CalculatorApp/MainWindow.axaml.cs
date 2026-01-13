using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;

namespace CalculatorApp;

[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    private readonly Calculator _calculator;

    public MainWindow()
    {
        InitializeComponent();
        _calculator = new Calculator();
        Display.Text = Calculator.DefaultDisplay;
    }

    private void OnDigitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        var digit = button.Tag?.ToString() ?? button.Content?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(digit))
            return;

        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessDigit(digit, currentText);
    }

    private void OnDecimalClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessDecimal(currentText);
    }

    private void OnBackspaceClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessBackspace(currentText);
    }

    private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Display.Text = _calculator.Clear();
    }

    private void OnSignToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ToggleSign(currentText);
    }

    private void OnPercentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessPercent(currentText);
    }

    private void OnOperatorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        var op = button.Tag?.ToString();
        if (string.IsNullOrEmpty(op))
            return;

        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessOperator(op, currentText);
    }

    private void OnEqualsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var currentText = Display.Text ?? Calculator.DefaultDisplay;
        Display.Text = _calculator.ProcessEquals(currentText);
    }
}
