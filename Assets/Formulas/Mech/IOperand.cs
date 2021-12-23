namespace Formulas {
    public interface IOperand {
        public double Evaluate(IVariableValueProvider valueProvider);
    }
}