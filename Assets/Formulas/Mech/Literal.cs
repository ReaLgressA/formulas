namespace Formulas {
    public class Literal : IOperand {
        public double Value { get; }
        
        public Literal(double value) {
            Value = value;
        }

        public double Evaluate(IVariableValueProvider valueProvider) {
            return Value;
        }

        public static bool TryParse(string formula, ref int startIndex, out Literal literal) {
            literal = null;
            int idx = startIndex;
            bool hasReachedDot = false;
            bool hasParsedDigit = false;
            bool hasParsedLeadingMinus = false;
            while (idx < formula.Length) {
                int value = formula[idx];
                if (!hasParsedDigit && !hasParsedLeadingMinus && value == CharCodes.SPACE) {
                    ++idx;
                } else if (!hasParsedDigit && value == CharCodes.MINUS) {
                    if (hasParsedLeadingMinus) {
                        return false;
                    }
                    hasParsedLeadingMinus = true;
                    ++idx;
                } else if (value == CharCodes.DOT) {
                    if (hasReachedDot) {
                        return false;
                    }
                    hasReachedDot = true;
                    ++idx;
                } else if (value >= CharCodes.ZERO && value <= CharCodes.NINE) {
                    hasParsedDigit = true;
                    ++idx;
                } else {
                    break;
                }
            }
            if (startIndex == idx) {
                return false;
            }
            literal = new Literal(double.Parse(formula.Substring(startIndex, idx - startIndex)));
            startIndex = idx;
            return true;
        }
    }
}