using System;
using Avalonia.Controls;

namespace CalculatorApp;

public partial class MainWindow : Window
{
    private double _accumulator;
    private string? _pendingOperator;
    private bool _isNewEntry = true;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnDigitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        var digit = button.Tag?.ToString() ?? button.Content?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(digit))
            return;

        if (_isNewEntry || Display.Text == "0")
        {
            Display.Text = digit;
            _isNewEntry = false;
        }
        else
        {
            Display.Text += digit;
        }
    }

    private void OnDecimalClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isNewEntry)
        {
            Display.Text = "0.";
            _isNewEntry = false;
        }
        else if (Display.Text is not null && !Display.Text.Contains('.'))
        {
            Display.Text += ".";
        }
    }

    private void OnBackspaceClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Display.Text is not null && Display.Text.Length > 1 && Display.Text != "0")
        {
            Display.Text = Display.Text[..^1];
            _isNewEntry = false;
        }
        else
        {
            Display.Text = "0";
            _isNewEntry = true;
        }
    }

    private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _accumulator = 0;
        _pendingOperator = null;
        _isNewEntry = true;
        Display.Text = "0";
    }

    private void OnSignToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (double.TryParse(Display.Text, out var value))
        {
            value = -value;
            Display.Text = value.ToString();
        }
    }

    private void OnPercentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (double.TryParse(Display.Text, out var value))
        {
            value = value / 100.0;
            Display.Text = value.ToString();
            _isNewEntry = true;
        }
    }

    private void OnOperatorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        var op = button.Tag?.ToString();
        if (string.IsNullOrEmpty(op))
            return;

        if (double.TryParse(Display.Text, out var value))
        {
            if (_pendingOperator is not null && !_isNewEntry)
            {
                _accumulator = ApplyOperator(_accumulator, value, _pendingOperator);
                Display.Text = _accumulator.ToString();
            }
            else
            {
                _accumulator = value;
            }
        }

        _pendingOperator = op;
        _isNewEntry = true;
    }

    private void OnEqualsClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_pendingOperator is null)
            return;

        if (!double.TryParse(Display.Text, out var value))
            return;

        _accumulator = ApplyOperator(_accumulator, value, _pendingOperator);
        Display.Text = _accumulator.ToString();
        _pendingOperator = null;
        _isNewEntry = true;
    }

    private static double ApplyOperator(double left, double right, string op)
    {
        return op switch
        {
            "add" => left + right,
            "subtract" => left - right,
            "multiply" => left * right,
            "divide" => right == 0 ? double.NaN : left / right,
            _ => right
        };
    }
}

