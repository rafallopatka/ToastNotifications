```
 _______              _   _   _       _   _  __ _           _   _                         ___  
|__   __|            | | | \ | |     | | (_)/ _(_)         | | (_)                       |__ \
   | | ___   __ _ ___| |_|  \| | ___ | |_ _| |_ _  ___ __ _| |_ _  ___  _ __  ___   __   __ ) |
   | |/ _ \ / _` / __| __| . ` |/ _ \| __| |  _| |/ __/ _` | __| |/ _ \| '_ \/ __|  \ \ / // /
   | | (_) | (_| \__ \ |_| |\  | (_) | |_| | | | | (_| (_| | |_| | (_) | | | \__ \   \ V // /_
   |_|\___/ \__,_|___/\__|_| \_|\___/ \__|_|_| |_|\___\__,_|\__|_|\___/|_| |_|___/    \_/|____|

```

# ToastNotifications v2
#### Toast notifications for WPF

ToastNotifications allows you to create and display rich notifications in WPF applications.
It's highly configurable with set of built-in options like positions, behaviours, themes and many others.
It's extendable, it gives you possibility to create custom and interactive notifications in simply manner.

[![Build status](https://ci.appveyor.com/api/projects/status/xk2e7g0nxfh5v92q?svg=true)](https://ci.appveyor.com/project/raflop/toastnotifications)
[![Code Climate](https://codeclimate.com/github/raflop/ToastNotifications/badges/gpa.svg)](https://codeclimate.com/github/raflop/ToastNotifications)
[![Issue Count](https://codeclimate.com/github/raflop/ToastNotifications/badges/issue_count.svg)](https://codeclimate.com/github/raflop/ToastNotifications)
[![Nuget install](https://img.shields.io/badge/nuget-install-green.svg)](https://www.nuget.org/packages/ToastNotifications/)
[![Nuget install](https://img.shields.io/badge/nuget-install-green.svg)](https://www.nuget.org/packages/ToastNotifications.Messages/)
[![LGPL v3 license](https://img.shields.io/badge/license-LGPLV3-blue.svg)](https://github.com/raflop/ToastNotifications/blob/master-v2/license)

## Demo

[![demo](https://raw.githubusercontent.com/raflop/ToastNotifications/master-v2/Media/demo.gif)](https://raw.githubusercontent.com/raflop/ToastNotifications/master-v2/Media/demo.gif)

## Usage

[Example code](https://github.com/raflop/ToastNotifications/tree/master-v2/Src/Examples/BasicUsageExample)

### 1 Install via nuget:
[ToastNotifications](https://www.nuget.org/packages/ToastNotifications/) and [ToastNotifications.Messages](https://www.nuget.org/packages/ToastNotifications.Messages/)

```
Install-Package ToastNotifications
Install-Package ToastNotifications.Messages
```

ToastNotifications v2 is plugin oriented.

*Nugget "ToastNotifications"* is a core, which contains only main mechanisms for creating and displaying notifications.
Predefined messages and other not key functionalities are provided by separate nuggets.

*Nugget ToastNotifications.Messages* contains basic messages like error, information, warning, success.
It's not required in case you want to create your own messages.

### 2 Import ToastNotifications.Messages theme in App.xaml
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/ToastNotifications.Messages;component/Themes/Default.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 3 Create Notifier instance
```csharp
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
/* * */
Notifier notifier = new Notifier(cfg =>
{
    cfg.PositionProvider = new WindowPositionProvider(
        parentWindow: Application.Current.MainWindow,
        corner: Corner.TopRight,
        offsetX: 10,  
        offsetY: 10);

    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
        notificationLifetime: TimeSpan.FromSeconds(3),
        maximumNotificationCount: MaximumNotificationCount.FromCount(5));

    cfg.Dispatcher = Application.Current.Dispatcher;
});
```

### 4 Use provided messages
```csharp
using ToastNotifications.Messages;
/* * */
notifier.ShowInformation(message);
notifier.ShowSuccess(message);
notifier.ShowWarning(message);
notifier.ShowError(message);
```

### 5 Dispose notifier when it's no longer needed
```csharp
/* * */
notifier.Dispose();
```

## Documentation

* [Release notes](https://github.com/raflop/ToastNotifications/blob/master-v2/Docs/ReleaseNotes.md)
  Release notes, description of new features, bug fixes and breaking changes.
  
* [Upgrading from v1](https://github.com/raflop/ToastNotifications/blob/master-v2/Docs/Migration.md)
  ToastNotifications v2 is completely new implementation and it's not compatibile with version 1, follow migration instructions to upgrade to the new version.

* [Configuration](https://github.com/raflop/ToastNotifications/blob/master-v2/Docs/Configuration.md)
  ToastNotifications v2 has lots of configuration options for its position, lifetime, messages and many others, this document describe them all.

* [Creating custom notifications](https://github.com/raflop/ToastNotifications/blob/master-v2/Docs/CustomNotificatios.md)
  This document describes how to create your own notifications.

* [Strongly named assemblies](https://github.com/raflop/ToastNotifications/blob/master-v2/Docs/StronglyNamedAssemblies.md)
  ToastNotifications v2 assembies are signed. Read this doc for more details.

## Contributors

Andy Li (https://github.com/oneandy)

B. Micka (https://github.com/b-mi)

CuiLiang (https://github.com/cuiliang)

BrainCrumbz (https://github.com/BrainCrumbz)

Fischjoghurt (https://github.com/Fischjoghurt)

Francois Botha igitur (https://github.com/igitur)

Jan M. (https://github.com/Pantheas)

Johannes Jidel (https://github.com/jidel)

Kostiantyn (https://github.com/dualbios)

Krzysztof Zmorzyński (https://github.com/ZmorzynskiK)

Robin Krom Lakritzator (https://github.com/Lakritzator)

Uwy (https://github.com/Uwy)

William David Cossey (https://github.com/wdcossey)



## Creating new issues
Before you create new issue, please check the documentation, because many features and options are already there.
(https://github.com/raflop/ToastNotifications/tree/master-v2/Docs)

If there is still a problem, please create new issue/question filling following informations. 
If it's possible, please provide a sample code to reproduce issue.
