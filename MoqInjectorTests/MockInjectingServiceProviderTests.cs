using System;

using Microsoft.Extensions.DependencyInjection;

using MoqInjector;

using NUnit.Framework;

namespace MoqInjectorTests
{
	[TestFixture]
	public class MockInjectingServiceProviderTests
	{
		[Test]
		public void NoDependenciesTest ()
		{
			//Arrange
			ServiceCollection serviceCollection = new ServiceCollection();

			IServiceProvider target = serviceCollection.BuildMockInjectingServiceProvider();

			//Act
			NoDependencies service = target.GetRequiredService<NoDependencies>();

			//Assert
			// no exceptions
		}

		[Test]
		public void OneDependencyTest ()
		{
			//Arrange
			ServiceCollection serviceCollection = new ServiceCollection();

			MockInjectingServiceProvider target = serviceCollection.BuildMockInjectingServiceProvider();

			//Act
			OneDependency service = target.GetRequiredService<OneDependency>();

			//Assert
			Assert.AreEqual(target.GetMock<ISomeInterface>().Object, service.SomeInterface);
		}

		[Test]
		public void TwoDependencyTest ()
		{
			//Arrange
			ServiceCollection serviceCollection = new ServiceCollection();

			MockInjectingServiceProvider target = serviceCollection.BuildMockInjectingServiceProvider();

			//Act
			TwoDependency service = target.GetRequiredService<TwoDependency>();

			//Assert
			Assert.AreEqual(target.GetMock<ISomeInterface>().Object, service.SomeInterface);
			Assert.AreEqual(target.GetMock<ISomeOtherInterface>().Object, service.SomeOtherInterface);
		}

		[Test]
		public void OneConfiguredDependency ()
		{
			//Arrange
			ServiceCollection serviceCollection = new ServiceCollection();
			SomeClass someClass = new SomeClass();
			serviceCollection.AddSingleton<ISomeInterface>(someClass);

			MockInjectingServiceProvider target = serviceCollection.BuildMockInjectingServiceProvider();

			//Act
			TwoDependency service = target.GetRequiredService<TwoDependency>();

			//Assert
			Assert.AreEqual(someClass, service.SomeInterface);
			Assert.AreEqual(target.GetMock<ISomeOtherInterface>().Object, service.SomeOtherInterface);
		}

		public class NoDependencies
		{
		}

		public class OneDependency
		{
			public ISomeInterface SomeInterface { get; }

			public OneDependency(ISomeInterface someInterface)
			{
				SomeInterface = someInterface;
			}
		}

		public class TwoDependency
		{
			public ISomeInterface SomeInterface { get; }
			public ISomeOtherInterface SomeOtherInterface { get; }

			public TwoDependency(ISomeInterface someInterface, ISomeOtherInterface someOtherInterface)
			{
				SomeInterface = someInterface;
				SomeOtherInterface = someOtherInterface;
			}
		}

		public interface ISomeInterface
		{
		}

		public class SomeClass : ISomeInterface
		{
		}

		public interface ISomeOtherInterface
		{
		}

	}
}
