using System;
using System.Collections.Generic;

namespace Formulas {
    public class BinaryOperator {
        private int priority = 0;
        private Func<double, double, double> operation;

        public int Priority => priority;
        
        public static bool TryParse(string formula, ref int startIndex, out BinaryOperator binaryOperator) {
            binaryOperator = null;
            int idx = startIndex;
            while (idx < formula.Length) {
                int value = formula[idx];
                if (value == CharCodes.SPACE) {
                    ++idx;
                }
                foreach (var pair in mapOperators) {
                    if (pair.Key == value) {
                        binaryOperator = pair.Value;
                        startIndex = idx + 1;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        
        public IOperand Apply(IOperand left, IOperand right, IVariableValueProvider valueProvider) {
            double result = operation.Invoke(left.Evaluate(valueProvider), right.Evaluate(valueProvider));
            return new Literal(result);
        }
        
        private static readonly Dictionary<char, BinaryOperator> mapOperators =
            new Dictionary<char, BinaryOperator> {
                { '+',  new BinaryOperator { operation = (a, b) => a + b } },
                { '-',  new BinaryOperator { operation = (a, b) => a - b } },
                { '*',  new BinaryOperator { priority = 1, operation = (a, b) => a * b } },
                { '/',  new BinaryOperator { priority = 1, operation = (a, b) => a / b } },
                { '%',  new BinaryOperator { priority = 1, operation = (a, b) => a % b } }
            };
    }
}