using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.Battle
{
    public interface ILogicNodeEnv: ILogicNodeContainerEnv
    {
        LogicNode GetLogicNodByIndex(int index);
    }
    
    public class LogicNodeContainer: ILogicNodeEnv
    {
        private ILogicNodeContainerEnv _env;
        public List<LogicNode> LogicNodes;
        public LogicNodeContainer(ILogicNodeContainerEnv env, List<LogicNode> logicNodes = null)
        {
            _env = env;
            LogicNodes = logicNodes ?? new List<LogicNode>();
        }

        public void AddNode(LogicNode logicNode)
        {
            LogicNodes.Add(logicNode);
        }

        public void RemoveNode(LogicNode logicNode)
        {
            LogicNodes.Remove(logicNode);
        }
        
        public void RunNode(LogicNode logicNode)
        {
            _env.RunNode(logicNode);
        }

        public void StopNode(LogicNode logicNode)
        {
            _env.StopNode(logicNode);
        }

        public LogicNode GetLogicNodByIndex(int index)
        {
            if (index < 0 || index >= LogicNodes.Count)
            {
                Debug.LogError("LogicNodeContainer: GetLogicNodByIndex index out of range");
                return null;
            }
            return LogicNodes[index];
        }
    }

}