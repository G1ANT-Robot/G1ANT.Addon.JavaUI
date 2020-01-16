using G1ANT.Addon.JavaUI.Models;
using G1ANT.Addon.JavaUI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WindowsAccessBridgeInterop;

namespace G1ANT.Addon.JavaUI.Tests.Models
{
    [TestFixture]
    class NodeModelTests
    {
        private Mock<INodeService> nodeServiceMock;
        private Mock<AccessBridge> accessBridgeMock;
        private NodeModel mut;

        [SetUp]
        public void Setup()
        {
            nodeServiceMock = new Mock<INodeService>();
            accessBridgeMock = new Mock<AccessBridge>();
            accessBridgeMock.Setup(ab => ab.IsLegacy).Returns(false);
        }

        private AccessibleContextNode GetTestContextNode()
        {
            return new AccessibleContextNode(accessBridgeMock.Object, new JavaObjectHandle(It.IsAny<int>(), new JOBJECT32(It.IsAny<int>())));
        }

        private void SetupContextInfo(AccessibleContextInfo info)
        {
            accessBridgeMock
                .Setup(ab => ab.Functions.GetAccessibleContextInfo(It.IsAny<int>(), It.IsAny<JavaObjectHandle>(), out info))
                .Returns(true);
        }

        [TestFixture]
        public class ActionsTests : NodeModelTests
        {
            [Test]
            public void ShouldReturnActions_WhenNodeContainsActions()
            {
                var actions = new List<string>() { "action" };
                SetupContextInfo(new AccessibleContextInfo() { accessibleAction = actions.Count });

                var node = GetTestContextNode();

                nodeServiceMock.Setup(ns => ns.GetActions(node)).Returns(actions);

                mut = new NodeModel(node, nodeServiceMock.Object);
                var result = mut.Actions;

                Assert.AreEqual(actions, result);
            }

            [Test]
            public void ShouldNotGetActions_WhenNodeContainsNoActions()
            {
                SetupContextInfo(new AccessibleContextInfo() { accessibleAction = 0 });

                var node = GetTestContextNode();

                mut = new NodeModel(node, nodeServiceMock.Object);
                var result = mut.Actions;

                Assert.IsFalse(result.Any());

                nodeServiceMock.Verify(ns => ns.GetActions(It.IsAny<AccessibleContextNode>()), Times.Never);
            }
        }


        [TestFixture]
        public class DataFromAccessibleContextInfoTests : NodeModelTests
        {
            [Test]
            public void ShouldFillDataFromAccessibleContextInfo()
            {
                var contextInfo = new AccessibleContextInfo()
                {
                    name = "name",
                    role = "role",
                    description = "description",
                    states = "first state, second state", // space at the beginning should be preserved
                    childrenCount = 1,
                    height = 1024,
                    width = 768,
                    x = 1,
                    y = 2,
                    indexInParent = 3,
                    accessibleAction = 0
                };
                SetupContextInfo(contextInfo);

                mut = new NodeModel(GetTestContextNode(), nodeServiceMock.Object);

                Assert.AreEqual(contextInfo.name, mut.Name);
                Assert.AreEqual(contextInfo.role, mut.Role);
                Assert.AreEqual(contextInfo.description, mut.Description);
                Assert.AreEqual(contextInfo.states.Split(','), mut.States);
                Assert.AreEqual(contextInfo.childrenCount, mut.ChildrenCount);
                Assert.AreEqual(contextInfo.height, mut.Height);
                Assert.AreEqual(contextInfo.width, mut.Width);
                Assert.AreEqual(contextInfo.x, mut.X);
                Assert.AreEqual(contextInfo.y, mut.Y);
                Assert.AreEqual(new Rectangle(contextInfo.x, contextInfo.y, contextInfo.width, contextInfo.height), mut.Bounds);
                Assert.AreEqual(contextInfo.indexInParent, mut.IndexInParent);
                Assert.AreEqual(contextInfo.accessibleAction, mut.Actions.Count);
            }
        }

        public class IdTests : NodeModelTests
        {
            [Test]
            public void ShouldFillId_WhenNodeIsAccessibleJvm()
            {
                var id = 1;
                mut = new NodeModel(new AccessibleJvm(accessBridgeMock.Object, id), nodeServiceMock.Object);
                Assert.AreEqual(id, mut.Id);
            }

            [Test]
            public void ShouldFillId_WhenNodeIsAccessibleWindow()
            {
                var id = 1;
                SetupContextInfo(new AccessibleContextInfo());

                mut = new NodeModel(
                    new AccessibleWindow(accessBridgeMock.Object, new IntPtr(id), new JavaObjectHandle(It.IsAny<int>(), It.IsAny<JOBJECT32>())),
                    nodeServiceMock.Object
                );

                Assert.AreEqual(id, mut.Id);
            }

