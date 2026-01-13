using System;
using System.Globalization;

namespace CalculatorApp;

/// <summary>
/// Core calculator logic that can be tested independently of UI components.
/// </summary>
public class Calculator
{
    public const string OperatorAdd = "add";
    public const string OperatorSubtract = "subtract";
    public const string OperatorMultiply = "multiply";
    public const string OperatorDivide = "divide";
    public const string ErrorMessage = "Error";
    public const string DefaultDisplay = "0";

    private double _accumulator;
    private string? _pendingOperator;
    private bool _isNewEntry = true;
    private bool _hasError;

    public double Accumulator => _accumulator;
    public string? PendingOperator => _pendingOperator;
    public bool IsNewEntry => _isNewEntry;
    public bool HasError => _hasError;

    /// <summary>
    /// Processes a digit input and returns the display text.
    /// </summary>
    public string ProcessDigit(string digit, string currentDisplay)
    {
        if (_hasError)
        {
            ClearError();
            currentDisplay = DefaultDisplay;
        }

        if (string.IsNullOrEmpty(digit))
            return currentDisplay;

        if (_isNewEntry || currentDisplay == DefaultDisplay)
        {
            _isNewEntry = false;
            return digit;
        }

        return currentDisplay + digit;
    }

    /// <summary>
    /// Processes a decimal point input and returns the display text.
    /// </summary>
    public string ProcessDecimal(string currentDisplay)
    {
        if (_hasError)
        {
            ClearError();
            currentDisplay = DefaultDisplay;
        }

        if (_isNewEntry)
        {
            _isNewEntry = false;
            return "0.";
        }

        if (!currentDisplay.Contains('.'))
        {
            return currentDisplay + ".";
        }

        return currentDisplay;
    }

    /// <summary>
    /// Processes a backspace input and returns the display text.
    /// </summary>
    public string ProcessBackspace(string currentDisplay)
    {
        if (_hasError)
        {
            ClearError();
            return DefaultDisplay;
        }

        if (currentDisplay.Length > 1 && currentDisplay != DefaultDisplay)
        {
            _isNewEntry = false;
            return currentDisplay[..^1];
        }

        _isNewEntry = true;
        return DefaultDisplay;
    }

    /// <summary>
    /// Clears the calculator and returns the default display.
    /// </summary>
    public string Clear()
    {
        ResetCalculator();
        return DefaultDisplay;
    }

    /// <summary>
    /// Toggles the sign of the current display value and returns the new display text.
    /// </summary>
    public string ToggleSign(string currentDisplay)
    {
        if (_hasError)
            return currentDisplay;

        if (double.TryParse(currentDisplay, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            value = -value;
            return FormatNumber(value);
        }

        return currentDisplay;
    }

    /// <summary>
    /// Converts the current display value to a percentage and returns the new display text.
    /// </summary>
    public string ProcessPercent(string currentDisplay)
    {
        if (_hasError)
            return currentDisplay;

        if (double.TryParse(currentDisplay, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            value /= 100.0;
            _isNewEntry = true;
            return FormatNumber(value);
        }

        return currentDisplay;
    }

    /// <summary>
    /// Processes an operator input and returns the display text.
    /// </summary>
    public string ProcessOperator(string op, string currentDisplay)
    {
        if (_hasError || string.IsNullOrEmpty(op))
            return currentDisplay;

        if (double.TryParse(currentDisplay, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            if (_pendingOperator is not null && !_isNewEntry)
            {
                var result = ApplyOperator(_accumulator, value, _pendingOperator);
                if (!IsValidResult(result))
                {
                    ShowError();
                    return ErrorMessage;
                }
                _accumulator = result;
            }
            else
            {
                _accumulator = value;
            }
        }

        _pendingOperator = op;
        _isNewEntry = true;
        return FormatNumber(_accumulator);
    }

    /// <summary>
    /// Processes the equals operation and returns the display text.
    /// </summary>
    public string ProcessEquals(string currentDisplay)
    {
        if (_hasError || _pendingOperator is null)
            return currentDisplay;

        if (!double.TryParse(currentDisplay, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return currentDisplay;

        var result = ApplyOperator(_accumulator, value, _pendingOperator);
        if (!IsValidResult(result))
        {
            ShowError();
            return ErrorMessage;
        }

        _accumulator = result;
        _pendingOperator = null;
        _isNewEntry = true;
        return FormatNumber(_accumulator);
    }

    /// <summary>
    /// Applies the specified operator to two operands.
    /// </summary>
    public static double ApplyOperator(double left, double right, string op)
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

    /// <summary>
    /// Validates if a calculation result is valid (not NaN or Infinity).
    /// </summary>
    public static bool IsValidResult(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    /// <summary>
    /// Formats a number for display, removing trailing zeros and handling scientific notation.
    /// </summary>
    public static string FormatNumber(double value)
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
    }
}
