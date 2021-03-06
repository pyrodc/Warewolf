/*
*  Warewolf - Once bitten, there's no going back
*  Copyright 2018 by Warewolf Ltd <alpha@warewolf.io>
*  Licensed under GNU Affero General Public License 3.0 or later. 
*  Some rights reserved.
*  Visit our website for more information <http://warewolf.io/>
*  AUTHORS <http://warewolf.io/authors.php> , CONTRIBUTORS <http://warewolf.io/contributors.php>
*  @license GNU Affero General Public License <http://www.gnu.org/licenses/agpl-3.0.html>
*/

using System;
using System.Windows;
using Dev2.Common.Interfaces.Infrastructure.Events;
using Dev2.Studio.Core.AppResources.Converters;
using Dev2.Studio.Core.Models;
using Dev2.Studio.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace Dev2.Core.Tests.ConverterTests
{
    [TestClass]
    public class DeployViewConnectedToVisiblityConverterTest
    {
        [TestMethod]
        [Owner("Jurie Smit")]
        [TestCategory("DeployViewConnectedToVisiblityConverter")]
        public void DeployViewConnectedToVisiblityConverter_Convert_IsConnectedIsFalse_VisibilityIsCollapsed()
        {
            //Arrange
            var converter = new DeployViewConnectedToVisiblityConverter();
            var mockEnvironmentConnection = new Mock<IEnvironmentConnection>();
            mockEnvironmentConnection.Setup(m => m.ServerEvents).Returns(new Mock<IEventPublisher>().Object);
            mockEnvironmentConnection.Setup(m => m.IsConnected).Returns(false);
            IServer server = new Server(Guid.NewGuid(), mockEnvironmentConnection.Object);

            //Act
            var actual = (Visibility)converter.Convert(server, typeof(bool), null, null);
            //Assert
            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        [Owner("Jurie Smit")]
        [TestCategory("DeployViewConnectedToVisiblityConverter")]
        public void DeployViewConnectedToVisiblityConverter_Convert_IsConnectedIsTrue_VisibilityIsCollapsed()
        {
            //Arrange
            var converter = new DeployViewConnectedToVisiblityConverter();
            var mockEnvironmentConnection = new Mock<IEnvironmentConnection>();
            mockEnvironmentConnection.Setup(m => m.ServerEvents).Returns(new Mock<IEventPublisher>().Object);
            mockEnvironmentConnection.Setup(m => m.IsConnected).Returns(true);
            IServer server = new Server(Guid.NewGuid(), mockEnvironmentConnection.Object);

            //Act
            var actual = (Visibility)converter.Convert(server, typeof(bool), null, null);
            //Assert
            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        [Owner("Jurie Smit")]
        [TestCategory("DeployViewConnectedToVisiblityConverter")]
        public void DeployViewConnectedToVisiblityConverter_Convert_EnvironmentModelIsNull_VisibilityIsCollapsed()
        {
            //Arrange
            var converter = new DeployViewConnectedToVisiblityConverter();
            //Act
            var actual = (Visibility)converter.Convert(null, typeof(bool), null, null);
            //Assert
            Assert.AreEqual(Visibility.Collapsed, actual);
        }
    }
}
