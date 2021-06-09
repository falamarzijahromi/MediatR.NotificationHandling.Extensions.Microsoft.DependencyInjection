# MediatR.NotificationHandling.Extensions.Microsoft.DependencyInjection
Separate dependency scope notification handling extension for MediateR using with Microsoft.Extensions.DependencyInjection.
By default MediatR handles every notification in just on IServiceScope, The one IMediatR resolved with.

By using this extension every notification will be handled in separate IServiceScope thus there will be no resource or thread conflicts and every dependencies injected in the hierarchy of INotificationHandler'1 will be disposed after Handle method called; whether successful or not.

Extensions tested with modified MediatR and MediatR.Extensions.Microsoft.DependencyInjection tests to ensure compatibility and performability.

You can use extension in two ways. in every cases there are three pre conditions:
1- IMediatR must be registered with scoped lifestyle.

```c#
services.AddMediatR(cfg => cfg.AsScoped(), typeof(Startup));
```

2- Microsoft runtime implemetation of IServiceProvider must be IServiceScope too. (This condition depends on microsoft .net team)

3- Extension methods must be called after all notification handlers registered.

Configuring MediatR immediately after AddMediatR registered handlers:

```c#
services
	.AddMediatR(cfg => cfg.AsScoped(), typeof(Startup))
	.IsolateNotificationHandlingScopes();
```

Configuring MediatR after AddMediatR and some relevant registration like other handlers:

```c#
services.AddMediatR(cfg => cfg.AsScoped(), typeof(Startup));

...

services.IsolateMediatRNotificationHandlingScopes();
```
