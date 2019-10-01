# MoqInjector
Helper to automatically inject Mocks of all dependencies for easier testing.

Usage
```c#
// Start with a normal service collection.
ServiceCollection serviceCollection = new ServiceCollection();

// Add whatever you don't want mocked, like your DbContext, etc.
serviceCollection.AddSingleton<MyDbContext>(dbContext);

// Turn it into a provider:
MockInjectingServiceProvider provider = serviceCollection.BuildMockInjectingServiceProvider();

// Get your Object Under Test:
ObjectUnderTest target = provider.GetRequiredService<ObjectUnderTest>();

// If you need access to the mocks (for setups or insertions, etc), get them from the provider.
// This should be done after the object under test is retreived.
Mock<ISomeInterface> mock = target.GetMock<ISomeInterface>()
```
