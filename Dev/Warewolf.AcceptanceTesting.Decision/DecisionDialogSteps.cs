﻿using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using Dev2.Activities;
using Dev2.Activities.Designers2.Decision;
using Dev2.Data.SystemTemplates.Models;
using Dev2.Studio.Core.Activities.Utils;
using Dev2.TO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;
using Warewolf.AcceptanceTesting.Core;
// ReSharper disable All

namespace Warewolf.AcceptanceTesting.Decision
{
    [Binding]
    public class DecisionDialogSteps
    {
        [BeforeFeature()]
        public static void SetupForSystem()
        {
            Utils.SetupResourceDictionaryActivities();
            var tos = new List<DecisionTO>();
            FeatureContext.Current["Tos"] = tos;
            Dev2DecisionStack stack = new Dev2DecisionStack();
            var mi = CreateModelItem(tos);
            FeatureContext.Current["modelItem"] = mi;
            var large = new Large();
            FeatureContext.Current.Add("view",large);
            var vm = new DecisionDesignerViewModel(mi);
            FeatureContext.Current["viewModel"] = vm;
            var dataContext = vm;
            large.DataContext = dataContext;
            Utils.ShowTheViewForTesting(large);
        }

        [Given(@"I have a workflow ""(.*)""")]
        public void GivenIHaveAWorkflow(string p0)
        {

        }
       


        [Given(@"drop a ""(.*)"" tool onto the design surface")]
        public void GivenDropAToolOntoTheDesignSurface(string p0)
        {
            
        }
        void ConfigureDecisionExpression(ModelItem mi)
        {

        }

        static ModelItem CreateModelItem(IEnumerable<DecisionTO> items, string displayName = "Find")
        {

            var dec = new DsfDecision();
            var modelItem = ModelItemUtils.CreateModelItem(dec);



            FeatureContext.Current["decision"] = dec;
            modelItem.SetProperty("DisplayName", displayName);

            return modelItem;
        }
        [Given(@"the decision tool window is opened")]
        public void GivenTheDecisionToolWindowIsOpened()
        {

          
        }

        [Given(@"a decision variable ""(.*)"" value ""(.*)""")]
        public void GivenADecisionVariableValue(string p0, string p1)
        {
            
        }

        //[Given(@"a decision variable ""(.*)"" operation ""(.*)"" right  ""(.*)""")]
        //public void GivenADecisionVariableOperationRight(string left, string option, int right)
        //{
        //    var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
        //    ((DecisionTO)vm.Tos[0]).MatchValue = p0;
        //    ((DecisionTO)vm.Tos[0]).From = p1;
        //    ((DecisionTO)vm.Tos[0]).To = p2;
        //}

        [Given(@"a decision variable ""(.*)"" operation ""(.*)"" right  ""(.*)"" position ""(.*)""")]
        public void GivenADecisionVariableOperationRightPosition(string left, string match, string right, int pos)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            ((DecisionTO)vm.Tos[pos]).SearchCriteria = right;
            ((DecisionTO)vm.Tos[pos]).MatchValue = left;
            ((DecisionTO)vm.Tos[pos]).From = right;
            ((DecisionTO)vm.Tos[pos]).SearchType = match;
        }

