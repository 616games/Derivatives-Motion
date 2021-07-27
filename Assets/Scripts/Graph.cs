using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    #region --Fields / Properties--

    [SerializeField, Tooltip("Global counter to monitor when all graphs should be reset.")]
    private Counter _resetCounter;

    [SerializeField, Tooltip("The original equation for this graph to which any derivatives will be created from.")]
    private GraphFunctionEquation _originalFunction;

    [SerializeField, Tooltip("The number of derivatives to be taken of the _originalFunction.")]
    private int _derivativeCount;
    
    [SerializeField, Tooltip("The derivative of the _originalFunction.")]
    private GraphFunctionEquation _derivativeFunction;
    
    [SerializeField, Tooltip("How fast the graph should be drawn.")]
    private float _graphSpeed = 1;

    [SerializeField, Tooltip("Desired color of the graph's line renderer.")]
    private Color _lineRendererColor;

    /// <summary>
    /// Keeps track of the change in value of the _resetCounter to determine when a reset should occur.
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
    /// Input value for the graph function.
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
        
        if (_derivativeCount > 0)
        {
            _derivativeFunction = _originalFunction;
            for (int i = 0; i < _derivativeCount; i++)
            {
                _derivativeFunction = CalculateDerivative();
            }
        }
    }

    /// <summary>
    /// Checks the current graph behavior for any changes.
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
        _resetCounter.count = 0;
        _lineRenderer.positionCount = 0;
        _lineRendererIndex = 0;
        _transform.position = _startingPosition;
        _input = 0;
        _movePosition = _derivativeCount > 0 ? 
                            GraphFunctionLibrary.GraphFunction(_input, _derivativeFunction.exponent, _derivativeFunction.coefficient) : 
                            GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
    }

    /// <summary>
    /// Increments the input value passed into the graph function.
    /// </summary>
    private void UpdateInput()
    {
        if (_isResetting) return;

        _input = _movePosition.x + _graphSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Tracks the output of the _originalFunction to determine when any derivatives should also stop graphing.
    /// </summary>
    private bool CheckGraphBounds()
    {
        Vector3 _originalMovePosition = GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
        return (_originalMovePosition.y + _originalFunction.height) < 0;
    }
    
    /// <summary>
    /// Updates the line renderer component to draw the graph.
    /// </summary>
    private void DrawGraph()
    {
        if (_isResetting) return;

        _movePosition = _derivativeCount > 0 ? 
                            GraphFunctionLibrary.GraphFunction(_input, _derivativeFunction.exponent, _derivativeFunction.coefficient) : 
                            GraphFunctionLibrary.GraphFunction(_input, _originalFunction.exponent, _originalFunction.coefficient);
        
        _previousCount = _resetCounter.count;
        
        if (CheckGraphBounds()) return;

        _resetCounter.count++;
            
        if (_lineRenderer.positionCount <= _lineRendererIndex)
        {
            _lineRenderer.positionCount = _lineRendererIndex + 1;
        }

        _movePosition.y += _startingPosition.y;
        Vector3 _updatedX = _movePosition;
        _updatedX.x += _startingPosition.x;

        _lineRenderer.SetPosition(_lineRendererIndex, _updatedX);
        _lineRendererIndex++;
    }

    /// <summary>
    /// Performs the calculation of the _derivativeFunction.
    /// </summary>
    private GraphFunctionEquation CalculateDerivative()
    {
        GraphFunctionEquation _derivative = new GraphFunctionEquation();
        _derivative.coefficient = _derivativeFunction.exponent * _derivativeFunction.coefficient;
        _derivative.exponent = _derivativeFunction.exponent - 1;

        if (Math.Abs(_derivative.exponent) < Mathf.Epsilon)
        {
            _derivative.height = _derivative.coefficient;
        }
        else
        {
            _derivative.height = 0;
        }

        return _derivative;
    }
    
    #endregion
    
}
