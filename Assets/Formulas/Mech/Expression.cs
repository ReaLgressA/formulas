using System.Collections.Generic;
using UnityEngine;

namespace Formulas {
    public class Expression : IOperand {
        public List<IOperand> Operands { get; } = new List<IOperand>();
        public List<BinaryOperator> Operators { get; } = new List<BinaryOperator>();
        
        public static bool Parse(string formula, out Expression expression) {
            expression = new Expression();
            int index = 0;
            while (index < formula.Length) {
                if (formula[index] == ' ') {
                    ++index;
                    continue;
                }
                if (expression.Operands.Count > 0 && BinaryOperator.TryParse(formula, ref index,
                                                                             out BinaryOperator binaryOperator)) {
                    expression.Operators.Add(binaryOperator);   
                } else if (Function.TryParse(formula, ref index, out Function function)) {
                    expression.Operands.Add(function);
                } else if (Literal.TryParse(formula, ref index, out Literal literal)) {
                    expression.Operands.Add(literal);
                } else if (Variable.TryParse(formula, ref index, out Variable variable)) {
                    expression.Operands.Add(variable);
                } else {
                    return false;
                }
            }
            return true;
        }
        
        public double Evaluate(IVariableValueProvider valueProvider) {
            Stack<IOperand> operands = new Stack<IOperand>();
            Stack<BinaryOperator> operators = new Stack<BinaryOperator>();
            if (Operators.Count == 0) {
                return Operands[0].Evaluate(valueProvider);
            }
            operands.Push(new Literal(Operands[0].Evaluate(valueProvider)));
            operands.Push(new Literal(Operands[1].Evaluate(valueProvider)));
            int nextOperandIndex = 2;
            operators.Push(Operators[0]);
            for (int operatorIndex = 1; operatorIndex < Operators.Count; ++operatorIndex) {
                BinaryOperator nextOperator = Operators[operatorIndex];
                BinaryOperator lastOperator = operators.Peek();
                while (operands.Count > 1 && lastOperator.Priority >= nextOperator.Priority) {
                    IOperand right = operands.Pop();
                    IOperand left = operands.Pop();
                    lastOperator = operators.Pop();
                    operands.Push(lastOperator.Apply(left, right, valueProvider));
                    if (operators.Count == 0) {
                        break;                        
                    }
                    lastOperator = operators.Peek();
                }
                if (nextOperandIndex >= Operands.Count) {
                    Debug.LogError("Not enough operands in expression!");
                    return 0f;
                }
                operands.Push(new Literal(Operands[nextOperandIndex++].Evaluate(valueProvider)));
                operators.Push(nextOperator);
            }
            while (operators.Count > 0) {
                IOperand right = operands.Pop();
                IOperand left = operands.Pop();
                BinaryOperator lastOperator = operators.Pop();
                operands.Push(lastOperator.Apply(left, right, valueProvider));
            }
            return operands.Pop().Evaluate(valueProvider);
        }
    }
}