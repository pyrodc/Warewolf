using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Dev2;
using Dev2.Activities;
using Dev2.Common.Interfaces;
using Dev2.Common.Interfaces.Diagnostics.Debug;
using Dev2.Communication;
using Dev2.Data.SystemTemplates.Models;
using Dev2.Studio.Core.Activities.Utils;
using Unlimited.Applications.BusinessDesignStudio.Activities;

namespace Warewolf.Studio.ViewModels
{

    public class DebugStateProcessor : IProcessor
    {
        private readonly IDebugState _debugState;
        private readonly IServiceTestModel _selectedServiceTest;
        private readonly ModelItem _modelItem;

        public DebugStateProcessor(IDebugState debugState, IServiceTestModel selectedServiceTest, ModelItem modelItem=null)
        {
            _debugState = debugState;
            _selectedServiceTest = selectedServiceTest;
            _modelItem = modelItem;
        }

        #region Implementation of IProcessor

        public void Process()
        {
            
            if (!string.IsNullOrEmpty(_debugState?.ActualType))
            {
                var type = Type.GetType(_debugState.ActualType);
                if (type == typeof(Flowchart) || type == typeof(ActivityBuilder))
                {
                    return;
                }
                if (type == typeof(DsfForEachActivity))
                {
                    ProcessForEach(_modelItem);
                }
                else if (type == typeof(DsfSequenceActivity))
                {
                    ProcessSequence(_modelItem);
                }
                else
                {
                    ProcessActivity(_debugState);
                }
            }
        }

        private void ProcessSequence(ModelItem modelItem)
        {
            var sequence = modelItem?.GetCurrentValue() as DsfSequenceActivity;
            AddSequence(sequence, _selectedServiceTest.TestSteps);
        }

        private void ProcessForEach(ModelItem modelItem)
        {
            var forEachActivity = modelItem?.GetCurrentValue() as DsfForEachActivity;
            AddForEach(forEachActivity, _selectedServiceTest.TestSteps);
        }

        private void AddForEach(DsfForEachActivity forEachActivity, ObservableCollection<IServiceTestStep> serviceTestSteps)
        {
            if (forEachActivity != null)
            {
                var uniqueId = forEachActivity.UniqueID;
                var exists = serviceTestSteps.FirstOrDefault(a => a.UniqueId.ToString() == uniqueId);

                if (exists == null)
                {
                    var testStep = new ServiceTestStep(Guid.Parse(uniqueId), typeof(DsfForEachActivity).Name, new List<IServiceTestOutput>(), StepType.Mock)
                    {
                        StepDescription = forEachActivity.DisplayName
                    };
                    var act = forEachActivity.DataFunc.Handler as DsfNativeActivity<string>;
                    if (act != null)
                    {
                        if (act.GetType() == typeof(DsfSequenceActivity))
                        {
                            AddSequence(act as DsfSequenceActivity, testStep.Children);
                        }
                        else if (act.GetType() == typeof(DsfForEachActivity))
                        {
                            AddForEach(forEachActivity.DataFunc.Handler as DsfForEachActivity, testStep.Children);
                        }
                        else
                        {
                            AddChildActivity(act, testStep);
                        }
                    }
                    serviceTestSteps.Add(testStep);
                }
            }
        }

        private void AddSequence(DsfSequenceActivity sequence, ObservableCollection<IServiceTestStep> serviceTestSteps)
        {
            if (sequence != null)
            {
                var uniqueId = sequence.UniqueID;
                var exists = serviceTestSteps.FirstOrDefault(a => a.UniqueId.ToString() == uniqueId);

                if (exists == null)
                {
                    var testStep = new ServiceTestStep(Guid.Parse(uniqueId), typeof(DsfSequenceActivity).Name, new List<IServiceTestOutput>(), StepType.Mock)
                    {
                        StepDescription = sequence.DisplayName
                    };
                    foreach (var activity in sequence.Activities)
                    {
                        var act = activity as DsfNativeActivity<string>;
                        if (act != null)
                        {
                            if (act.GetType() == typeof(DsfSequenceActivity))
                            {
                                AddSequence(act as DsfSequenceActivity, testStep.Children);
                            }
                            else
                            {
                                AddChildActivity(act, testStep);
                            }
                        }
                        else
                        {
                            if (activity.GetType() == typeof(DsfForEachActivity))
                            {
                                AddForEach(activity as DsfForEachActivity, testStep.Children);
                            }
                        }
                    }
                    serviceTestSteps.Add(testStep);
                }
            }
        }

