using UnityEngine;

namespace Formulas {
    public class Variable : IOperand {
        public string Name { get; }

        public Variable(string name) {
            Name = name;
        }
        
        public static bool TryParse(string formula, ref int startIndex, out Variable variable) {
            variable = null;
            int idx = startIndex;
            bool hasParsedFirstLetter = false;
            while (idx < formula.Length) {
                char character = formula[idx];
                if (hasParsedFirstLetter && character >= CharCodes.ZERO && character <= CharCodes.NINE) {
                    ++idx;
                } else if (CharCodes.mapAllowedSymbols.Contains(character)) {
                    hasParsedFirstLetter = true;
                    ++idx;
                } else {
                    break;
                }
            }
            if (startIndex == idx) {
                return false;
            }
            variable = new Variable(formula.Substring(startIndex, idx - startIndex));
            startIndex = idx;
            return true;
        }

        public double Evaluate(IVariableValueProvider valueProvider) {
            if (valueProvider.GetVariableValue(Name, out double value)) {
                return value;
            }
            Debug.LogError($"Failed to evaluate variable: '{Name}'");
            return 0d;
        }
    }
}