﻿using System;

using Microsoft.Extensions.DependencyInjection;

namespace MoqInjectingServiceProvider
{
	public static class ServiceCollectionExtensions
	{
		public static MockInjectingServiceProvider BuildMockInjectingServiceProvider (this IServiceCollection services)
		{
			return new MockInjectingServiceProvider(services.BuildServiceProvider());
		}
	}
}
