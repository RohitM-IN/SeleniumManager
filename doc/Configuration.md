# ConfigurationSettings in Selenium Manager

The `ConfigurationSettings` class is a core component of the Selenium Manager library. It's designed to encapsulate various configuration options that govern how the Selenium Manager interacts with Selenium Grid, manages WebDriver instances, and handles test execution.

## Table of Contents

- [Introduction](#Introduction)
- [Properties](#Properties)
  - [GridHost](#GridHost)
  - [statistics](#statistics)
  - [UserName](#UserName)
  - [Password](#Password)
  - [MaxConcurrency](#MaxConcurrency)
  - [Endpoints](#Endpoints)
  - [Options](#Options)
- [Usage](#Usage)
- [Examples](#Examples)

## Introduction

The `ConfigurationSettings` class acts as a configuration hub for Selenium Manager, allowing you to customize how the library behaves in different scenarios. It provides a structured way to define settings related to Selenium Grid communication, browser options, concurrency, and more.

## Properties

### GridHost

- **Type**: `string`
- **Description**: The URL of the Selenium Grid hub where browser instances are managed and tests are executed. This URL can include the scheme (`http` or `https`), host, and port.

### statistics

- **Type**: `Dictionary<string, int>`
- **Description**: A dictionary that holds browser statistics, influencing the distribution of test actions across different browsers. The key is the browser name (e.g., "chrome," "firefox"), and the value is the desired ratio of instances for that browser.

### UserName

- **Type**: `string`
- **Description**: The username used for authenticating with the Selenium Grid hub. If your Selenium Grid requires authentication, provide the appropriate username here. If not required, leave it empty.

### Password
- **Type**: `string`
- **Description**: The password used for authenticating with the Selenium Grid hub. If your Selenium Grid requires authentication, provide the appropriate password here. If not required, leave it empty.

### Endpoints
- **Type**: `Endpoints`
- **Description**: A collection of endpoint URLs used by Selenium Manager to communicate with different parts of the Selenium Grid infrastructure. These endpoints include URLs for retrieving status, session details, and more.

### Options
- **Type**: Options
- **Description**: A collection of browser-specific options used to configure the behavior of WebDriver instances. This property holds sub-properties for each browser type (e.g., Chrome, Firefox) to allow you to set browser-specific options.

## Usage

You can leverage the `ConfigurationSettings` class to tailor the behavior of the Selenium Manager to your requirements. Customize the various properties to match your Selenium Grid setup, browser preferences, and desired concurrency level.

## Examples

### Basic Configuration

```csharp
var configSettings = new ConfigurationSettings
{
    GridHost = "http://localhost:4444/wd/hub",
    statistics = new Dictionary<string, int>
    {
        { "chrome", 1 },
        { "firefox", 1 },
        { "MicrosoftEdge", 1 }
    },
    UserName = "your-username",
    Password = "your-password",
    MaxConcurrency = 5,
    Endpoints = new Endpoints
    {
        Status = "/status",
        // ... other endpoint URLs
    },
    Options = new Options
    {
        ChromeOptions = new ChromeOptions(),
        FirefoxOptions = new FirefoxOptions(),
        // ... other browser-specific options
    }
};

```

### Configuration via Custom Config.json

```json
{
  "Gridhost": "http://127.0.0.1:4444",
  "UserName": "",
  "Password": "",
  "statistics": {
    "Chrome": 2,
    "MicrosoftEdge": 1,
    "Firefox": 1,
    "Internet Explorer": 0
  },
  "endpoints": {
    "status": "/status"
  }
}
```

Using above config file

```csharp
var Config = new ConfigManager("path/to/custom_config.json");

var manager = new SeleniumManager(Config);

```

---

This example demonstrates how to configure the ConfigurationSettings class to suit your Selenium Manager needs. Adjust the properties according to your testing environment and preferences.

