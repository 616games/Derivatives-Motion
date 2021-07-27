using UnityEngine;

public class GraphFunctionLibrary : MonoBehaviour
{
    #region --Custom Methods--
    
    private static Vector3 HorizontalLine(float _xInput, bool _positiveExponent, float _coefficient = 1f, float _yIntercept = 0f)
    {
        float _yInput = _coefficient + _yIntercept;
        return new Vector3(_xInput, _yInput, 0);
    }
    
    /// <summary>
    /// Simulates the graph of a line.
    /// </summary>
    private static Vector3 SlopedLine(float _xInput, bool _positiveExponent, float _coefficient = 1f, float _yIntercept = 0f)
    {
        float _yInput = _coefficient * (_positiveExponent ? _xInput : -_xInput) + _yIntercept;
        return new Vector3(_xInput, _yInput, 0);
    }

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
