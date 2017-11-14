﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Dev2.Activities;
using Dev2.Common.ExtMethods;
using Dev2.Common.Interfaces;
using Dev2.Common.Interfaces.Studio.Controller;
using Dev2.Studio.Core;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Models;
using Dev2.Studio.Interfaces;
using Dev2.ViewModels.Merge;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Warewolf.MergeParser;
using Dev2.Util;

namespace Dev2.Integration.Tests.Merge
{
    [TestClass]
    public class MergeWorkflowViewModelIntergrationTests
    {
        readonly IServerRepository _server = ServerRepository.Instance;

        [TestInitialize]
        [Owner("Nkosinathi Sangweni")]
        public void Init()
        {
            AppSettings.LocalHost = "http://localhost:1245";
            _server.Source.ResourceRepository.ForceLoad();
            var mockApplicationAdapter = new Mock<IApplicationAdaptor>();
            var mockPopupController = new Mock<IPopupController>();
            var mockServer = new Mock<IServer>();
            var mockShellViewModel = new Mock<IShellViewModel>();
            var mockServerRepository = new Mock<IServerRepository>();
            mockServerRepository.Setup(a => a.IsLoaded).Returns(true);
            CustomContainer.Register<IActivityParser>(new ActivityParser());
            CustomContainer.Register(mockApplicationAdapter.Object);
            CustomContainer.Register(mockPopupController.Object);
            CustomContainer.Register(_server);
            CustomContainer.Register(mockShellViewModel.Object);
            CustomContainer.Register(mockServerRepository.Object);
            var mockParseServiceForDifferences = new ServiceDifferenceParser();
            CustomContainer.Register<IServiceDifferenceParser>(mockParseServiceForDifferences);
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void Initialize_GivenSameResourceModel_ShouldHaveNoDeifferences_desicion()
        {
            //---------------Set up test pack-------------------
            var helloWorldGuid = "41617daa-509e-40eb-aa76-b0827028721d".ToGuid();
            var loadContextualResourceModel = _server.Source.ResourceRepository.LoadContextualResourceModel(helloWorldGuid);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(loadContextualResourceModel);
            //---------------Execute Test ----------------------
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(loadContextualResourceModel, loadContextualResourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void Initialize_GivenSameResourceModel_ShouldHaveNoDeifferences_Switch()
        {
            //---------------Set up test pack-------------------
            var resourceId = "6dcdd72f-c4ba-484d-9806-8134d8eb2447".ToGuid();
            var loadContextualResourceModel = _server.Source.ResourceRepository.LoadContextualResourceModel(resourceId);
            var resourceModel = new ResourceModel(_server.Source) { ID = resourceId };
            var xElement = XML.XmlResource.Fetch("SameResourceSwitch");
            var element = xElement.Element("Action");
            Assert.IsNotNull(element);
            var xamlDef = element.ToString(SaveOptions.DisableFormatting);
            resourceModel.WorkflowXaml = new StringBuilder(xamlDef);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(loadContextualResourceModel, resourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        [Ignore]
        public void Batch_Compare_All_Examples_Have_No_Differences()
        {
            //---------------Set up test pack-------------------
            var environmentModel = _server.Source;
            environmentModel.Connect();
            var resourceRepository = _server.Source.ResourceRepository;

            resourceRepository.Load();
            var resourceModels = resourceRepository.All();

            Assert.IsNotNull(resourceModels);
            var examples = resourceModels.Where(model => model.GetSavePath().StartsWith("Examples")).ToList();

            foreach (var example in examples)
            {
                //---------------Assert Precondition----------------
                //---------------Execute Test ----------------------
                var contextualResourceModel = example as IContextualResourceModel;
                try
                {
                    var mergeWorkflowViewModel =
                        new MergeWorkflowViewModel(contextualResourceModel, contextualResourceModel, true);
                    //---------------Test Result -----------------------
                    Assert.IsNotNull(mergeWorkflowViewModel);
                    var conflicts = mergeWorkflowViewModel.Conflicts;
                    var all = conflicts.All(conflict =>
                    {
                        var toolConflict = conflict as IToolConflict;
                        return toolConflict.DiffViewModel == null;
                    });
                    if (all)
                        Assert.IsTrue(all);
                    else
                    {
                        Debug.WriteLine(example.ID + " " + example.DisplayName + " Has some differences ");
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(example.ID + " " + example.DisplayName + " Has some differences " + e.Message);
                }
            }
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void Example_ControlFlowDecision_No_Differences_TreeHierachyIsCorrect()
        {
            //---------------Set up test pack-------------------
            var environmentModel = _server.Source;
            environmentModel.Connect();
            var resourceRepository = _server.Source.ResourceRepository;
            resourceRepository.Load();

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var contextualResourceModel =
                resourceRepository.LoadContextualResourceModel("41617daa-509e-40eb-aa76-b0827028721d".ToGuid());
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(contextualResourceModel, contextualResourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);

            var all = mergeWorkflowViewModel.Conflicts.All(conflict => !conflict.HasConflict);
            Assert.IsTrue(all);
            var conflictsCount = mergeWorkflowViewModel.Conflicts.Count;
            Assert.AreEqual(3, conflictsCount);
            var completeConflict1 = mergeWorkflowViewModel.Conflicts.First;
            Assert.IsNotNull(completeConflict1);

            var toolConflict1 = completeConflict1.Value as IToolConflict;
            Assert.IsTrue(!toolConflict1.CurrentViewModel.Children.Any());
            Assert.AreEqual("Use the Decision tool to:", toolConflict1.CurrentViewModel.MergeDescription);

            var completeConflict2 = completeConflict1.Next;
            Assert.IsNotNull(completeConflict2);
            var toolConflict2 = completeConflict2.Value as IToolConflict;
            Assert.IsTrue(!toolConflict2.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 1 - Basic Usage", toolConflict2.CurrentViewModel.MergeDescription);

            var completeConflict3 = completeConflict2.Next;
            Assert.IsNotNull(completeConflict3);
            var toolConflict3 = completeConflict3.Value as IToolConflict;
            Assert.IsTrue(toolConflict3.CurrentViewModel.Children.Any());
            Assert.AreEqual("Decision", toolConflict3.CurrentViewModel.MergeDescription);

            var childrenCount = toolConflict3.CurrentViewModel.Children.Count;
            Assert.AreEqual(4, childrenCount);

            var mergeToolModel = toolConflict3.CurrentViewModel.Children.Single(model => model.MergeDescription == "Over 18");
            Assert.AreEqual(4, mergeToolModel.Children.Count);
            AsserthildrenHasChild(mergeToolModel.Children, "Correct Result");
            AsserthildrenHasChild(mergeToolModel.Children, "Incorrect Result");
            AsserthildrenHasChild(mergeToolModel.Children, "EXAMPLE 3 - Error Checking");
            AsserthildrenHasChild(mergeToolModel.Children, "Was there an error?");

            var toolModel = mergeToolModel.Children.Single(model => model.MergeDescription == "Was there an error?");
            var count = toolModel.Children.Count;
            Assert.AreEqual(5, count);
            AsserthildrenHasChild(toolModel.Children, "Correct Result");
            AsserthildrenHasChild(toolModel.Children, "Incorrect Result");
            AsserthildrenHasChild(toolModel.Children, "EXAMPLE 4 - Recordsets");
            AsserthildrenHasChild(toolModel.Children, "Assign (3)");
            AsserthildrenHasChild(toolModel.Children, "If [[rec(*).set]] Is Numeric");

            var single = toolModel.Children.Single(model => model.MergeDescription == "If [[rec(*).set]] Is Numeric");
            var childCount = single.Children.Count;
            Assert.AreEqual(2, childCount);
            var trueArmTools = single.Children.Count(model => model.ParentDescription == "True");
            var falseArmTools = single.Children.Count(model => model.ParentDescription == "False");
            Assert.AreEqual(1, trueArmTools);
            Assert.AreEqual(1, falseArmTools);
            AsserthildrenHasChild(single.Children, "Incorrect");
            AsserthildrenHasChild(single.Children, "Correct Result");
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void MergeCurrent_HelloWorld_No_Differences_TreeHierachyIsCorrect()
        {
            //---------------Set up test pack-------------------
            var environmentModel = _server.Source;
            environmentModel.Connect();
            var resourceRepository = _server.Source.ResourceRepository;
            resourceRepository.Load();

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var contextualResourceModel =
                resourceRepository.LoadContextualResourceModel("9e9660d8-1a3c-45ab-a330-673c2343e517".ToGuid());
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(contextualResourceModel, contextualResourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);

            var all = mergeWorkflowViewModel.Conflicts.All(conflict => !conflict.HasConflict);
            Assert.IsTrue(all);
            var conflictsCount = mergeWorkflowViewModel.Conflicts.Count;
            Assert.AreEqual(4, conflictsCount);
            var completeConflict1 = mergeWorkflowViewModel.Conflicts.First;
            Assert.IsNotNull(completeConflict1);
            var toolConflict1 = completeConflict1.Value as IToolConflict;
            Assert.IsTrue(!toolConflict1.CurrentViewModel.Children.Any());
            Assert.AreEqual("Use the Switch tool to:", toolConflict1.CurrentViewModel.MergeDescription);

            var completeConflict2 = completeConflict1.Next;
            Assert.IsNotNull(completeConflict2);
            var toolConflict2 = completeConflict2.Value as IToolConflict;
            Assert.IsTrue(!toolConflict2.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 1 - Basic Usage", toolConflict2.CurrentViewModel.MergeDescription);

            var completeConflict3 = completeConflict2.Next;
            Assert.IsNotNull(completeConflict3);
            var toolConflict3 = completeConflict3.Value as IToolConflict;
            Assert.IsFalse(toolConflict3.CurrentViewModel.Children.Any());
            Assert.AreEqual("Assign (1)", toolConflict3.CurrentViewModel.MergeDescription);

            var completeConflict4 = completeConflict3.Next;
            Assert.IsNotNull(completeConflict4);
            var toolConflict4 = completeConflict4.Value as IToolConflict;
            Assert.IsTrue(toolConflict4.CurrentViewModel.Children.Any());
            Assert.AreEqual("[[DiceRollValue]]", toolConflict4.CurrentViewModel.MergeDescription);

            var childrenCount = toolConflict4.CurrentViewModel.Children.Count;
            Assert.AreEqual(7, childrenCount);

            var mergeToolModels = toolConflict4.CurrentViewModel.Children;
            AsserthildrenHasChild(mergeToolModels, "Incorrect", 6);
            AsserthildrenHasChild(mergeToolModels, "Correct", 1);
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void Example_ControlFlowSequence_No_Differences_TreeHierachyIsCorrect()
        {
            //---------------Set up test pack-------------------
            var environmentModel = _server.Source;
            environmentModel.Connect();
            var resourceRepository = _server.Source.ResourceRepository;
            resourceRepository.Load();

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var contextualResourceModel =
                resourceRepository.LoadContextualResourceModel("1b0e0881-9869-4b71-b853-e0c752c38678".ToGuid());
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(contextualResourceModel, contextualResourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);

            var all = mergeWorkflowViewModel.Conflicts.All(conflict => !conflict.HasConflict);
            Assert.IsTrue(all);
            var conflictsCount = mergeWorkflowViewModel.Conflicts.Count;
            Assert.AreEqual(6, conflictsCount);

            var completeConflict1 = mergeWorkflowViewModel.Conflicts.First;
            Assert.IsNotNull(completeConflict1);
            var toolConflict1 = completeConflict1.Value as IToolConflict;
            Assert.IsTrue(!toolConflict1.CurrentViewModel.Children.Any());
            Assert.AreEqual("Use the Sequence tool to:", toolConflict1.CurrentViewModel.MergeDescription);

            var completeConflict2 = completeConflict1.Next;
            Assert.IsNotNull(completeConflict2);
            var toolConflict2 = completeConflict2.Value as IToolConflict;
            Assert.IsTrue(!toolConflict2.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 1 - Basic Usage", toolConflict2.CurrentViewModel.MergeDescription);

            var completeConflict3 = completeConflict2.Next;
            Assert.IsNotNull(completeConflict3);
            var toolConflict3 = completeConflict3.Value as IToolConflict;
            Assert.IsFalse(toolConflict3.CurrentViewModel.Children.Any());
            Assert.AreEqual("Create Example Data (1)", toolConflict3.CurrentViewModel.MergeDescription);

            var completeConflict4 = completeConflict3.Next;
            Assert.IsNotNull(completeConflict4);
            var toolConflict4 = completeConflict4.Value as IToolConflict;
            Assert.IsTrue(toolConflict4.CurrentViewModel.Children.Any());
            Assert.AreEqual("Organize Customers", toolConflict4.CurrentViewModel.MergeDescription);
            var childrenCount = toolConflict4.CurrentViewModel.Children.Count;
            Assert.AreEqual(3, childrenCount);
            AsserthildrenHasChild(toolConflict4.CurrentViewModel.Children, "Split Names (3)");
            AsserthildrenHasChild(toolConflict4.CurrentViewModel.Children, "Find Only Unique Names");
            AsserthildrenHasChild(toolConflict4.CurrentViewModel.Children, "Sort Names Alphabetically");

            var completeConflict5 = completeConflict4.Next;
            Assert.IsNotNull(completeConflict5);
            var toolConflict5 = completeConflict5.Value as IToolConflict;
            Assert.IsFalse(toolConflict5.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 2 - Advanced Usage", toolConflict5.CurrentViewModel.MergeDescription);

            var completeConflict6 = completeConflict5.Next;
            Assert.IsNotNull(completeConflict6);
            var toolConflict6 = completeConflict6.Value as IToolConflict;
            Assert.IsTrue(toolConflict6.CurrentViewModel.Children.Any());
            Assert.AreEqual("For Each", toolConflict6.CurrentViewModel.MergeDescription);

            var childrenCount5 = toolConflict6.CurrentViewModel.Children.Count;
            Assert.AreEqual(1, childrenCount5);

            var mergeToolModel = toolConflict6.CurrentViewModel.Children.Single();
            Assert.AreEqual(2, mergeToolModel.Children.Count);
            AsserthildrenHasChild(mergeToolModel.Children, "Convert Case To Title Case (2)");
            AsserthildrenHasChild(mergeToolModel.Children, "Create New Email Addresses (3)");
        }

        [TestMethod]
        [Owner("Nkosinathi Sangweni")]
        public void Example_LoopConstructsForEach_No_Differences_TreeHierachyIsCorrect()
        {
            //---------------Set up test pack-------------------
            var environmentModel = _server.Source;
            environmentModel.Connect();
            var resourceRepository = _server.Source.ResourceRepository;
            resourceRepository.Load();

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var contextualResourceModel =
                resourceRepository.LoadContextualResourceModel("a3ad09e1-a058-4dc1-af6a-b4d856dc0e52".ToGuid());
            var mergeWorkflowViewModel = new MergeWorkflowViewModel(contextualResourceModel, contextualResourceModel, true);
            //---------------Test Result -----------------------
            Assert.IsNotNull(mergeWorkflowViewModel);

            var all = mergeWorkflowViewModel.Conflicts.All(conflict => !conflict.HasConflict);
            Assert.IsTrue(all);
            var conflictsCount = mergeWorkflowViewModel.Conflicts.Count;
            Assert.AreEqual(7, conflictsCount);

            var completeConflict1 = mergeWorkflowViewModel.Conflicts.First;
            Assert.IsNotNull(completeConflict1);
            var toolConflict1 = completeConflict1.Value as IToolConflict;
            Assert.IsTrue(!toolConflict1.CurrentViewModel.Children.Any());
            Assert.AreEqual("Use the For Each tool to:", toolConflict1.CurrentViewModel.MergeDescription);

            var completeConflict2 = completeConflict1.Next;
            Assert.IsNotNull(completeConflict2);
            var toolConflict2 = completeConflict2.Value as IToolConflict;
            Assert.IsTrue(!toolConflict2.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 1 - No. of Executions", toolConflict2.CurrentViewModel.MergeDescription);

            var completeConflict3 = completeConflict2.Next;
            Assert.IsNotNull(completeConflict3);
            var toolConflict3 = completeConflict3.Value as IToolConflict;
            Assert.IsTrue(toolConflict3.CurrentViewModel.Children.Any());
            Assert.AreEqual("For Each", toolConflict3.CurrentViewModel.MergeDescription);
            var childrenCount = toolConflict3.CurrentViewModel.Children.Count;
            Assert.AreEqual(1, childrenCount);
            AsserthildrenHasChild(toolConflict3.CurrentViewModel.Children, "Random");

            var completeConflict4 = completeConflict3.Next;
            Assert.IsNotNull(completeConflict4);
            var toolConflict4 = completeConflict4.Value as IToolConflict;
            Assert.IsTrue(!toolConflict4.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 2 - * in Range", toolConflict4.CurrentViewModel.MergeDescription);

            var completeConflict5 = completeConflict4.Next;
            Assert.IsNotNull(completeConflict5);
            var toolConflict5 = completeConflict5.Value as IToolConflict;
            Assert.IsTrue(toolConflict5.CurrentViewModel.Children.Any());
            Assert.AreEqual("For Each", toolConflict5.CurrentViewModel.MergeDescription);
            var childrenCount1 = toolConflict5.CurrentViewModel.Children.Count;
            Assert.AreEqual(1, childrenCount1);
            AsserthildrenHasChild(toolConflict5.CurrentViewModel.Children, "Random");

            var completeConflict6 = completeConflict5.Next;
            Assert.IsNotNull(completeConflict6);
            var toolConflict6 = completeConflict6.Value as IToolConflict;
            Assert.IsTrue(!toolConflict6.CurrentViewModel.Children.Any());
            Assert.AreEqual("EXAMPLE 3 - * in CSV", toolConflict6.CurrentViewModel.MergeDescription);

            var completeConflict7 = completeConflict6.Next;
            Assert.IsNotNull(completeConflict7);
            var toolConflict7 = completeConflict7.Value as IToolConflict;
            Assert.IsTrue(toolConflict7.CurrentViewModel.Children.Any());
            Assert.AreEqual("For Each", toolConflict7.CurrentViewModel.MergeDescription);
            var childrenCount2 = toolConflict7.CurrentViewModel.Children.Count;
            Assert.AreEqual(1, childrenCount2);
            AsserthildrenHasChild(toolConflict7.CurrentViewModel.Children, "Random");
        }


        private void AsserthildrenHasChild(ObservableCollection<IMergeToolModel> children, string description)
        {
            var count = children.Count(model =>
                model.MergeDescription.Equals(description, StringComparison.Ordinal));
            Assert.AreEqual(1, count);
        }
        private void AsserthildrenHasChild(ObservableCollection<IMergeToolModel> children, string description, int numberOfChildren)
        {
            var count = children.Count(model =>
                model.MergeDescription.Equals(description, StringComparison.Ordinal));
            Assert.AreEqual(numberOfChildren, count);
        }
    }
}