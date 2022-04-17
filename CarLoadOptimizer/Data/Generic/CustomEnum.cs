namespace CarLoadOptimizer.Data.Generic
{
    /*
    * Contains the definition for custom enums 
    */

    /// <summary>
    /// Type of charge:
    /// Minimum: charge until direct charging amount is reached
    /// Desired: charge until desired state of charge reached
    /// </summary>
    enum ChargeType
    {
        Minimum,
        Desired
    }
}
