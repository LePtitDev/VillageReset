using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DecisionTree<T>
{

    /// <summary>
    /// Decision's percept
    /// </summary>
    public class Percept
    {

        // Percept name
        private string _name;

        // Percept actions
        private Action[] _actions;

        // Percept weights
        private float[] _weights;
        
        /// <summary>
        /// Percept name
        /// </summary>
        public string Name { get { return _name; } }
        
        /// <summary>
        /// Percept actions
        /// </summary>
        public Action[] Actions { get { return _actions; } }

        /// <summary>
        /// Percept weights
        /// </summary>
        public float[] Weights { get { return _weights; } }
        
        /// <summary>
        /// Create a percept
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="actions">Actions</param>
        /// <param name="weights">Weights</param>
        public Percept(string name, Action[] actions, float[] weights)
        {
            _name = name;
            _actions = actions;
            _weights = weights;
        }
        
    }

    /// <summary>
    /// Decision's action
    /// </summary>
    public class Action : IComparable<Action>
    {

        // Action name
        private string _name;

        // Action content
        private T _content;

        // Action weight
        private float _weight;
        
        /// <summary>
        /// Action name
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Action content
        /// </summary>
        public T Content { get { return _content; } }
        
        /// <summary>
        /// Action weight
        /// </summary>
        public float Weight { get { return _weight; } }

        /// <summary>
        /// Create an action
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="content">Content</param>
        public Action(string name, T content)
        {
            _name = name;
            _content = content;
            _weight = 0.0f;
        }

        /// <summary>
        /// Reset action weight
        /// </summary>
        public void Reset()
        {
            _weight = 0.0f;
        }

        /// <summary>
        /// Activate a percept for increment (or decrement) action weight
        /// </summary>
        /// <param name="weight"></param>
        public void Activate(float weight)
        {
            _weight += weight;
        }

        /// <summary>
        /// Compare to an other action
        /// </summary>
        /// <param name="other">The other action</param>
        /// <returns>-1 if less, 0 if equal and 1 otherwise</returns>
        public int CompareTo(Action other)
        {
            float res = _weight - other._weight;
            if (res == 0.0f)
                return 0;
            return res < 0.0f ? -1 : 1;
        }
        
    }

    // Percepts list
    private List<Percept> _percepts;

    // Actions list
    private List<Action> _actions;
    
    /// <summary>
    /// Percepts list
    /// </summary>
    public Percept[] Percepts { get { return _percepts.ToArray(); } }
    
    /// <summary>
    /// Actions list
    /// </summary>
    public Action[] Actions { get { return _actions.ToArray(); } }

    /// <summary>
    /// Create a decision tree
    /// </summary>
    public DecisionTree()
    {
        _percepts = new List<Percept>();
        _actions = new List<Action>();
    }

    /// <summary>
    /// Clear the decision tree
    /// </summary>
    public void Clear()
    {
        _percepts.Clear();
        _actions.Clear();
    }

    /// <summary>
    /// Reset action weights
    /// </summary>
    public void Reset()
    {
        foreach (Action action in _actions)
            action.Reset();
    }

    /// <summary>
    /// Add a percept
    /// </summary>
    /// <param name="percept">Percept</param>
    public void AddPercept(Percept percept)
    {
        _percepts.Add(percept);
    }

    /// <summary>
    /// Remove a percept
    /// </summary>
    /// <param name="percept">Percept</param>
    public void RemovePercept(Percept percept)
    {
        _percepts.Remove(percept);
    }

    /// <summary>
    /// Add an action
    /// </summary>
    /// <param name="action">Action</param>
    public void AddAction(Action action)
    {
        _actions.Add(action);
    }

    /// <summary>
    /// Remove an action
    /// </summary>
    /// <param name="action">Action</param>
    public void RemoveAction(Action action)
    {
        _actions.Remove(action);
    }

    /// <summary>
    /// Execute the decision tree
    /// </summary>
    /// <returns>The action content</returns>
    public T Decide()
    {
        _actions.Sort();
        return _actions.Last().Content;
    }
    
}