        private static void AddChildActivity(DsfNativeActivity<string> act, ServiceTestStep testStep)
        {
            var outputs = act.GetOutputs();
            if (outputs != null && outputs.Count > 0)
            {
                var serviceTestOutputs = outputs.Select(output => new ServiceTestOutput(output, "")
                {
                    HasOptionsForValue = false
                }).Cast<IServiceTestOutput>().ToList();

                var serviceTestStep = new ServiceTestStep(Guid.Parse(act.UniqueID), act.GetType().Name, serviceTestOutputs, StepType.Mock)
                {
                    StepDescription = act.DisplayName,
                    Parent = testStep
                };
                testStep.Children.Add(serviceTestStep);
            }
        }

        private void ProcessSwitch(IDebugState debugState)
        {
            var modelItem = ModelItemUtils.CreateModelItem(new object());
            var cases = modelItem.GetProperty("Switches") as Dictionary<string, IDev2Activity>;
            var defaultCase = modelItem.GetProperty("Default") as IDev2Activity;
            var uniqueId = modelItem.GetProperty("UniqueID").ToString();
            var exists = _selectedServiceTest.TestSteps.FirstOrDefault(a => a.UniqueId.ToString() == uniqueId);

            if (exists == null)
            {
                if (_selectedServiceTest != null)
                {
                    var switchOptions = cases?.Select(pair => pair.Key).ToList();
                    if (defaultCase != null)
                    {
                        switchOptions.Insert(0, "Default");
                    }
                    var serviceTestOutputs = new List<IServiceTestOutput>();
                    var serviceTestOutput = new ServiceTestOutput("Condition Result", "")
                    {
                        HasOptionsForValue = true,
                        OptionsForValue = switchOptions
                    };
                    serviceTestOutputs.Add(serviceTestOutput);
                    _selectedServiceTest.AddTestStep(uniqueId, modelItem.GetProperty("DisplayName").ToString(), typeof(DsfSwitch).Name, serviceTestOutputs);
                }
            }
        }

        private void ProcessFlowSwitch(IDebugState debugState)
        {
            var modelItem = ModelItemUtils.CreateModelItem(new object());
            var condition = modelItem.GetProperty("Expression");
            var activity = (DsfFlowNodeActivity<string>)condition;
            var flowSwitch = modelItem.GetCurrentValue() as FlowSwitch<string>;
            var cases = flowSwitch.Cases;
            var defaultCase = flowSwitch.Default;
            var uniqueId = activity.UniqueID;
            var exists = _selectedServiceTest.TestSteps.FirstOrDefault(a => a.UniqueId.ToString() == uniqueId);

            if (exists == null)
            {
                if (_selectedServiceTest != null)
                {
                    var switchOptions = cases?.Select(pair => pair.Key).ToList();
                    if (defaultCase != null)
                    {
                        switchOptions.Insert(0, "Default");
                    }
                    var serviceTestOutputs = new List<IServiceTestOutput>();
                    var serviceTestOutput = new ServiceTestOutput("Condition Result", "")
                    {
                        HasOptionsForValue = true,
                        OptionsForValue = switchOptions
                    };
                    serviceTestOutputs.Add(serviceTestOutput);
                    _selectedServiceTest.AddTestStep(uniqueId, flowSwitch.DisplayName, typeof(DsfSwitch).Name, serviceTestOutputs);
                }
            }
        }

