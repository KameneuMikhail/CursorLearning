using CalculatorApp;

namespace CalculatorApp.Tests;

public class CalculatorTests
{
    [Fact]
    public void ProcessDigit_WithNewEntry_ReturnsDigit()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = Calculator.DefaultDisplay;

        // Act
        var result = calculator.ProcessDigit("5", currentDisplay);

        // Assert
        Assert.Equal("5", result);
        Assert.False(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessDigit_WithExistingDisplay_AppendsDigit()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessDigit("1", Calculator.DefaultDisplay);
        calculator.ProcessDigit("2", "1");
        var currentDisplay = "12";

        // Act
        var result = calculator.ProcessDigit("3", currentDisplay);

        // Assert
        Assert.Equal("123", result);
    }

    [Fact]
    public void ProcessDigit_WithZeroDisplay_ReplacesZero()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "0";

        // Act
        var result = calculator.ProcessDigit("7", currentDisplay);

        // Assert
        Assert.Equal("7", result);
    }

    [Fact]
    public void ProcessDigit_WithError_ClearsError()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0"); // This will cause division by zero error

        // Act
        var result = calculator.ProcessDigit("5", Calculator.ErrorMessage);

        // Assert
        Assert.Equal("5", result);
        Assert.False(calculator.HasError);
    }

    [Fact]
    public void ProcessDecimal_WithNewEntry_ReturnsZeroPoint()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = Calculator.DefaultDisplay;

        // Act
        var result = calculator.ProcessDecimal(currentDisplay);

        // Assert
        Assert.Equal("0.", result);
        Assert.False(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessDecimal_WithoutDecimal_AddsDecimalPoint()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessDigit("1", Calculator.DefaultDisplay);
        calculator.ProcessDigit("2", "1");
        calculator.ProcessDigit("3", "12");
        var currentDisplay = "123";

        // Act
        var result = calculator.ProcessDecimal(currentDisplay);

        // Assert
        Assert.Equal("123.", result);
    }

    [Fact]
    public void ProcessDecimal_WithExistingDecimal_DoesNotAddAnother()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessDigit("1", Calculator.DefaultDisplay);
        calculator.ProcessDigit("2", "1");
        calculator.ProcessDecimal("12");
        calculator.ProcessDigit("5", "12.");
        var currentDisplay = "12.5";

        // Act
        var result = calculator.ProcessDecimal(currentDisplay);

        // Assert
        Assert.Equal("12.5", result);
    }

    [Fact]
    public void ProcessBackspace_WithMultipleDigits_RemovesLastDigit()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "123";

        // Act
        var result = calculator.ProcessBackspace(currentDisplay);

        // Assert
        Assert.Equal("12", result);
        Assert.False(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessBackspace_WithSingleDigit_ReturnsZero()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "5";

        // Act
        var result = calculator.ProcessBackspace(currentDisplay);

        // Assert
        Assert.Equal(Calculator.DefaultDisplay, result);
        Assert.True(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessBackspace_WithError_ClearsError()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0");

        // Act
        var result = calculator.ProcessBackspace(Calculator.ErrorMessage);

        // Assert
        Assert.Equal(Calculator.DefaultDisplay, result);
        Assert.False(calculator.HasError);
    }

    [Fact]
    public void Clear_ResetsCalculator()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessDigit("5", Calculator.DefaultDisplay);
        calculator.ProcessOperator(Calculator.OperatorAdd, "5");

        // Act
        var result = calculator.Clear();

        // Assert
        Assert.Equal(Calculator.DefaultDisplay, result);
        Assert.Equal(0, calculator.Accumulator);
        Assert.Null(calculator.PendingOperator);
        Assert.True(calculator.IsNewEntry);
        Assert.False(calculator.HasError);
    }

    [Fact]
    public void ToggleSign_PositiveNumber_ReturnsNegative()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "5";

        // Act
        var result = calculator.ToggleSign(currentDisplay);

        // Assert
        Assert.Equal("-5", result);
    }

    [Fact]
    public void ToggleSign_NegativeNumber_ReturnsPositive()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "-5";

        // Act
        var result = calculator.ToggleSign(currentDisplay);

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void ToggleSign_WithError_DoesNotChangeDisplay()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0");

        // Act
        var result = calculator.ToggleSign(Calculator.ErrorMessage);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void ProcessPercent_ConvertsToPercentage()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "50";

        // Act
        var result = calculator.ProcessPercent(currentDisplay);

        // Assert
        Assert.Equal("0.5", result);
        Assert.True(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessPercent_WithError_DoesNotChangeDisplay()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0");

        // Act
        var result = calculator.ProcessPercent(Calculator.ErrorMessage);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void ProcessOperator_FirstOperator_SetsAccumulator()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "5";

        // Act
        var result = calculator.ProcessOperator(Calculator.OperatorAdd, currentDisplay);

        // Assert
        Assert.Equal("5", result);
        Assert.Equal(5, calculator.Accumulator);
        Assert.Equal(Calculator.OperatorAdd, calculator.PendingOperator);
        Assert.True(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessOperator_WithPendingOperator_AppliesOperation()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorAdd, "5");
        // Simulate user entering "3" after operator
        calculator.ProcessDigit("3", Calculator.DefaultDisplay);
        var currentDisplay = "3";

        // Act
        var result = calculator.ProcessOperator(Calculator.OperatorMultiply, currentDisplay);

        // Assert
        Assert.Equal("8", result);
        Assert.Equal(8, calculator.Accumulator);
        Assert.Equal(Calculator.OperatorMultiply, calculator.PendingOperator);
    }

    [Fact]
    public void ProcessOperator_WithError_DoesNotProcess()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0");

        // Act
        var result = calculator.ProcessOperator(Calculator.OperatorAdd, Calculator.ErrorMessage);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void ProcessEquals_AddOperation_ReturnsCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorAdd, "5");
        var currentDisplay = "3";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal("8", result);
        Assert.Equal(8, calculator.Accumulator);
        Assert.Null(calculator.PendingOperator);
        Assert.True(calculator.IsNewEntry);
    }

    [Fact]
    public void ProcessEquals_SubtractOperation_ReturnsCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorSubtract, "10");
        var currentDisplay = "3";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal("7", result);
    }

    [Fact]
    public void ProcessEquals_MultiplyOperation_ReturnsCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorMultiply, "5");
        var currentDisplay = "4";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal("20", result);
    }

    [Fact]
    public void ProcessEquals_DivideOperation_ReturnsCorrectResult()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "20");
        var currentDisplay = "4";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void ProcessEquals_DivisionByZero_ReturnsError()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        var currentDisplay = "0";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
        Assert.True(calculator.HasError);
        Assert.Null(calculator.PendingOperator);
    }

    [Fact]
    public void ProcessEquals_WithError_DoesNotProcess()
    {
        // Arrange
        var calculator = new Calculator();
        calculator.ProcessOperator(Calculator.OperatorDivide, "10");
        calculator.ProcessEquals("0");

        // Act
        var result = calculator.ProcessEquals(Calculator.ErrorMessage);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void ProcessEquals_WithoutPendingOperator_ReturnsCurrentDisplay()
    {
        // Arrange
        var calculator = new Calculator();
        var currentDisplay = "5";

        // Act
        var result = calculator.ProcessEquals(currentDisplay);

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void ApplyOperator_Add_ReturnsSum()
    {
        // Act
        var result = Calculator.ApplyOperator(5, 3, Calculator.OperatorAdd);

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public void ApplyOperator_Subtract_ReturnsDifference()
    {
        // Act
        var result = Calculator.ApplyOperator(10, 3, Calculator.OperatorSubtract);

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public void ApplyOperator_Multiply_ReturnsProduct()
    {
        // Act
        var result = Calculator.ApplyOperator(5, 4, Calculator.OperatorMultiply);

        // Assert
        Assert.Equal(20, result);
    }

    [Fact]
    public void ApplyOperator_Divide_ReturnsQuotient()
    {
        // Act
        var result = Calculator.ApplyOperator(20, 4, Calculator.OperatorDivide);

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public void ApplyOperator_DivideByZero_ReturnsNaN()
    {
        // Act
        var result = Calculator.ApplyOperator(10, 0, Calculator.OperatorDivide);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    [Fact]
    public void ApplyOperator_UnknownOperator_ReturnsRightOperand()
    {
        // Act
        var result = Calculator.ApplyOperator(5, 3, "unknown");

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public void IsValidResult_ValidNumber_ReturnsTrue()
    {
        // Act
        var result = Calculator.IsValidResult(5.0);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidResult_NaN_ReturnsFalse()
    {
        // Act
        var result = Calculator.IsValidResult(double.NaN);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidResult_PositiveInfinity_ReturnsFalse()
    {
        // Act
        var result = Calculator.IsValidResult(double.PositiveInfinity);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidResult_NegativeInfinity_ReturnsFalse()
    {
        // Act
        var result = Calculator.IsValidResult(double.NegativeInfinity);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FormatNumber_Integer_ReturnsIntegerString()
    {
        // Act
        var result = Calculator.FormatNumber(5.0);

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void FormatNumber_Decimal_RemovesTrailingZeros()
    {
        // Act
        var result = Calculator.FormatNumber(5.50);

        // Assert
        Assert.Equal("5.5", result);
    }

    [Fact]
    public void FormatNumber_WithTrailingZeros_RemovesThem()
    {
        // Act
        var result = Calculator.FormatNumber(12.300);

        // Assert
        Assert.Equal("12.3", result);
    }

    [Fact]
    public void FormatNumber_NaN_ReturnsErrorMessage()
    {
        // Act
        var result = Calculator.FormatNumber(double.NaN);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void FormatNumber_Infinity_ReturnsErrorMessage()
    {
        // Act
        var result = Calculator.FormatNumber(double.PositiveInfinity);

        // Assert
        Assert.Equal(Calculator.ErrorMessage, result);
    }

    [Fact]
    public void FormatNumber_VeryLargeNumber_UsesScientificNotation()
    {
        // Act
        var result = Calculator.FormatNumber(1e16);

        // Assert
        Assert.Contains("E", result);
    }

    [Fact]
    public void FormatNumber_VerySmallNumber_UsesScientificNotation()
    {
        // Act
        var result = Calculator.FormatNumber(1e-11);

        // Assert
        Assert.Contains("E", result);
    }

    [Fact]
    public void FormatNumber_Zero_ReturnsZero()
    {
        // Act
        var result = Calculator.FormatNumber(0.0);

        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void FormatNumber_NegativeNumber_ReturnsNegativeString()
    {
        // Act
        var result = Calculator.FormatNumber(-5.5);

        // Assert
        Assert.Equal("-5.5", result);
    }

    [Theory]
    [InlineData(Calculator.OperatorAdd, 10, 5, 15)]
    [InlineData(Calculator.OperatorSubtract, 10, 5, 5)]
    [InlineData(Calculator.OperatorMultiply, 10, 5, 50)]
    [InlineData(Calculator.OperatorDivide, 10, 5, 2)]
    public void ApplyOperator_VariousOperations_ReturnsExpectedResult(string op, double left, double right, double expected)
    {
        // Act
        var result = Calculator.ApplyOperator(left, right, op);

        // Assert
        Assert.Equal(expected, result);
    }
}