            [Test]
            public void ShouldNotFillId_WhenNodeIsAccessibleContextNode()
            {
                SetupContextInfo(new AccessibleContextInfo());
                mut = new NodeModel(GetTestContextNode(), nodeServiceMock.Object);
                Assert.AreEqual(0, mut.Id);
            }
        }

        [TestFixture]
        public class DoActionTests : NodeModelTests
        {
            [Test]
            public void ShouldExecuteAction_WhenNodeContainsAction()
            {
                var actions = new List<string>() { "action" };
                var node = GetTestContextNode();

                SetupContextInfo(new AccessibleContextInfo() { accessibleAction = actions.Count });
                nodeServiceMock.Setup(ns => ns.GetActions(node)).Returns(actions);

                mut = new NodeModel(node, nodeServiceMock.Object);
                mut.DoAction(actions[0]);

                nodeServiceMock.Verify(ns => ns.DoAction(node, actions[0]), Times.Once);
            }


            [Test]
            public void ShouldThrow_WhenNodeDoesNotContainAction()
            {
                var actions = new List<string>() { "action" };
                var node = GetTestContextNode();

                SetupContextInfo(new AccessibleContextInfo() { accessibleAction = actions.Count });
                nodeServiceMock.Setup(ns => ns.GetActions(node)).Returns(actions);

                mut = new NodeModel(node, nodeServiceMock.Object);

                var ex = Assert.Throws<ArgumentOutOfRangeException>(() => mut.DoAction("some not existing action"));
                StringAssert.Contains(actions[0], ex.Message);

                nodeServiceMock.Verify(ns => ns.DoAction(node, It.IsAny<string>()), Times.Never);
            }
        }


        [TestFixture]
        public class ToXPathTests : NodeModelTests
        {
            [Test]
            public void ShouldReturnEmptyString_WhenNodeIsAccessibleJvm()
            {
                var node = new AccessibleJvm(accessBridgeMock.Object, It.IsAny<int>());
                mut = new NodeModel(node, nodeServiceMock.Object);
                Assert.AreEqual("", mut.ToXPath());
            }

            [Test]
            public void ShouldReturnDescendant_WhenNodeIsAccessibleWindow()
            {
                var node = new AccessibleWindow(accessBridgeMock.Object, It.IsAny<IntPtr>(), new JavaObjectHandle(It.IsAny<int>(), It.IsAny<JOBJECT32>()));
                SetupContextInfo(new AccessibleContextInfo());

                mut = new NodeModel(node, nodeServiceMock.Object);
                StringAssert.StartsWith("/descendant::", mut.ToXPath());
            }

            [Test]
            public void ShouldReturnPathByName_WhenNodeIsAccessibleContextNodeWithName()
            {
                var name = "name";
                var node = GetTestContextNode();
                SetupContextInfo(new AccessibleContextInfo() { name = name });

                mut = new NodeModel(node, nodeServiceMock.Object);
                Assert.AreEqual($"/ui[@name='{name}']", mut.ToXPath());
            }

            [Test]
            public void ShouldReturnPathByRole_WhenNodeIsAccessibleContextNodeWithRole()
            {
                var role = "role";
                var node = GetTestContextNode();
                SetupContextInfo(new AccessibleContextInfo() { name = "", role = role });

                mut = new NodeModel(node, nodeServiceMock.Object);
                Assert.AreEqual($"/ui[@role='{role}']", mut.ToXPath());
            }

            [Test]
            public void ShouldReturnPathById_WhenNodeHasId()
            {
                var id = 1;
                var node = new AccessibleWindow(accessBridgeMock.Object, new IntPtr(id), new JavaObjectHandle(It.IsAny<int>(), It.IsAny<JOBJECT32>()));
                SetupContextInfo(new AccessibleContextInfo() { name = "", role = "" });

                mut = new NodeModel(node, nodeServiceMock.Object);
                Assert.AreEqual($"/descendant::ui[@id='{id}']", mut.ToXPath());
            }

            [Test]
            public void ShouldReturnPathByName_WhenNodeIsAccessibleContextNodeWithRole()
            {
                var indexInParent = 1;
                var node = GetTestContextNode();
                SetupContextInfo(new AccessibleContextInfo() { name = "", role = "", indexInParent = indexInParent });

                mut = new NodeModel(node, nodeServiceMock.Object);
                Assert.AreEqual($"/ui[position()={indexInParent}]", mut.ToXPath());
            }
        }

    }
}
