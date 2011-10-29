#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.StructureMap.Tests
{
    using System;
    using System.Web.Mvc;

    using Moq;
    using Xunit;
    using Xunit.Extensions;

    using ConfigurationExpression = global::StructureMap.ConfigurationExpression;
    using IContainer = global::StructureMap.IContainer;

    public class StructureMapAdapterTests
    {
        private readonly Mock<IContainer> container;
        private readonly StructureMapAdapter adapter;

        public StructureMapAdapterTests()
        {
            container = new Mock<IContainer>();
            adapter = new StructureMapAdapter(container.Object);
        }

        [Theory]
        [InlineData(LifetimeType.PerRequest)]
        [InlineData(LifetimeType.Singleton)]
        [InlineData(LifetimeType.Transient)]
        public void Should_be_able_to_register_type(LifetimeType lifetime)
        {
            var registry = (ConfigurationExpression)Activator.CreateInstance(typeof(ConfigurationExpression), true);

            container.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>())).Callback((Action<ConfigurationExpression> x) => x(registry)).Verifiable();

            adapter.RegisterType(typeof(DummyObject), typeof(DummyObject), lifetime);

            container.Verify();
        }

        [Fact]
        public void Should_be_able_to_register_instance()
        {
            var registry = (ConfigurationExpression)Activator.CreateInstance(typeof(ConfigurationExpression), true);

            container.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>())).Callback((Action<ConfigurationExpression> x) => x(registry)).Verifiable();

            adapter.RegisterInstance(typeof(DummyObject), new DummyObject());

            container.Verify();
        }

        [Fact]
        public void Should_be_able_to_inject()
        {
            var dummy = new DummyObject();

            container.Setup(c => c.BuildUp(It.IsAny<object>()));

            adapter.Inject(dummy);

            container.VerifyAll();
        }

        [Fact]
        public void Should_be_able_to_get_service_by_type()
        {
            container.Setup(c => c.GetInstance(It.IsAny<Type>()));

            adapter.GetService<DummyObject>();

            container.VerifyAll();
        }

        [Fact]
        public void Should_be_able_to_get_services()
        {
            container.Setup(c => c.GetAllInstances(It.IsAny<Type>())).Returns(new[] { new DummyObject() });

            adapter.GetServices(typeof(DummyObject));

            container.VerifyAll();
        }

        private class DummyObject
        {
        }
    }
}