using System;
using System.Globalization;
using Avalonia.Controls;

namespace CalculatorApp;

public partial class MainWindow : Window
{
    private const string OperatorAdd = "add";
    private const string OperatorSubtract = "subtract";
    private const string OperatorMultiply = "multiply";
    private const string OperatorDivide = "divide";
    private const string ErrorMessage = "Error";
    private const string DefaultDisplay = "0";

    private double _accumulator;
    private string? _pendingOperator;
    private bool _isNewEntry = true;
    private bool _hasError;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnDigitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_hasError)
        {
            ClearError();
        }

        var digit = button.Tag?.ToString() ?? button.Content?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(digit))
            return;

        var currentText = Display.Text ?? DefaultDisplay;

        if (_isNewEntry || currentText == DefaultDisplay)
        {
            Display.Text = digit;
            _isNewEntry = false;
        }
        else
        {
            Display.Text = currentText + digit;
        }
    }

    private void OnDecimalClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_hasError)
        {
            ClearError();
        }

        var currentText = Display.Text ?? DefaultDisplay;

        if (_isNewEntry)
        {
            Display.Text = "0.";
            _isNewEntry = false;
        }
        else if (!currentText.Contains('.'))
        {
            Display.Text = currentText + ".";
        }
    }

    private void OnBackspaceClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_hasError)
        {
            ClearError();
            return;
        }

        var currentText = Display.Text ?? DefaultDisplay;

        if (currentText.Length > 1 && currentText != DefaultDisplay)
        {
            Display.Text = currentText[..^1];
            _isNewEntry = false;
        }
        else
        {
            Display.Text = DefaultDisplay;
            _isNewEntry = true;
        }
    }

    private void OnClearClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ResetCalculator();
        Display.Text = DefaultDisplay;
    }

    private void OnSignToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_hasError)
            return;

        var currentText = Display.Text ?? DefaultDisplay;
        if (double.TryParse(currentText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            value = -value;
            Display.Text = FormatNumber(value);
        }
    }

    private void OnPercentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_hasError)
            return;

        var currentText = Display.Text ?? DefaultDisplay;
        if (double.TryParse(currentText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            value /= 100.0;
            Display.Text = FormatNumber(value);
            _isNewEntry = true;
        }
    }

    private void OnOperatorClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_hasError)
            return;

        var op = button.Tag?.ToString();
        if (string.IsNullOrEmpty(op))
            return;

        var currentText = Display.Text ?? DefaultDisplay;
        if (double.TryParse(currentText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            if (_pendingOperator is not null && !_isNewEntry)
            {
                var result = ApplyOperator(_accumulator, value, _pendingOperator);
                if (!IsValidResult(result))
                {
                    ShowError();
                    return;
                }
                _accumulator = result;
                Display.Text = FormatNumber(_accumulator);
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
        if (_hasError || _pendingOperator is null)
            return;

        var currentText = Display.Text ?? DefaultDisplay;
        if (!double.TryParse(currentText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return;

        var result = ApplyOperator(_accumulator, value, _pendingOperator);
        if (!IsValidResult(result))
        {
            ShowError();
            return;
        }

        _accumulator = result;
        Display.Text = FormatNumber(_accumulator);
        _pendingOperator = null;
        _isNewEntry = true;
    }

    private static double ApplyOperator(double left, double right, string op)
    {
        return op switch
        {
            OperatorAdd => left + right,
            OperatorSubtract => left - right,
            OperatorMultiply => left * right,
            OperatorDivide => right == 0 ? double.NaN : left / right,
            _ => right
        };
    }

    private static bool IsValidResult(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    private static string FormatNumber(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return ErrorMessage;

        // Remove trailing zeros and unnecessary decimal point
        var formatted = value.ToString(CultureInfo.InvariantCulture);
        
        if (formatted.Contains('.'))
        {
            formatted = formatted.TrimEnd('0').TrimEnd('.');
        }

        // For very large or very small numbers, use scientific notation
        if (Math.Abs(value) >= 1e15 || (Math.Abs(value) < 1e-10 && value != 0))
        {
            return value.ToString("E", CultureInfo.InvariantCulture);
        }

        return formatted;
    }

    private void ResetCalculator()
    {
        _accumulator = 0;
        _pendingOperator = null;
        _isNewEntry = true;
        _hasError = false;
    }

    private void ClearError()
    {
        _hasError = false;
        ResetCalculator();
    }

    private void ShowError()
    {
        _hasError = true;
        _pendingOperator = null;
        _isNewEntry = true;
        Display.Text = ErrorMessage;
    }
}
