using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Moq;

namespace MoqInjectingServiceProvider
{
	public class MockInjectingServiceProvider : IServiceProvider
	{
		private readonly IServiceProvider serviceProvider;

		private readonly ConcurrentDictionary<Type, object> mockedServices = new ConcurrentDictionary<Type, object>();
		private readonly ConcurrentDictionary<Type, object> mocks = new ConcurrentDictionary<Type, object>();

		public Mock<T> GetMock<T> () where T : class
		{
			return (Mock<T>)mocks[typeof(T)];
		}

		public MockInjectingServiceProvider (IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public object GetService (Type serviceType)
		{
			ConstructorInfo constructor = serviceType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();

			// we need this so we can keep them in the right order.
			List<object> injectedObjects = new List<object>(constructor.GetParameters().Length);

			foreach (ParameterInfo parameter in constructor.GetParameters())
			{
				object dependency;
				mockedServices.TryGetValue(parameter.ParameterType, out dependency);

				if (dependency == null)
				{
					dependency = TryGetConfiguredService(parameter.ParameterType);
				}

				if (dependency == null)
				{
					dependency = CreateMock(parameter.ParameterType);
				}

				injectedObjects.Add(dependency);

			}

			return constructor.Invoke(injectedObjects.ToArray());
		}

		private object TryGetConfiguredService (Type dependencyType)
		{
			try
			{
				return serviceProvider.GetService(dependencyType);
			}
			catch (InvalidOperationException ex) when (ex.Message.Contains(dependencyType.Name))
			{
				return null;
			}
		}

		private object CreateMock (Type dependencyType)
		{
			// reflect a moq of it.
			Type mockType = typeof(Mock<>).MakeGenericType(dependencyType);
			object mock = Activator.CreateInstance(mockType);

			// add it to the mock dictionary too so we can find it later.
			mocks.TryAdd(dependencyType, mock);

			// Save it's "Object" property so we can stuff it in the target.'
			PropertyInfo objProperty = mockType.GetProperties().Single(p => p.Name == "Object" && p.DeclaringType == mockType);

			object mockService = objProperty.GetGetMethod().Invoke(mock, null);

			// save it so we don't have to mock it again.
			mockedServices.TryAdd(dependencyType, mockService);

			return mockService;
		}
	}
}
