namespace Formulas {
    public interface IVariableValueProvider {
        bool GetVariableValue(string variableName, out double value);
    }
}