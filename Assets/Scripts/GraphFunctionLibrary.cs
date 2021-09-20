using UnityEngine;

public class GraphFunctionLibrary : MonoBehaviour
{
    #region --Custom Methods--
    
    /// <summary>
    /// Helper method for parametric functions.
    /// </summary>
    public static Vector3 GraphFunction(float _xInput,float _exponent, float _coefficient = 1f)
    {
        float _yInput = _coefficient * Mathf.Pow(_xInput, _exponent);
        return new Vector3(_xInput, _yInput, 0);
    }
    
    #endregion
    
}
