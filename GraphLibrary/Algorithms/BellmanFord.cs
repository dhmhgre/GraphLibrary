﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.Algorithms {

    public class PathIterator : IEnumerator<CGraphNode> {
        private List<CGraphNode> m_path;
        private int position;

        public PathIterator(List<CGraphNode> mPath) {
            m_path = mPath;
        }

        public CGraphNode Current => m_path[position];
        
        public void Dispose() {
            
        }

        public bool MoveNext() {
            position++;
            return position < m_path.Count;
        }

        public void Reset() {
            position = -1;
        }

        object IEnumerator.Current => Current;
    }

    public class Path :IEnumerable<CGraphNode> {
        private List<CGraphNode> m_path=new List<CGraphNode>();

        public CGraphNode this[int index] {
            get { return m_path[index]; }
        }

        public IEnumerator<CGraphNode> GetEnumerator() {
            return new PathIterator(m_path);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a node at the specified position. The default is to
        /// place the node at the end of the list.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        public void Insert(CGraphNode node, int index = -1) {
            if (index == -1) {
                m_path.Add(node);
            }
            else if ( index>-1) {
                m_path.Insert(index,node);
            }
        }

        public override string ToString() {
            StringBuilder path = new StringBuilder();
            path.AppendFormat("{0}->{1}: ",m_path[0].M_Label,m_path[m_path.Count-1].M_Label);
            foreach (CGraphNode node in m_path) {
                path.Append(node.M_Label);
            }
            return path.ToString();
        }

    }

    public class BellmanFordQueryInfo : CGraphQueryInfo<NodePathInfo,int?, Dictionary<CGraphNode, Dictionary<CGraphNode, Path>>> {
        public BellmanFordQueryInfo(CGraph graph, object key) : base(graph, key) {
        }

        public int? Distance(CGraphNode node) {
            return Info(node).MDistance;
        }

        public void SetDistance(CGraphNode node, int distance) {
            Info(node).MDistance = distance;
        }
        public void SetPredecessor(CGraphNode node, CGraphNode predecessor) {
            Info(node).MPredecessor = predecessor;
        }

        public CGraphNode Predecessor(CGraphNode node) {
            return Info(node).MPredecessor;
        }

        public int? Weight(CGraphNode source, CGraphNode target) {
            return Info(source, target);
        }

        public Dictionary<CGraphNode, Dictionary<CGraphNode, Path>> ShortestPaths() {
            return Info();
        }
    }

    public class BellmanFord :CGraphAlgorithm<int> {
        private CGraphQueryInfo<object,int,object> m_inputGraphWeightInfo;
        private BellmanFordQueryInfo m_outputShortestPathsInfo;
        private CGraph mGraph;
        private CGraphNode m_source;
        public const int  m_PATHINFO =0;
        public const int m_SHORTESTPATHSINFO = 1;
        private Dictionary<CGraphNode, Dictionary<CGraphNode, Path >> m_shortestPaths = 
            new Dictionary<CGraphNode, Dictionary<CGraphNode, Path>>();
           
        
        public BellmanFord(CGraph graph,CGraphNode source, int graphWeightsKey) {
            mGraph = graph;
            m_source = source;
            this[m_PATHINFO] = m_outputShortestPathsInfo =  new BellmanFordQueryInfo(graph,this);
            m_inputGraphWeightInfo = new CGraphQueryInfo<object, int, object>(graph,graphWeightsKey);
            m_outputShortestPathsInfo.CreateInfo(m_shortestPaths);
        }

        public void FindAllPairsShortestPaths() {
            CIt_GraphNodes itn = new CIt_GraphNodes(mGraph);

            for (itn.Begin(); !itn.End(); itn.Next()) {
                m_source = itn.M_CurrentItem;
                Run();
            }
        }

        public override void Init() {

            CIt_GraphNodes itn = new CIt_GraphNodes(mGraph);
            m_shortestPaths[m_source] = new Dictionary<CGraphNode, Path>();

            for (itn.Begin(); !itn.End(); itn.Next()) {
                if (itn.M_CurrentItem != m_source) {
                    m_outputShortestPathsInfo.CreateInfo(itn.M_CurrentItem,
                        new NodePathInfo() {MDistance = null, MPredecessor = null});
                }
                else {
                    m_outputShortestPathsInfo.CreateInfo(itn.M_CurrentItem,
                        new NodePathInfo() { MDistance = 0, MPredecessor = null});
                }
            }
        }

        public override int Run() {
            Init();

            CIt_GraphEdges ite = new CIt_GraphEdges(mGraph);
            for (int i = 0; i < mGraph.M_NumberOfNodes - 1; i++) {
#if DEBUG
                Console.WriteLine("Iteration : {0}",i);
#endif
                for (ite.Begin(); !ite.End(); ite.Next()) {
                    Relax(ite.M_CurrentItem.M_Source,ite.M_CurrentItem.M_Target);
                }
            }

            for (ite.Begin(); !ite.End(); ite.Next()) {
                if (Distance(ite.M_CurrentItem.M_Target) > Distance(ite.M_CurrentItem.M_Source) +
                    Weight(ite.M_CurrentItem.M_Source, ite.M_CurrentItem.M_Target)) {
                    throw new Exception("The graph contains negative cycles");
                }
            }
            GeneratePaths();
#if DEBUG
            Debug();
#endif
            return 0;
        }

        private void Debug() {
            CIt_GraphNodes it = new CIt_GraphNodes(mGraph);

            for (it.Begin(); !it.End(); it.Next()) {
                Console.WriteLine("{0}.distance={1}", it.M_CurrentItem.M_Label, Distance(it.M_CurrentItem));
                if (Predecessor(it.M_CurrentItem) != null) {
                    Console.WriteLine("{0}.predecessor={1}", it.M_CurrentItem.M_Label,
                        Predecessor(it.M_CurrentItem).M_Label);
                }
            }

            Console.WriteLine("Shortest Paths {0}->*",m_source.M_Label);

            for (it.Begin(); !it.End(); it.Next()) {
                
                foreach (var cGraphNode in m_shortestPaths[m_source][it.M_CurrentItem]) {
                    Console.Write("{0},",cGraphNode.M_Label);
                }
                Console.WriteLine();
            }
        }

        private void GeneratePaths() {
            CIt_GraphNodes it = new CIt_GraphNodes(mGraph);
            CGraphNode m_node;

            for (it.Begin(); !it.End(); it.Next()) {
                m_shortestPaths[m_source][it.M_CurrentItem]=new Path();
                m_node = it.M_CurrentItem;
                while (m_node!=null){
                    m_shortestPaths[m_source][it.M_CurrentItem].Insert(m_node,0);
                    m_node = Predecessor(m_node);
                }
            }
        }

        protected void Relax(CGraphNode source, CGraphNode target) {
            int currentDistance ;
            
            if ( Distance(source)!=null ) {
                currentDistance = (int) Distance(source) + Weight(source, target);

                if (Distance(target) != null) {
                    if (Distance(target) > currentDistance) {
                        SetDistance(target, currentDistance);
                        SetPredecessor(target, source);
                    }
                }
                else {
                    SetDistance(target, currentDistance);
                    SetPredecessor(target, source);
                }
            }
            else {
#if DEBUG
                currentDistance = Weight(source, target);
#endif
            }

#if DEBUG
            Console.WriteLine("Relax: {0} -->  {1}  CurrentDistance={2}, Distance={3}, Weight={4}", 
                source.M_Label, target.M_Label,currentDistance, Distance(target),Weight(source, target));
#endif
        }

        protected int? Distance(CGraphNode node) {
            return m_outputShortestPathsInfo.Distance(node);
        }

        protected void SetDistance(CGraphNode node, int distance) {
            m_outputShortestPathsInfo.SetDistance(node,distance);
        }
        protected void SetPredecessor(CGraphNode node, CGraphNode predecessor) {
            m_outputShortestPathsInfo.SetPredecessor(node,predecessor);
        }

        protected CGraphNode Predecessor(CGraphNode node) {
            return m_outputShortestPathsInfo.Predecessor(node);
        }

        protected Dictionary<CGraphNode, Dictionary<CGraphNode, Path>> ShortestPaths() {
            return m_outputShortestPathsInfo.ShortestPaths();
        } 

        protected int Weight(CGraphNode source, CGraphNode target) {
            return (int) (m_inputGraphWeightInfo.Info(source, target));
        }



    }



    public class NodePathInfo {
        private int? m_distance;
        private CGraphNode m_predecessor;

        public NodePathInfo() {
        }

        public int? MDistance {
            get => m_distance;
            set => m_distance = value;
        }

        public CGraphNode MPredecessor {
            get => m_predecessor;
            set => m_predecessor = value;
        }
    }
    
}
