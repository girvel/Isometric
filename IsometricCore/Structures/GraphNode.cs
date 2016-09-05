﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace IsometricCore.Structures
{
    public static class GraphNode
    {
        public static GraphNode<T> Create<T>(T value, Graph<T> parentGraph)
        {
            return new GraphNode<T>(value, parentGraph);
        }
    }

    [Serializable]
    public class GraphNode<T>
    {
        public T Value { get; set; }
        public Graph<T> ParentGraph { get; set; }

        private readonly List<GraphNode<T>> _children;



        /// <summary>
        /// Serialization ctor.
        /// </summary>
        [Obsolete("Using serialization ctor", true)]
        public GraphNode() {}

        public GraphNode(T value, Graph<T> parentGraph)
        {
            Value = value;
            ParentGraph = parentGraph;
            _children = new List<GraphNode<T>>();
        } 



        public void Add(GraphNode<T> child)
        {
            #if !DEBUG
            ParentGraph.TryAddNode(child);
            #else
            if (!ParentGraph.TryAddNode(child)
                && _children.Contains(child) 
                && ParentGraph.CheckIdentity)
            {
                throw new ArgumentException("Node already has this child");
            }
            #endif

            _children.Add(child);
        }

        public GraphNode<T>[] GetChildren() => _children.ToArray();



        public bool IsParentOf(T item)
        {
            return _children.Any(c => c.Value.Equals(item));
        }
    }
}