        private void ProcessActivity(IDebugState debugState)
        {
            List<KeyValuePair<string, string>> outputs = new List<KeyValuePair<string, string>>();
            foreach (var debugItem in debugState.Outputs)
            {
                foreach (var debugItemResult in debugItem.ResultsList)
                {
                    if(string.IsNullOrEmpty(debugItemResult.Variable))
                        continue;
                    outputs.Add(new KeyValuePair<string, string>(debugItemResult.Variable, debugItemResult.Value));
                }
            }
            var activityTypeName = debugState.DisplayName;

            var exists = _selectedServiceTest.TestSteps.FirstOrDefault(a => a.UniqueId.ToString().Equals(debugState.ID.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (exists == null)
            {
                if (outputs.Count > 0)
                {
                    var serviceTestOutputs = outputs.Select(output => new ServiceTestOutput(output.Key, output.Value)
                    {
                        HasOptionsForValue = false
                    }).Cast<IServiceTestOutput>().ToList();
                    //Remove the empty row
                    serviceTestOutputs.RemoveAt(serviceTestOutputs.Count - 1);
                    _selectedServiceTest.AddTestStep(debugState.ID.ToString(), debugState.DisplayName, activityTypeName, serviceTestOutputs);
                }
            }
        }


        private void ProcessDecision(IDebugState debugState)
        {

            Dev2DecisionStack dds = debugState as Dev2DecisionStack;
            //var uniqueId = modelItem.GetProperty("UniqueID").ToString();
            var exists = _selectedServiceTest.TestSteps.FirstOrDefault(a => a.UniqueId.ToString() == debugState.ID.ToString());

            if (exists == null)
            {
                if (_selectedServiceTest != null)
                {
                    var serviceTestOutputs = new List<IServiceTestOutput>();
                    var serviceTestOutput = new ServiceTestOutput("Condition Result", "")
                    {
                        HasOptionsForValue = true,
                        OptionsForValue = new List<string> { dds.TrueArmText, dds.FalseArmText }
                    };
                    serviceTestOutputs.Add(serviceTestOutput);
                    _selectedServiceTest.AddTestStep(debugState.ID.ToString(), debugState.DisplayName, typeof(DsfDecision).Name, serviceTestOutputs);
                }
            }
        }

        private void ProcessFlowDecision(IDebugState debugState)
        {
            //var condition = debugState.GetProperty("Condition");
            //var activity = (DsfFlowNodeActivity<bool>)condition;
            var activity = new DsfActivity();
            var expression = "activity.ExpressionText";
            if (expression != null)
            {
                var eval = Dev2DecisionStack.ExtractModelFromWorkflowPersistedData(expression);

                if (!string.IsNullOrEmpty(eval))
                {
                    Dev2JsonSerializer ser = new Dev2JsonSerializer();
                    var dds = ser.Deserialize<Dev2DecisionStack>(eval);
                    var uniqueId = debugState.ID;
                    var exists = _selectedServiceTest.TestSteps.FirstOrDefault(a => a.UniqueId.ToString() == uniqueId.ToString());

                    if (exists == null)
                    {
                        if (_selectedServiceTest != null)
                        {
                            var serviceTestOutputs = new List<IServiceTestOutput>();
                            var serviceTestOutput = new ServiceTestOutput("Condition Result", "")
                            {
                                HasOptionsForValue = true,
                                OptionsForValue = new List<string> { dds.TrueArmText, dds.FalseArmText }
                            };
                            serviceTestOutputs.Add(serviceTestOutput);
                            _selectedServiceTest.AddTestStep(uniqueId.ToString(), dds.DisplayText, typeof(DsfDecision).Name, serviceTestOutputs);
                        }
                    }
                }
            }
        }

        private static ModelItem RecursiveForEachCheck(dynamic activity)
        {
            var innerAct = activity.DataFunc.Handler as ModelItem;
            if (innerAct != null)
            {
                if (innerAct.ItemType == typeof(DsfForEachActivity))
                {
                    innerAct = RecursiveForEachCheck(innerAct);
                }
            }
            return innerAct;
        }

        #endregion
    }
}