﻿using Dev2.Common;
using Dev2.Studio.Interfaces;
using Dev2.ViewModels.Merge;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dev2.Core.Tests
{
    [TestClass]
    public class ConflictModelFactoryTests
    {
        [TestMethod]
        public void Construct_GivenPassThrough()
        {
            //---------------Set up test pack-------------------
            var activity = new Mock<IDev2Activity>();
            var conflictNode = new Mock<IConflictNode>();
            var node = new Mock<IConflictTreeNode>();
            node.Setup(p => p.Activity).Returns(activity.Object);
            var contextualResourceModel = new Mock<IContextualResourceModel>();
            var factory = new ConflictModelFactory(contextualResourceModel.Object, node.Object);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            Assert.IsNotNull(factory.Children);
            Assert.IsNull(factory.Model);
        }
    }
}