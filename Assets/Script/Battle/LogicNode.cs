using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.Battle
{
    public class LogicCondition
    {
        public LogicCondition(string conditionType, params object[] conditionParams)
        {
            this.conditionType = conditionType;
            this.conditionParams = conditionParams;
        }
        public string conditionType;
        public object[] conditionParams;
    }

    public class LogicAction
    {
        public LogicAction(string actionType, params object[] actionParams)
        {
            this.actionType = actionType;
            this.actionParams = actionParams;
        }
        public string actionType;
        public object[] actionParams;
    }
    
    public class LogicTrigger
    {
        public ILogicTriggerEnv env;
        public string triggerType;
        public List<LogicCondition> conditions;
        public List<LogicAction> actions;

        public LogicTrigger(ILogicTriggerEnv env, string triggerType, List<LogicCondition> conditions,
            List<LogicAction> actions)
        {
            this.env = env;
            this.triggerType = triggerType;
            this.conditions = conditions;
            this.actions = actions;
        }

        public bool CheckConditions()
        {
            foreach (var condition in conditions)
            {
                switch (condition.conditionType)
                {
                    case "TestConditionTrue":
                        break;
                    case "TestConditionFalse":
                        return false;
                }
            }
            return true;
        }

        public void DoActions()
        {
            foreach (var action in actions)
            {
                switch (action.actionType)
                {
                    // TODO: Implement action execution
                    case "StopNode":
                        env.StopNode(env.GetHost());
                        break;
                    case "RunNode":
                        env.RunNode(env.GetLogicNodByIndex((int)action.actionParams[0]));
                        break;
                    case "LogInfo":
                        Debug.Log($"{action.actionParams[0].GetType()} " + action.actionParams[0]);
                        break;
                }
            }
        }
    }
    
    public interface ILogicTriggerEnv: ILogicNodeEnv
    {
        void RunNode(LogicNode logicNode);
        void StopNode(LogicNode logicNode);
        
        LogicNode GetHost();
    }
    public class LogicNode: ILogicTriggerEnv
    {
        private readonly List<LogicTrigger> _triggers;
        private readonly ILogicNodeEnv _env;

        public LogicNode(ILogicNodeEnv env, List<LogicTrigger> triggers)
        {
            this._triggers = triggers;
            this._env = env;
        }

        public void TestEvent()
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.triggerType == "TestEvent" && trigger.CheckConditions())
                {
                    trigger.DoActions();
                }
            }
        }
        
        public void Enter()
        {
            //需要一个映射关系,将triggerType绑定到对应的游戏事件
            BindEvents();
            OnEnter();
        }
        
        public void Exit()
        {
            //此时阶段结束解绑事件
            OnExit();
            UnbindEvents();
        }
        
        private void BindEvents()
        {
            // Test.TestEvent += TestEvent;
        }
        private void UnbindEvents()
        {
            // Test.TestEvent -= TestEvent;
        }
        
        public void OnEnter()
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.triggerType == "OnEnter" && trigger.CheckConditions())
                {
                    trigger.DoActions();
                }
            }
        }

        public void Tick()
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.triggerType == "Tick" && trigger.CheckConditions())
                {
                    trigger.DoActions();
                }
            }
        }
        
        public void OnExit()
        {
            foreach (var trigger in _triggers)
            {
                if (trigger.triggerType == "OnExit" && trigger.CheckConditions())
                {
                    trigger.DoActions();
                }
            }
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
            return _env.GetLogicNodByIndex(index);
        }

        public LogicNode GetHost()
        {
            return this;
        }
    }
    
}