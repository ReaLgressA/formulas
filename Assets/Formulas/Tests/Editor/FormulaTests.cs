using System;
using System.Collections.Generic;
using Formulas.Mech;
using NUnit.Framework;

namespace Formulas.Tests.Editor {
    public class FormulaTests {
        private IVariableValueProvider variableProvider;
        
        [SetUp]
        public void Setup() {
            variableProvider = new TestValueProvider(new Dictionary<string, Func<double>> {
                { "VarA", () => 1d },
                { "VarB", () => 2d },
                { "var_C10", () => 10d },
                { "var_20D", () => 20d },
                { "v69_x", () => 69d },
                { "v0_52x", () => 0.52d },
                { "neg_100", () => -100d }
            });
        }
        
        [Test]
        [TestCase("0", 1)]
        [TestCase("5", 1)]
        [TestCase("9", 1)]
        [TestCase("2 3", 2)]
        [TestCase("4 6 7 8", 4)]
        [TestCase("467 128 13", 3)]
        [TestCase("12.3 11.1230 3 122.52123", 4)]
        [TestCase("-12.3 -11.1230 -3 -122.52123", 4)]
        public void ParseLiterals(string formula, int literalCount) {
            Formula f = new Formula(formula);
            Assert.AreEqual(f.Expression.Operands.Count, literalCount);
        }
        
        [Test]
        [TestCase("1 + 1", 1)]
        [TestCase("1 + 1 - 2", 2)]
        [TestCase("2+345-2123.3", 2)]
        [TestCase("2+345-2123.3 / 15 + 20 * 32", 5)]
        public void ParseOperators(string formula, int operatorCount) {
            Formula f = new Formula(formula);
            Assert.AreEqual(operatorCount, f.Expression.Operators.Count);
        }

        [Test]
        [TestCase("420.69", 420.69d)]
        [TestCase("3 + 5", 3d + 5d)]
        [TestCase("1 + 2 + 3", 1d + 2d + 3d)]
        [TestCase("1000 + 20000 + 30000", 1000d + 20000d + 30000d)]
        [TestCase("3 * 5", 3d * 5d)]
        [TestCase("1 * 2 * 3", 1d * 2d * 3d)]
        [TestCase("1000 * 20000 * 30000", 1000d * 20000d * 30000d)]
        [TestCase("2 + 3 * 5 - 4", 2 + 3 * 5 - 4)]
        [TestCase("7 * 2 + 5 * 3 + 1", 7 * 2 + 5 * 3 + 1)]
        [TestCase("7 * 2 + 5 * 3 + 1 / 10 * 3.2", 7 * 2 + 5 * 3 + 1d / 10d * 3.2)]
        public void Arithmetics(string formula, double expectedResult) {
            double result = new Formula(formula).Evaluate(variableProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }

        [Test]
        [TestCase("VarA", 1)]
        [TestCase("VarA + VarB", 2)]
        [TestCase("a + b - c * E", 4)]
        [TestCase("A_X / B_Y - Z_X / N_A", 4)]
        [TestCase("A1 + A2 + B3 + C4 + D_01_A0", 5)]
        public void ParseVariables(string formula, double variablesCount) {
            Formula f = new Formula(formula);
            Assert.AreEqual(variablesCount, f.Expression.Operands.Count);
        }

        [Test]
        [TestCase("VarA", 1d)]
        [TestCase("VarA + VarB", 1d + 2d)]
        [TestCase("VarB * var_C10 + VarA", 2d * 10d + 1d)]
        [TestCase("v69_x - VarB * var_C10 + VarA / var_20D", 69d - 2d * 10d + 1d / 20d)]
        public void ArithmeticsWithVariables(string formula, double expectedResult) {
            double result = new Formula(formula).Evaluate(variableProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }
        
        [Test]
        [TestCase("MAX(VarA, VarB) / ABS(neg_100)", 0.02d)]
        [TestCase("POW(VarB, var_C10) + VarB", 1026)]
        [TestCase("VarB * var_C10 + ROUND(v0_52x)", 2d * 10d + 1d)]
        [TestCase("MAX( CEILING(v0_52x), FLOOR(v0_52x) ) + 32", 1d + 32d)]
        public void ArithmeticsWithFunctions(string formula, double expectedResult) {
            double result = new Formula(formula).Evaluate(variableProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }

        [Test]
        [TestCase("MIN(0, 1)", 0)]
        [TestCase("MIN(9999, 1)", 1)]
        [TestCase("MIN(256, 256)", 256)]
        [TestCase("MIN(-1000, 121)", -1000)]
        [TestCase("MIN(-34, -121)", -121)]
        [TestCase("MAX(12, 52)", 52)]
        [TestCase("MAX(10, 10)", 10d)]
        [TestCase("MAX(-20, -1)", -1d)]
        [TestCase("MAX(-20, -20)", -20d)]
        [TestCase("POW(2, 3)", 2 * 2 * 2)]
        [TestCase("POW(2, -3)", 0.125d)]
        [TestCase("POW(2, 0)", 1d)]
        [TestCase("SQRT(4)", 2)]
        [TestCase("ROUND(4.1)", 4d)]
        [TestCase("ROUND(4.9)", 5d)]
        [TestCase("CEILING(4.2)", 5d)]
        [TestCase("FLOOR(4.2)", 4d)]
        [TestCase("ABS(3)", 3d)]
        [TestCase("ABS(-3)", 3d)]
        public void Functions(string formula, double expectedResult) {
            double result = new Formula(formula).Evaluate(variableProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }

        [Test]
        [TestCase(4, 1, 3d)]
        public void ArchetypeSwordsman(int unitRank, int targetRank, double expectedResult) {
            var testProvider = new TestValueProvider(new Dictionary<string, Func<double>> {
                { "TargetRank", () => targetRank },
                { "UnitRank", () => unitRank }
            });
            string formula = "MAX(UnitRank - TargetRank, 0)";
            double result = new Formula(formula).Evaluate(testProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }
        
        [Test]
        [TestCase(4, 1, -3d)]
        public void ArchetypeHeavyCavalry(int unitRank, int targetRank, double expectedResult) {
            var testProvider = new TestValueProvider(new Dictionary<string, Func<double>> {
                { "TargetRank", () => targetRank },
                { "UnitRank", () => unitRank }
            });
            string formula = "NEG(MAX(UnitRank - TargetRank, 0))";
            double result = new Formula(formula).Evaluate(testProvider);
            Assert.AreEqual(expectedResult, result, double.Epsilon);
        }

        private class TestValueProvider : IVariableValueProvider {
            private readonly Dictionary<string, Func<double>> valueProviders;
            
            public TestValueProvider(Dictionary<string, Func<double>> valueProviders) {
                this.valueProviders = valueProviders;
            }

            public bool GetVariableValue(string variableName, out double value) {
                value = 0d;
                if (valueProviders.TryGetValue(variableName, out Func<double> variableValueGetter)) {
                    value = variableValueGetter.Invoke();
                    return true;
                }
                return false;
            }
        }
    }
}
