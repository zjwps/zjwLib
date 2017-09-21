using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BtNode
{
    public enum Result
    {
        NoRun,
        Run,
        Succ,
        Fail,
        End,
    }
    /// <summary>
    /// 节点数据
    /// </summary>
    public class Data
    {
        /// <summary>
        /// int数据
        /// </summary>
        public int Int32 = 0;
        /// <summary>
        /// string 数据
        /// </summary>
        public string Str = "";
        /// <summary>
        /// 复杂数据
        /// </summary>
        private List<object> _dataList = new List<object>();
        private List<string> _dataNames = new List<string>();
        private int trigerType;

        public Data(int int32)
        {
            Int32 = int32;
        }
        public Data(string str)
        {
            Str = str;
        }

        public Data()
        {
        }

        // Should use dataId as parameter to get data instead of this
        public T GetData<T>(string dataName)
        {
            int dataId = IndexOfDataId(dataName);
            if (dataId == -1) return default(T);

            return (T)_dataList[dataId];
        }
        public T GetData<T>(int dataId)
        {
            //if (BT.BTConfiguration.ENABLE_DATABASE_LOG)
            //{
            //    Debug.Log("BTDatabase: getting data for " + _dataNames[dataId]);
            //}
            return (T)_dataList[dataId];
        }

        public void SetData<T>(string dataName, T data)
        {
            int dataId = GetDataId(dataName);
            _dataList[dataId] = (object)data;
        }

        public void SetData<T>(int dataId, T data)
        {
            _dataList[dataId] = (object)data;
        }

        public bool CheckDataNull(string dataName)
        {
            int dataId = IndexOfDataId(dataName);
            if (dataId == -1) return true;

            return CheckDataNull(dataId);
        }

        public bool CheckDataNull(int dataId)
        {
            // Despite == test, Equal test helps the case that the reference is Monobahvior and is destroyed.
            return _dataList[dataId] == null || _dataList[dataId].Equals(null);
        }

        public int GetDataId(string dataName)
        {
            int dataId = IndexOfDataId(dataName);
            if (dataId == -1)
            {
                _dataNames.Add(dataName);
                _dataList.Add(null);
                dataId = _dataNames.Count - 1;
            }

            return dataId;
        }

        private int IndexOfDataId(string dataName)
        {
            for (int i = 0; i < _dataNames.Count; i++)
            {
                if (_dataNames[i].Equals(dataName)) return i;
            }

            return -1;
        }

        public bool ContainsData(string dataName)
        {
            return IndexOfDataId(dataName) != -1;
        }
    }

    /// <summary>
    /// 行为节点
    /// </summary>
    public class Node
    {
        public uint Id;
        public string Name = "";
        public string Details { get; set; }
        public Data Data;
        //根节点
        //public Node Root;
        //父节点 是由那个节点触发的 
        public Node Parent;
        //属于哪个树
        public Btree Tree;

        protected bool _started = false;
        public bool IsStart
        {
            get { return _started; }
        }
        protected bool _end = false;

        public Node SuccNode;
        public Node FailNode;
        //public void Update(float del)
        public void Start(Data data = null)
        {
            if (_started) return;
            if (data != null) Data = data;
            _started = true;
            if (Name == "") Name = this.GetType().Name;
            OnStart();
        }

        public virtual void OnStart()
        {

        }

        public Result Update(float deltaTime)
        {
            if (_end) return Result.End;
            if (!_started) return Result.NoRun;
            var r = OnUpdate(deltaTime);

            ResultHandel(r);
            if (r == Result.Succ) _end = true;
            return r;
        }
        private Result ResultHandel(Result r)
        {
            if (r == Result.Succ)
            {
                if (SuccNode != null) StartChildNode(SuccNode);
            }
            else if (r == Result.Fail)
            {
                if (FailNode != null) StartChildNode(FailNode);
            }
            OnResult(r);
            return r;
        }
        public Node AddSuccNode(Node node)
        {
            SuccNode = node;
            SuccNode.Parent = this;
            SuccNode.Data = Data;
            return node;
        }
        public Node AddFailNode(Node node)
        {
            FailNode = node;
            node.Parent = this;
            FailNode.Data = Data;
            return node;
        }
        /// <summary>
        /// 激活子节点.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Result StartChildNode(Node node)
        {
            if (Tree == null) return Result.Fail;
            Tree.AddNode(node);
            return Result.Succ;

        }
        public virtual Result OnResult(Result r)
        {
            return r;
        }
        public virtual Result OnUpdate(float deltaTime)
        {
            return Result.Run;
        }
    }


    /// <summary>
    /// tree节点，方便控制
    /// </summary>
    public class TreeNode : Node
    {
        //自身的tree
        protected Btree _tree;
        protected bool _stop = false;
        public bool IsStop
        {
            get { return _stop; }
        }
        public TreeNode()
        {
            _tree = new Btree();
        }
        public void Stop()
        {
            _stop = true;
            OnStop();
        }
        public override Result OnUpdate(float deltaTime)
        {
            if (_stop) return Result.Succ;
            if (_tree != null)
                _tree.Update(deltaTime);
            if (!_tree.NeedUpdate) return Result.Succ;
            return base.OnUpdate(deltaTime);
        }
        public virtual void OnStop()
        {

        }
    }
    /// <summary>
    /// 复合节点
    /// </summary>
    public class CompositeNode : Node
    {
        private List<Node> _childs;

        public List<Node> Childs
        {
            get
            {
                if (_childs == null)
                {
                    _childs = new List<Node>();
                }
                return _childs;
            }

            set
            {
                _childs = value;
            }
        }
        public void AddChild(Node node)
        {
            if (node != null)
            {
                Childs.Add(node);
            }
        }
        public void RemoveChild(Node node)
        {
            Childs.Remove(node);
            //selectedChildrenForClear.Remove(node);
        }
    }
    /// <summary>
    /// 选择执行的节点,如果其中一个成功，就选这个来执行。
    /// </summary>
    public class SectorNode : CompositeNode
    {
        public Node ResultNode;
        public Node Curr;
        public override Result OnUpdate(float deltaTime)
        {
            if (Childs.Count == 0) return Result.Fail;
            if (Curr == null)
            {
                Curr = Childs[0];
                Curr.Start(Data);
            }
            var r = Curr.Update(deltaTime);
            if (r == Result.Fail)
            {
                return Result.Run;
            }
            if (r == Result.Succ)
            {
                Childs.Remove(Curr);
                return Result.Succ;
            }
            return base.OnUpdate(deltaTime);

        }

    }


    /// <summary>
    /// 序列执行的节点
    /// ,子节点有一个失败了就销毁整个。
    /// </summary>
    public class SequenceNode : CompositeNode
    {
        /// <summary>
        /// 当前运行的节点
        /// </summary>
        public Node Curr;
        public override Result OnUpdate(float deltaTime)
        {
            if (Childs.Count == 0) return Result.Succ;
            if (Curr == null)
            {
                Curr = Childs[0];
                Curr.Start(Data);
            }
            var r = Curr.Update(deltaTime);
            if (r == Result.Fail)
            {
                Childs.Remove(Curr);
                Curr = null;
                return Result.Fail;
            }
            if (r == Result.Succ)
            {
                Childs.Remove(Curr);
                Curr = null;
            }
            return base.OnUpdate(deltaTime);

        }
    }

    /// <summary>
    /// 执行一个方法的node
    /// </summary>
    public class FuncionNode : Node
    {
        private Action<Node> _action;
        private bool autoEnd;

        public FuncionNode(Action<Node> action, bool autoEnd = true)
        {
            _action = action;
            this.autoEnd = autoEnd;
        }
        public override void OnStart()
        {
            if (_action != null) _action(this);
        }
        public override Result OnUpdate(float deltaTime)
        {
            if (autoEnd)
            {
                return Result.Succ;
            }
            return Result.Run;
        }
    }
    /// <summary>
    /// 执行一个方法，返回true就 返回 Result.Succ
    /// </summary>
    public class JudgeNode : Node
    {
        private Func<Node, bool> _judgeFunc;
        public override void OnStart()
        {
            //Name = _judgeFunc.GetType().
        }
        public JudgeNode(Func<Node, bool> judgeFunc)
        {
            _judgeFunc = judgeFunc;
        }
        public override Result OnUpdate(float deltaTime)
        {
            if (_judgeFunc != null)
            {
                var r = _judgeFunc(this);
                if (r) return Result.Succ;
                else return Result.Fail;
            }
            return Result.Fail;
            //return base.OnUpdate(deltaTime);
        }
    }
    public class UpdateAcionNode : Node
    {
        private Func<Func<bool>> mBuildAction;
        private Func<bool> mRunAction;
        public UpdateAcionNode(Func<Func<bool>> buildAction)
        {
            mBuildAction = buildAction;
        }
        public override void OnStart()
        {
            mRunAction = mBuildAction();
        }
        public override Result OnUpdate(float deltaTime)
        {
            if (mRunAction == null) return Result.Fail;
            if (mRunAction()) return Result.Succ;
            return Result.Run;
        }

    }

    public class Btree
    {
        public List<Node> nodes = new List<Node>();
        public bool NeedUpdate = false;
        private uint _nodeIndex = 0;


        public Btree()
        {


        }
        public void Update(float deltaTime)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var t = nodes[i];
                var r = t.Update(deltaTime);
                //如果成功，失败，完成 都移除。
                if (r != Result.Run)
                {
                    nodes.Remove(t);
                    if (nodes.Count == 0) NeedUpdate = false;
                }
            }
        }
        public void Reset()
        {
            nodes = new List<Node>();
            _nodeIndex = 0;
        }
        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(Node node, bool autoStart = true)
        {
            if (node == null)
            {
                Debug.LogError("节点不能为空");
                return;
            }
            _nodeIndex++;
            node.Tree = this;
            nodes.Add(node);
            node.Id = _nodeIndex;
            if (autoStart)
                node.Start();
            NeedUpdate = true;
        }
        /// <summary>
        /// 添加序列执行的复合节点。
        /// </summary>
        /// <param name="nodes"></param>
        public void AddSequenceNode(params Node[] nodes)
        {
            AddNode(BuildSequenceNode(nodes));
        }
        /// <summary>
        /// 序列执行的复合节点。
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static SequenceNode BuildSequenceNode(params Node[] nodes)
        {
            var r = new SequenceNode();
            var list = r.Childs = new List<Node>();
            for (int i = 0; i < nodes.Length; i++)
            {
                list.Add(nodes[i]);
            }
            return r;
        }
    }
    public static class BtreeEx
    {
        public static Node AddSuccNode(this Node node, Func<Node, bool> needUpdatefunc)
        {
            var t = new JudgeNode(needUpdatefunc);
            node.AddSuccNode(t);
            return t;
        }

        public static Node AddSuccNode(this Node node, Func<Func<bool>> buildUpdatefunc)
        {
            var t = new UpdateAcionNode(buildUpdatefunc);
            node.AddSuccNode(t);
            return t;
        }
        public static Node AddSuccNode(this Node node, Action<Node> runAction)
        {
            var t = new FuncionNode(runAction);
            node.AddSuccNode(t);
            return t;
        }
    }
}