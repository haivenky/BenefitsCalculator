namespace Api.Services
{
    /// <summary>
    /// Constants for various cost calculations
    /// </summary>
    public static class Constants
    {
        public const decimal BaseCostPerMonth = 1000m;
        public const decimal DependentCostPerMonth = 600m;
        public const decimal HighSalaryThreshold = 80000m;
        public const decimal HighSalaryAdditionalCostRate = 0.02m;
        public const int PaychecksPerYear = 26;
        public const int MonthsInYear = 12;
        public const int DependentAgeThreshold = 50;
        public const decimal AdditionalCostForOldDependents = 200m;
    }
}
