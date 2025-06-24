using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class RendererCache
{
    private readonly Dictionary<string, UIElement> _elements = new Dictionary<string, UIElement>();
    private readonly Canvas _canvas;
    private bool _isGridDrawn;

    public RendererCache(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void ClearDynamicElements()
    {
        foreach (var element in _elements.Values)
        {
            _canvas.Children.Remove(element);
        }
        _elements.Clear();
    }

    public UIElement GetOrCreateElement(string key, Func<UIElement> createElement)
    {
        if (!_elements.TryGetValue(key, out var element))
        {
            element = createElement();
            _elements[key] = element;
            _canvas.Children.Add(element);
        }
        return element;
    }

    public bool IsGridDrawn
    {
        get => _isGridDrawn;
        set => _isGridDrawn = value;
    }
}