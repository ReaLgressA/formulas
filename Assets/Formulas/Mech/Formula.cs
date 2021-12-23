using System;
using System.Collections;
using JsonParser;
using UnityEngine;

namespace Formulas.Mech {
    public class Formula : IJsonInterface {
        private Expression expression;
        
        public Expression Expression => expression;
        public ApplicationMethod ApplicationMethod { get; set; } = ApplicationMethod.AddValue;
        
        public Formula() { }

        public Formula(string formula) {
            if (!Expression.Parse(formula, out expression)) {
                expression = null;
            }
        }

        public double Evaluate(IVariableValueProvider valueProvider) {
            return Expression.Evaluate(valueProvider);
        }

        public void ToJsonObject(Hashtable ht) {
            throw new NotImplementedException("Formula::ToJsonObject is not implemented!");
        }

        public void FromJson(Hashtable ht, bool isAddition = false) {
            string formula = ht.GetStringSafe(Keys.FORMULA);
            if (string.IsNullOrEmpty(formula)) {
                Debug.LogError($"Can't create formula from '{formula}'!");
            } else if (!Expression.Parse(formula, out expression)) {
                expression = null;
            }
            ApplicationMethod = ht.GetEnum(Keys.APPLICATION_METHOD, ApplicationMethod);
        }

        private static class Keys {
            public const string FORMULA = "Formula";
            public const string APPLICATION_METHOD = "ApplicationMethod";
        }
    }
}