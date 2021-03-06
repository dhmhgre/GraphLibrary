﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.Generics {
    /// <summary>
    /// Provides information regarding the nodes, edges and the graph
    /// The algorithm operates on a source graph and can potentially create a target
    /// graph. The purpose of this class is to simplify the expressions that the designer
    /// should write to access graph information from the code that describes the 
    /// algorithm. This way the code that describes the algorithm is described
    /// with minimal complexity expressions something necessery to make him easily
    /// readapt to the code after long periods of abstention from this framework
    /// The class provides general access (takes object and returns object) to 
    /// the node/edge information of the source and target graphs and requires
    /// casting to specialize it. 
    /// </summary>
    /// <typeparam name="Node">The type of the node.</typeparam>
    /// <typeparam name="Edge">The type of the edge.</typeparam>
    [Serializable]
    public abstract class AbstractGraphQueryInfo<GRAPH, Node, Edge, IN,IE,IG> {

        /// <summary>
        /// The original graph where the algorithm is applied
        /// </summary>
        protected GRAPH m_graph;

        /// <summary>
        /// Sets the source graph reference. It is mandatory
        /// before source graph queries begin
        /// </summary>
        public GRAPH M_Graph {
            get { return m_graph; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractGraphQueryInfo{Node, Edge}"/> class.
        /// </summary>
        public AbstractGraphQueryInfo() {
        }

        /// <summary>
        /// Returns the information from the source graph under the given key
        /// </summary>
        /// <returns>The information object</returns>
        public abstract IG Info();
        public abstract IG TempInfo();

        /// <summary>
        /// Returns information concerning a node of the source graph 
        /// </summary>
        /// <param name="node">The node of the source graph</param>
        /// <returns></returns>
        public abstract IN Info(Node node, bool checkOwnership = true);
        public abstract IN TempInfo(Node node, bool checkOwnership = true);

        /// <summary>
        /// Returns the information of the specified edge of the source graph
        /// </summary>
        /// <param name="edge">The edge of the source graph</param>
        /// <returns></returns>
        public abstract IE Info(Edge edge, bool checkOwnership = true);
        public abstract IE TempInfo(Edge edge, bool checkOwnership = true);

        /// <summary>
        /// Returns the information of the edge between the given source
        /// and target nodes of the source graph.
        /// </summary>
        /// <param name="source">The source node on the source graph</param>
        /// <param name="target">The target node on the source graph</param>
        /// <returns></returns>
        public abstract IE Info(Node source, Node target, bool checkOwnership = true);
        public abstract IE TempInfo(Node source, Node target, bool checkOwnership = true);

        /// <summary>
        /// Create an information object for the source graph under the 
        /// given key
        /// </summary>
        /// <param name="info">The information object</param>
        public abstract void CreateInfo(IG info);
        public abstract void CreateTempInfo(IG info);

        /// <summary>
        /// Stores the information at the specified node of the source graph. If information already
        /// exists, it is overwritten
        /// </summary>
        /// <param name="node">The node on the source graph.</param>
        /// <param name="info">The information object</param>
        public abstract void CreateInfo(Node node, IN info, bool checkOwnership = true);
        public abstract void CreateTempInfo(Node node, IN info, bool checkOwnership = true);

        /// <summary>
        /// Stores the information at the specified edge of the source graph. If information already
        /// exists, it is overwritten
        /// </summary>
        /// <param name="edge">The edge on the source graph</param>
        /// <param name="info">The information object</param>
        public abstract void CreateInfo(Edge edge, IE info , bool checkOwnership = true);
        public abstract void CreateTempInfo(Edge edge, IE info, bool checkOwnership = true);

        /// <summary>
        /// Stores the information at the specified edge of the source graph. If information already
        /// exists, it is overwritten
        /// </summary>
        /// <param name="source">The source node on the source graph</param>
        /// <param name="target">The target node on the source graph</param>
        /// <param name="info">The information object</param>
        public abstract void CreateInfo(Node source, Node target, IE info, bool checkOwnership = true);
        public abstract void CreateTempInfo(Node source, Node target, IE info, bool checkOwnership = true);

    }

    
}
