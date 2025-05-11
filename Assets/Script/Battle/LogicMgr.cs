using System.Collections.Generic;

namespace NanBeiStudy.Battle
{
    public interface ILogicNodeContainerEnv
    {
        void RunNode(LogicNode logicNode);
        void StopNode(LogicNode logicNode);
    }
    
    public class LogicMgr: ILogicNodeContainerEnv
    {
        //实现单例模式
        private static LogicMgr _instance;
        private readonly List<LogicNode> _runningNodes= new List<LogicNode>();
        public static LogicMgr Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogicMgr();
                }
                return _instance;
            }
        }

        public void Init()
        {
            //初始化逻辑
        }

        public void RunNode(LogicNode logicNode)
        {
            _runningNodes.Add(logicNode);
            logicNode.Enter();
        }
        
        public void StopNode(LogicNode logicNode)
        {
            logicNode.Exit();
            _runningNodes.Remove(logicNode);
        }

        public void Tick()
        {
            foreach (var logicNode in _runningNodes)
            {
                logicNode.Tick();
            }
        }
    }
}