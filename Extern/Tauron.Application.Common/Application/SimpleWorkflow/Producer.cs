using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.SimpleWorkflow
{
    [PublicAPI]
    public abstract class Producer<TState, TContext>
        where TState : IStep<TContext>
    {
        [PublicAPI]
        public class StepConfiguration
        {
            private readonly StepRev _context;

            internal StepConfiguration([NotNull] StepRev context)
            {
                _context = context;
            }

            [NotNull]
            public StepConfiguration AddCondition([NotNull] ICondition<TContext> condition)
            {
                _context.Conditions.Add(condition);
                return this;
            }

            [NotNull]
            public ConditionConfiguration AddCondition([CanBeNull] Func<TContext, IStep<TContext>, bool> guard = null)
            {
                var con = new SimpleCondition<TContext> {Guard = guard};
                if (guard != null) return new ConditionConfiguration(AddCondition(con), con);

                _context.GenericCondition = con;
                return new ConditionConfiguration(this, con);
            }
        }

        public class ConditionConfiguration
        {
            private readonly SimpleCondition<TContext> _condition;
            private readonly StepConfiguration _config;

            public ConditionConfiguration([NotNull] StepConfiguration config, [NotNull] SimpleCondition<TContext> condition)
            {
                _config = config;
                _condition = condition;
            }

            [NotNull]
            public StepConfiguration GoesTo(StepId id)
            {
                _condition.Target = id;

                return _config;
            }
        }

        internal class StepRev
        {
            public StepRev()
            {
                Conditions = new List<ICondition<TContext>>();
            }

            public TState State { get; set; }

            [NotNull]
            public List<ICondition<TContext>> Conditions { get; }

            [CanBeNull]
            public ICondition<TContext> GenericCondition { get; set; }

            public override string ToString()
            {
                var b = new StringBuilder();
                b.Append(State);

                foreach (var condition in Conditions)
                    b.AppendLine("->" + condition + ";");

                if (GenericCondition != null) b.Append("Generic->" + GenericCondition + ";");

                return b.ToString();
            }
        }

        private readonly Dictionary<StepId, StepRev> _states;

        private StepId _lastId;

        private string _errorMessage = string.Empty;

        protected Producer()
        {
            _states = new Dictionary<StepId, StepRev>();
        }

        public void Begin(StepId id, TContext context)
        {
            Process(id, context);

            if (_lastId.Name == StepId.Invalid.Name) throw new InvalidOperationException(_errorMessage);
        }

        [DebuggerStepThrough]
        protected bool SetLastId(StepId id)
        {
            _lastId = id;
            return _lastId.Name == StepId.Finish.Name || _lastId.Name == StepId.Invalid.Name;
        }

        protected virtual bool Process(StepId id, TContext context)
        {
            if (SetLastId(id)) return true;

            StepRev rev;
            if (!_states.TryGetValue(id, out rev))
                return SetLastId(StepId.Invalid);

            var sId = rev.State.OnExecute(context);
            var result = false;

            switch (sId.Name)
            {
                case "Invalid":
                    _errorMessage = rev.State.ErrorMessage;
                    return SetLastId(sId);
                case "None":
                    result = ProgressConditions(rev, context);
                    break;
                case "Loop":
                    var ok = true;

                    do
                    {
                        var loopId = rev.State.NextElement(context);
                        if (loopId.Name == StepId.LoopEnd.Name)
                        {
                            ok = false;
                            continue;
                        }
                        if (loopId.Name == StepId.Invalid.Name) return SetLastId(StepId.Invalid);

                        ProgressConditions(rev, context);
                    } while (ok);
                    break;
                case "Finish":
                case "Skip":
                    result = true;
                    break;
                default:
                    return SetLastId(StepId.Invalid);
            }

            if (!result)
                rev.State.OnExecuteFinish(context);

            return result;
        }

        private bool ProgressConditions([NotNull] StepRev rev, TContext context)
        {
            var std = (from con in rev.Conditions
                let stateId = con.Select(rev.State, context)
                where stateId.Name != StepId.None.Name
                select stateId).ToArray();

            if (std.Length != 0) return std.Any(id => Process(id, context));

            if (rev.GenericCondition == null) return false;
            var cid = rev.GenericCondition.Select(rev.State, context);
            return cid.Name != StepId.None.Name && Process(cid, context);
        }

        [NotNull]
        public StepConfiguration SetStep(TState stade)
        {
            var rev = new StepRev {State = stade};
            _states[stade.Id] = rev;

            return new StepConfiguration(rev);
        }

        [NotNull]
        public StepConfiguration GetStateConfiguration(StepId id)
        {
            return new StepConfiguration(_states[id]);
        }
    }
}