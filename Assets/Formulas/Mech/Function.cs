using System;
using System.Collections.Generic;
using UnityEngine;

namespace Formulas {
    public class Function : IOperand {
        public string Name { get; }
        public List<IOperand> Arguments { get; }

        public Function(string name, List<IOperand> arguments) {
            Name = name;
            Arguments = arguments;
        }
        
        public static bool TryParse(string formula, ref int startIndex, out Function function) {
            function = null;
            List<IOperand> arguments = null;
            string functionName = null;
            int idx = startIndex;
            bool hasParsedFirstLetter = false;
            while (idx < formula.Length) {
                char character = formula[idx];
                if (hasParsedFirstLetter && character >= CharCodes.ZERO && character <= CharCodes.NINE) {
                    ++idx;
                } else if (CharCodes.mapAllowedSymbols.Contains(character)) {
                    hasParsedFirstLetter = true;
                    ++idx;
                } else if (hasParsedFirstLetter && character == CharCodes.SPACE) {
                    ++idx;
                } else if (hasParsedFirstLetter && character == CharCodes.PARENTHESIS_OPEN) {
                    functionName = formula.Substring(startIndex, idx - startIndex);
                    if (!TryParseArguments(formula, ref idx, out arguments)) {
                        return false;
                    }
                    break;
                } else {
                    return false;
                }
            }
            if (string.IsNullOrWhiteSpace(functionName) || startIndex == idx) {
                return false;
            }
            function = new Function(functionName, arguments);
            startIndex = idx;
            return true;
        }

        private static bool TryParseArguments(string formula, ref int startIndex, out List<IOperand> arguments) {
            arguments = new List<IOperand>();
            int idx = startIndex;
            bool hasParsedOpenParenthesis = false;
            int openedParenthesisCount = 0;
            while (idx < formula.Length) {
                char character = formula[idx];
                if (character == CharCodes.SPACE) {
                    ++idx;
                } else if (character == CharCodes.PARENTHESIS_OPEN) {
                    ++idx;
                    int lastCommaIndex = idx;
                    openedParenthesisCount = 1;
                    while (idx < formula.Length && openedParenthesisCount > 0) {
                        character = formula[idx];
                        if (character == CharCodes.COMMA && openedParenthesisCount == 1) {
                            string expressionText = formula.Substring(lastCommaIndex, idx - lastCommaIndex);
                            if (expressionText.Length == 0) {
                                Debug.LogError($"Failed to parse expression for argument #{arguments.Count}: argument can't be empty!");
                                return false;
                            }
                            if (!Expression.Parse(expressionText, out Expression expression))
                            {
                                Debug.LogError($"Failed to parse expression '{expressionText}' for argument #{arguments.Count}");
                                return false;
                            }
                            arguments.Add(expression);
                            lastCommaIndex = idx + 1;
                        } else if (character == CharCodes.PARENTHESIS_OPEN) {
                            ++openedParenthesisCount;
                        } else if (character == CharCodes.PARENTHESIS_CLOSE) {
                            --openedParenthesisCount;
                            if (openedParenthesisCount == 0) {
                                string expressionText = formula.Substring(lastCommaIndex, idx - lastCommaIndex);
                                if (expressionText.Length == 0) {
                                    Debug.LogError($"Failed to parse expression for argument #{arguments.Count}: argument can't be empty!");
                                    return false;
                                }

                                if (!Expression.Parse(expressionText, out Expression expression)) {
                                    Debug.LogError($"Failed to parse expression '{expressionText}' for argument #{arguments.Count}");
                                    return false;
                                }
                                arguments.Add(expression);
                            }
                        }
                        ++idx;
                    }
                    startIndex = idx;
                    return true;
                }
            }
            return false;
        }

        public double Evaluate(IVariableValueProvider valueProvider) {
            try {
                if (mapFunctions.TryGetValue(Name, out var function)) {
                    return function.Invoke(Arguments, valueProvider);
                }
            } catch (Exception ex) {
                Debug.LogError($"Failed to evaluate function call: '{Name}' with {Arguments.Count} arguments!\nException:{ex.StackTrace}");
            }
            Debug.LogError($"Failed to evaluate function call: '{Name}' with {Arguments.Count} arguments!");
            return 0d;
        }
        
        private static readonly Dictionary<string, Func<List<IOperand>, IVariableValueProvider, double>> mapFunctions =
            new Dictionary<string, Func<List<IOperand>, IVariableValueProvider, double>> {
                { "MIN", (args, vars) =>
                    Math.Min(args[0].Evaluate(vars), args[1].Evaluate(vars)) },
                { "MAX", (args, vars) =>
                    Math.Max(args[0].Evaluate(vars), args[1].Evaluate(vars)) },
                { "POW", (args, vars) =>
                    Math.Pow(args[0].Evaluate(vars), args[1].Evaluate(vars)) },
                { "SQRT", (args, vars) =>
                    Math.Sqrt(args[0].Evaluate(vars)) },
                { "ROUND", (args, vars) =>
                    Math.Round(args[0].Evaluate(vars)) },
                { "FLOOR", (args, vars) =>
                    Math.Floor(args[0].Evaluate(vars)) },
                { "CEILING", (args, vars) =>
                    Math.Ceiling(args[0].Evaluate(vars)) },
                { "ABS", (args, vars) =>
                    Math.Abs(args[0].Evaluate(vars)) },
                { "NEG", (args, vars) =>
                    -(args[0].Evaluate(vars)) }
            };
    }
}