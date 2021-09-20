using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    #region --Fields / Properties--

    [SerializeField, Tooltip("Global counter to monitor when all graphs should be reset.")]
    private Counter _resetCounter;

    [SerializeField, Tooltip("Y position desired to stop the graph from drawing when it passes this value.")]
    private float _yPositionBoundary;

    [SerializeField, Tooltip("The original equation for this graph to which any derivatives will be created from.")]
    private GraphFunctionEquation _originalFunction;

    [SerializeField, Tooltip("The number of derivatives to be taken of the _originalFunction (n).")]
    private int _derivativeCount;
    
    [SerializeField, Tooltip("The nth derivative of the _originalFunction.")]
    private GraphFunctionEquation _derivativeFunction;
    
    [SerializeField, Tooltip("How fast the graph should be drawn.")]
    private float _graphSpeed = 1;

    [SerializeField, Tooltip("Desired color of the graph's line renderer.")]
    private Color _lineRendererColor;

    /// <summary>
    /// Keeps track of the change in value of the _resetCounter to determine when a reset should occur.
    /// Reset should occur when the _resetCounter stops incrementing (when _previousCount = _resetCounter).
    /// </summary>
    private int _previousCount;

    /// <summary>
    /// Cached Transform component.
    /// </summary>
    private Transform _transform;

    /// <summary>
    /// Current position of the graphing node (this game object).
    /// </summary>
    private Vector3 _movePosition;

    /// <summary>
    /// Tracks if the graph is performing a reset.
    /// </summary>
    private bool _isResetting;

    /// <summary>
    /// Input value (x in f(x)) for the graph function.
    /// </summary>
    private float _input;

    /// <summary>
    /// How long to wait between ResetGraph calls.
    /// </summary>
    private float _resetDelayTime = 1f;
    
    /// <summary>
    /// Tracks the time elapsed for the _resetDelayTime.
    /// </summary>
    private float _resetDelayTimer;

    /// <summary>
    /// Records the starting position of the graphing node so it can be reset properly.
    /// </summary>
    private Vector3 _startingPosition;

    /// <summary>
    /// Used to show the graph of the function.
    /// </summary>
    private LineRenderer _lineRenderer;
    
    /// <summary>
    /// Tracks current number of positions used by the line renderer.
    /// </summary>
    private int _lineRendererIndex;

    #endregion
    
    #region --Unity Specific Methods---

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        CheckGraphReset();
        UpdateInput();
        DrawGraph();
    }
    
    #endregion
    
    #region --Custom Methods--
    
    /// <summary>
    /// Initializes variables and caches components.
    /// </summary>
    private void Init()
    {
        _transform = transform;
        _startingPosition = _transform.parent.position;
        _resetCounter = Resources.Load("ScriptableObjects/ResetCounter") as Counter;
        _previousCount = -1;
        
        ConfigureLineRenderer();
        CheckDerivativeCount();
    }

    /// <summary>
    /// Sets up the line renderer for use.
    /// </summary>
    private void ConfigureLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startColor = _lineRendererColor;
        _lineRenderer.endColor = _lineRendererColor;
        _lineRenderer.startWidth = .1f;
        _lineRenderer.endWidth = .1f;
    }

    /// <summary>
    /// Performs a check against the _derivativeCount and calculates the derivative of the _originalFunction.
    /// </summary>
    private void CheckDerivativeCount()
    {
        if (_derivativeCount < 0) _derivativeCount = 0;

        if (_derivativeCount <= 0) return;
        
        _derivativeFunction = _originalFunction;
        for (int i = 0; i < _derivativeCount; i++)
        {
            _derivativeFunction = CalculateDerivative();
        }
    }

    /// <summary>
    /// Checks if the _previousCount is equal to the _resetCounter and performs a reset of the graph.
    /// </summary>
    private void CheckGraphReset()
    {
        if (!_isResetting)
        {
            if (_previousCount == _resetCounter.count)
            {
                ResetGraph();
            }
        }
        else
        {
            _resetDelayTimer += Time.deltaTime;
            if (_resetDelayTimer > _resetDelayTime)
            {
                ResetGraph();
                _resetDelayTimer = 0;
                _isResetting = false;
            }
        }
    }

    /// <summary>
    /// Resets graph to default.
    /// </summary>
    private void ResetGraph()
    {
        _isResetting = true;
        _previousCount = -1;
        _lineRenderer.positionCount = 0;
        _lineRendererIndex = 0;
        _transform.position = _startingPosition;
        _input = 0;
        _movePosition = _derivativeCount > 0 ? 
                            GraphFunctionLibrary.GraphFunction(_input, _derivativeFunction.exponent, _derivativeFunction.coefficient) : 
                            GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
    }

    /// <summary>
    /// Increments the input value passed into the graph function, adjusting it for _graphSpeed and the time elapsed from the last frame.
    /// </summary>
    private void UpdateInput()
    {
        if (_isResetting) return;

        _input = _movePosition.x + _graphSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Tracks the output of the _originalFunction in order to stop the graph from drawing if it passes the _yPositionBoundary.
    /// </summary>
    private bool CheckGraphBounds()
    {
        Vector3 _originalMovePosition = GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
        return (_originalMovePosition.y + _originalFunction.yIntercept) < _yPositionBoundary;
    }
    
    /// <summary>
    /// Updates the line renderer component to draw the graph.
    /// </summary>
    private void DrawGraph()
    {
        if (_isResetting) return;

        //Draw either the derivative or the original function.
        _movePosition = _derivativeCount > 0 ? 
                            GraphFunctionLibrary.GraphFunction(_input, _derivativeFunction.exponent, _derivativeFunction.coefficient) : 
                            GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
        
        //Constantly update the _previousCount to determine when a function has stopped graphing.
        _previousCount = _resetCounter.count;
        
        //If the graph has passed the _yPositionBoundary, then stop graphing.
        if (CheckGraphBounds()) return;

        //Constantly increment the _resetCounter to indicate a graph is currently drawing.
        _resetCounter.count++;
            
        //Update the positionCount to 1 value greater than the _lineRendererIndex to ensure the SetPosition method properly assigns the next position for it to render the line.
        if (_lineRenderer.positionCount <= _lineRendererIndex)
        {
            _lineRenderer.positionCount = _lineRendererIndex + 1;
        }

        //Update the graph's position.
        _movePosition.y += _startingPosition.y;
        Vector3 _updatedInput = _movePosition;
        _updatedInput.x += _startingPosition.x;

        //Draw the line renderer.
        _lineRenderer.SetPosition(_lineRendererIndex, _updatedInput);
        _lineRendererIndex++;
    }

    /// <summary>
    /// Performs the calculation of the _derivativeFunction.
    /// </summary>
    private GraphFunctionEquation CalculateDerivative()
    {
        GraphFunctionEquation _derivative = new GraphFunctionEquation();
        
        //Uses the power rule for derivatives.
        _derivative.coefficient = _derivativeFunction.exponent * _derivativeFunction.coefficient;
        _derivative.exponent = _derivativeFunction.exponent - 1;

        if (Math.Abs(_derivative.exponent) <= 0)
        {
            _derivative.yIntercept = _derivative.coefficient;
        }
        else
        {
            _derivative.yIntercept = 0;
        }

        return _derivative;
    }
    
    #endregion
    
}
