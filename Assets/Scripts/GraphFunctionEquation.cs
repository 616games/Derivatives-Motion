using UnityEngine;

[System.Serializable]
public class GraphFunctionEquation
{
    [SerializeField, Tooltip("Required exponent of the function.")]
    private float _exponent;
    public float exponent { get { return _exponent; } set { _exponent = value; } }
    
    [SerializeField, Tooltip("Optional coefficient for the function.")]
    private float _coefficient;
    public float coefficient { get { return _coefficient; } set { _coefficient = value; } }

    [SerializeField, Tooltip("Optional Y-axis intercept of the function (constant).")]
    private float _yIntercept;
    public float yIntercept { get { return _yIntercept; } set { _yIntercept = value; } }
}
