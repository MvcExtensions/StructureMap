#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.StructureMap.Tests
{
    using System;

    using Microsoft.Practices.ServiceLocation;

    using Moq;
    using Xunit;

    using ConfigurationExpression = global::StructureMap.ConfigurationExpression;
    using IContainer = global::StructureMap.IContainer;
    using Registry = global::StructureMap.Configuration.DSL.Registry;

    public class StructureMapBootstrapperTests
    {
        [Fact]
        public void Should_be_able_to_create_adapter()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.Assemblies).Returns(new[] { GetType().Assembly });

            var bootstrapper = new StructureMapBootstrapper(buildManager.Object);

            Assert.IsType<StructureMapAdapter>(bootstrapper.Adapter);
        }

        [Fact]
        public void Should_be_able_to_load_modules()
        {
            var buildManager = new Mock<IBuildManager>();
            buildManager.SetupGet(bm => bm.ConcreteTypes).Returns(new[] { typeof(DummyRegistry) });

            var registry = (ConfigurationExpression)Activator.CreateInstance(typeof(ConfigurationExpression), true);

            var container = new Mock<IContainer>();
            container.Setup(c => c.Configure(It.IsAny<Action<ConfigurationExpression>>())).Callback((Action<ConfigurationExpression> x) => x(registry)).Verifiable();

            var bootstrapper = new StructureMapBootstrapperTestDouble(container.Object, buildManager.Object);

            Assert.IsType<StructureMapAdapter>(bootstrapper.Adapter);

            container.Verify();
        }

        private sealed class DummyRegistry : Registry
        {
        }

        private sealed class StructureMapBootstrapperTestDouble : StructureMapBootstrapper
        {
            private readonly IContainer container;

            public StructureMapBootstrapperTestDouble(IContainer container, IBuildManager buildManager) : base(buildManager)
            {
                this.container = container;
            }

            protected override ContainerAdapter CreateAdapter()
            {
                return new StructureMapAdapter(container);
            }
        }
    }
}