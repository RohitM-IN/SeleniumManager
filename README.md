# Selenium Manager

Selenium Manager is a .NET library designed to simplify parallel testing and dynamic browser instance management using Selenium WebDriver.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Initializing Selenium Manager](#initializing-selenium-manager)
  - [Enqueuing Actions](#enqueuing-actions)
  - [Parallel Testing](#parallel-testing)
- [API Reference](doc/API_REFERENCE.md)
- [Contributing](doc/CONTRIBUTING.md)
- [License](#license)

## Features

- Simplifies parallel testing with Selenium WebDriver
- Dynamic browser instance management
- Automatic browser selection based on statistics
- Customizable browser options
- Easily integrate with your existing Selenium projects

## Installation

To use Selenium Manager, you need to install the NuGet package `SeleniumManager.Core`.

## Usage

### Initializing Selenium Manager

To get started, you'll need to initialize the Selenium Manager by creating an instance of the `SeleniumManager` class. You can provide a custom configuration file path or use the default configuration included in the library.

```csharp
using SeleniumManager.Core;

// Initialize Selenium Manager with default configuration
var configManager = new ConfigManager();
var seleniumManager = new SeleniumManager(configManager);
```

### Enqueuing Actions

Enqueuing an action allows you to add a function to the execution queue, which will be processed in parallel on available browser instances.

```csharp
// Enqueue an action without specifying a browser
var result = await seleniumManager.EnqueueAction(BrowseWebsite);

// Enqueue an action and specify the browser
var chromeResult = await seleniumManager.EnqueueAction(BrowseWebsite, WebDriverType.Chrome.GetDescription());
```

### Parallel Testing

Selenium Manager makes it easy to perform parallel testing by enqueuing multiple actions simultaneously.

```csharp
using System.Threading.Tasks;
using System.Collections.Generic;

// Perform parallel testing with multiple tasks
List<Task> tasks = new List<Task>();

for (int i = 0; i < 10; i++)
{
    Task task = Task.Run(async () =>
    {
        await seleniumManager.EnqueueAction(BrowseWebsite);
    });

    tasks.Add(task);
}

await Task.WhenAll(tasks);

```

## API Reference

For detailed information about the available classes, methods, and options, please refer to the [API Reference](/doc/API_REFERENCE.md).

## Contributing
Contributions to this project are welcome! For more information on how to contribute, please read the [Contributing Guidelines](/doc/CONTRIBUTING.md).

## License

This project is licensed under the [MIT License](/LICENSE.txt).