        [When(@"I change decision variable position ""(.*)"" to ""(.*)""")]
        public void WhenIChangeDecisionVariablePositionTo(int pos, string match)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];

            ((DecisionTO)vm.Tos[pos]).MatchValue = match;

        }


        [Given(@"is ""(.*)"" ""(.*)"" ""(.*)""")]
        public void GivenIs(string left, string op, string right)
        {
           var tos = (List<DecisionTO>) ScenarioContext.Current["Tos"];
           tos.Add(new DecisionTO(left,op,right,tos.Count));
        
        }

        [Given(@"""(.*)"" is selected")]
        public void GivenIsSelected(string name)
        {
            var view = FeatureContext.Current["view"] as Large;
             view.DoneAction();
        }


        [Given(@"""(.*)"" is ""(.*)""")]
        public void GivenIs(string p0, string p1)
        {
            var view = FeatureContext.Current["view"] as Large;
            view.SetAllTrue(p1=="True");
        }

        [Then(@"""(.*)"" has a value of ""(.*)""")]
        public void ThenHasAValueOf(string p0, string p1)
        {
            var view = FeatureContext.Current["view"] as Large;
            bool b =  view.GetAllTrue();
            Assert.AreEqual(p1 == "True",b);
        }

        bool GetAllTrue()
        {
            return false;
        }

        [Given(@"Match Type equals ""(.*)""")]
        public void GivenMatchTypeEquals(string p0)
        {
         
        }

        [Given(@"I right click to view the context menu")]
        public void GivenIRightClickToViewTheContextMenu()
        {

        }

        [Given(@"I select ""(.*)""")]
        public void GivenISelect(string p0)
        {
           
        }

        [Given(@"""(.*)"" ""(.*)"" ""(.*)"" is removed from the decision")]
        public void GivenIsRemovedFromTheDecision(string p0, string p1, string p2)
        {

        }

        [Given(@"the decision has ""(.*)"" error")]
        public void GivenTheDecisionHasError(string p0)
        {

        }

        [Given(@"the decision tool has ""(.*)"" Error")]
        public void GivenTheDecisionToolHasError(string p0)
        {

        }

        [Given(@"Error message ""(.*)""\[\[A]]}"" is visible")]
        public void GivenErrorMessageAIsVisible(string p0)
        {
    
        }

        [Given(@"I select the ""(.*)"" menu")]
        public void GivenISelectTheMenu(string p0)
        {
       
        }

        [Given(@"Match Type has '(.*)' visible")]
        public void GivenMatchTypeHasVisible(string option)
        {
            var view = (Large)FeatureContext.Current["view"];
            view.VerifyOption(option);
        }

        [When(@"the decision tool is executed")]
        public void WhenTheDecisionToolIsExecuted()
        {
  
        }

        [When(@"I change decision variable ""(.*)"" to ""(.*)""")]
        public void WhenIChangeDecisionVariableTo(string p0, int p1)
        {
        
        }

        [When(@"""(.*)"" is selected")]
        public void WhenIsSelected(string action)
        {
            if(action == "Done")
            {
                var view = (Large)FeatureContext.Current["view"];
                view.ClickDone();
            }
        }

        [Then(@"the Decision window is opened")]
        [Given(@"the Decision window is opened")]
        public void ThenTheDecisionWindowIsOpened()
        {

        }

        [Then(@"'(.*)' fields are ""(.*)""")]
        public void ThenFieldsAre(string name, string state)
        {
            var view = (Large) FeatureContext.Current["view"];
            view.VerifyInputsAvailable();
        }

        [Then(@"an empty row has been added")]
        public void ThenAnEmptyRowHasBeenAdded()
        {
            var view = (Large)FeatureContext.Current["view"];
            view.VerifyEmptyRow();
        }

       
        [Then(@"the decision match variables '(.*)'and match '(.*)' and to match'(.*)'")]
        public void ThenTheDecisionMatchVariablesAndMatchAndToMatch(string p0, string p1, string p2)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            ((DecisionTO)vm.Tos[0]).MatchValue = p0;
            ((DecisionTO)vm.Tos[0]).From = p1;
            ((DecisionTO)vm.Tos[0]).To = p2;
        }

        [Then(@"MatchType  is '(.*)'")]
        public void ThenMatchTypeIs(string p0)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            ((DecisionTO)vm.Tos[0]).SearchType = p0;

        }


        [Then(@"the inputs are '(.*)'")]
        public void ThenTheInputsAre(string p0)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            var to =   ((DecisionTO)vm.Tos[0]);
            var vis = p0.Split(new char[] { ',' });
            switch(vis.Count())
            {
                case 2:
                    Assert.IsTrue(to.IsSearchCriteriaEnabled);
                    Assert.IsTrue(to.SearchType.Length>0);
                    Assert.IsFalse(to.IsBetweenCriteriaVisible);
                break;
                case 3:
                Assert.IsTrue(to.IsSearchCriteriaVisible);
                Assert.IsTrue(to.SearchType.Length > 0);
                Assert.IsFalse(to.IsBetweenCriteriaVisible);
                break;
                case 4:
                Assert.IsTrue(to.IsSearchCriteriaVisible);
                Assert.IsTrue(to.SearchType.Length > 0);
                Assert.IsTrue(to.IsBetweenCriteriaVisible);
                break;
                default:
                    Assert.Fail("unexpected test input");
                    break;
            }
        }



        [Then(@"""(.*)"" is visible")]
        public void ThenIsVisible(string p0)
        {
            
        }

        [Then(@"a decision variable '(.*)' value '(.*)'")]
        public void ThenADecisionVariableValue(string variable, string value2)
        {
          
        }

        [Then(@"the decision variables '(.*)' value '(.*)' and variable match '(.*)' value '(.*)' and variable match'(.*)' value '(.*)'")]
        public void ThenTheDecisionVariablesValueAndVariableMatchValueAndVariableMatchValue(string match, string p1, string p2, string p3, string p4, string p5)
        {
            ScenarioContext.Current.Pending();
        }


        [Then(@"check if '(.*)' ""(.*)"" '(.*)'")]
        public void ThenCheckIf(string p0, string p1, string p2)
        {
        
        }



        [Then(@"the Decision tool window is closed")]
        public void ThenTheDecisionToolWindowIsClosed()
        {
            //not required but reads nicely
    
        }

        [Then(@"""(.*)"" is visible in tool")]
        public void ThenIsVisibleInTool(string var)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
             var view = (Large)FeatureContext.Current["view"];
            string disp = view.GetDisplayName();
            Assert.IsTrue(disp.Contains(var));

        }

        [Then(@"I open the Decision tool window")]
        public void ThenIOpenTheDecisionToolWindow()
        {
        
        }

        [Then(@"decision variable ""(.*)"" is not visible")]
        public void ThenDecisionVariableIsNotVisible(string var)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
    
            Assert.IsFalse(vm.Tos.Any(a => ((DecisionTO)a).SearchCriteria.Contains(var)));
        }
        [When(@"I select the ""(.*)"" button at position (.*)")]
        public void WhenISelectTheButtonAtPosition(string p0, int p1)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            Assert.IsTrue(((DecisionTO)vm.Tos[p1]).CanDelete(((DecisionTO)vm.Tos[p1])));
            vm.DeleteRow((DecisionTO)vm.Tos[p1]);
            

        }

        [Then(@"""(.*)"" is removed from the decision")]
        public void ThenIsRemovedFromTheDecision(string var)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];

            Assert.IsFalse(vm.Tos.Any(a=> ((DecisionTO)a).SearchCriteria.Contains(var)));
        }



        [Then(@"""(.*)"" is visible in Match field")]
        public void ThenIsVisibleInMatchField(string var)
        {
            var vm = (DecisionDesignerViewModel)FeatureContext.Current["viewModel"];
            vm.GetExpressionText();
            Assert.IsTrue(vm.ExpressionText.Contains(var));
        }

        [Then(@"""(.*)"" is ""(.*)""")]
        public void ThenIs(string p0, string p1)
        {

        }
    }
}
