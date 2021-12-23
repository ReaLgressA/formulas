using System;

namespace Formulas.Mech {
    [Flags]
    public enum ApplicationMethod {
        None = 0,
        AddValue = 1,
        AddPercent = 2,
        MultiplyByValue = 4,
        AddPercentAndMultiply = AddPercent | MultiplyByValue,
        All = AddValue | AddPercent | MultiplyByValue,
    }
}