using SimpleCalculator.Core;
using System;
using System.Globalization;

namespace SimpleCalculator.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private double _firstNumber;
        private string? _currentOperation;      // символ текущей операции (+, -, *, /)
        private Func<double, double, double>? _function;
        private string _result = string.Empty;

        public string Result
        {
            get => string.IsNullOrEmpty(_result) ? "0" : _result;
            set => SetProperty(ref _result, value);
        }

        private string _calculationString = string.Empty;
        public string CalculationString
        {
            get => _calculationString;
            set => SetProperty(ref _calculationString, value);
        }

        public RelayCommand ClearCommand { get; }
        public RelayCommand CalculateCommand { get; }
        public RelayCommand<string> SymbolCommand { get; }
        public RelayCommand<string> OperationCommand { get; }
        public RelayCommand SignCommand { get; }
        
        public MainViewModel()
        {
            ClearCommand = new RelayCommand(Clear);
            CalculateCommand = new RelayCommand(Calculate, CanCalculate);
            SymbolCommand = new RelayCommand<string>(AddSymbol, CanAddSymbol);
            OperationCommand = new RelayCommand<string>(SetOperation, CanSetOperation);
            SignCommand = new RelayCommand(SignInverse, ()=>true);
        }

        private void SignInverse()
        {
            Result = (int.Parse(Result) * -1).ToString();
            
        }

        private void Clear()
        {
            _firstNumber = 0;
            _currentOperation = null;
            _function = null;
            Result = string.Empty;

            CalculationString = string.Empty;
        }

        private void AddSymbol(string symbol)
        {
            if (Result == "Error")
                return;

            // Убираем очистку CalculationString отсюда
            // CalculationString = string.Empty; - УДАЛИТЬ ЭТУ СТРОКУ

            if (Result == "0" && symbol != ".")
            {
                Result = symbol;
                return;
            }

            Result += symbol;
        }

        private void SetOperation(string operation)
        {
            // Сохраняем первое число, даже если ввод пуст (тогда считаем его нулём)
            if (!double.TryParse(Result, NumberStyles.Any, CultureInfo.InvariantCulture, out _firstNumber))
            {
                _firstNumber = 0;
            }

            // Обновляем строку вычислений, но не очищаем её полностью
            CalculationString = Result + " " + operation;

            // Очищаем поле для ввода второго числа
            Result = string.Empty;

            _currentOperation = operation;
            _function = operation switch
            {
                "+" => (a, b) => a + b,
                "-" => (a, b) => a - b,
                "*" => (a, b) => a * b,
                "/" => (a, b) => a / b,
                _ => null
            };
        }

        

        private void Calculate()
        {
            if (_function == null || _currentOperation == null)
                return;

            if (!double.TryParse(Result, NumberStyles.Any, CultureInfo.InvariantCulture, out double secondNumber))
            {
                Result = "Error";
                _function = null;
                _currentOperation = null;
                return;
            }

            // Проверка деления на ноль
            if (_currentOperation == "/" && Math.Abs(secondNumber) < double.Epsilon)
            {
                Result = "Error";
                _function = null;
                _currentOperation = null;
                return;
            }

            try
            {
                // Формируем полную строку вычислений
                CalculationString += " " + Result + " =";
                double result = _function(_firstNumber, secondNumber);
                Result = result.ToString(CultureInfo.InvariantCulture);
                _function = null;
                _currentOperation = null;
            }
            catch
            {
                Result = "Error";
                _function = null;
                _currentOperation = null;
            }
        }

        private bool CanAddSymbol(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return false;

            char c = symbol[0];
            if (!char.IsDigit(c) && c != '.')
                return false;

            if (c == '.')
            {
                if (string.IsNullOrEmpty(_result) || _result.Contains('.'))
                    return false;
            }

            return true;
        }

        private bool CanSetOperation(string operation)
        {
            if (Result == "Error")
                return false;
            return operation is "+" or "-" or "*" or "/";
        }

        private bool CanCalculate()
        {
            if (_function == null || _currentOperation == null || Result == "Error")
                return false;
            return double.TryParse(Result, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }
    }
}