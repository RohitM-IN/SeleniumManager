# Selenium Manager API Reference

## Table of Contents

- [Selenium Manager](#selenium-manager)
  - [Properties](#properties)
  - [Methods](#methods)
- [ConfigManager](#configmanager)
  - [Properties](#properties-1)
  - [Methods](#methods-1)
- [ISeleniumManager](#iseleniummanager)
  - [Properties](#properties-2)
  - [Methods](#methods-2)
- [WebDriverType](#webdrivertype)
- [AdjustType](#adjusttype)
- [RatioDictionary](#ratiodictionary)
- [WebDriverTypeExtensions](#webdrivertypeextensions)

## Selenium Manager

### Properties

- `int MaxSessions`: Gets the maximum number of sessions available.
- `int FreeSessions`: Gets the number of free sessions available.
- `int ConcurrentSessions`: Gets the number of concurrent sessions in use.
- `int AvailableSessions`: Gets the number of available sessions.
- `int TotalSessions`: Gets the total count of sessions.
- `Dictionary<string, long> MaxStereotypes`: Dictionary of browser names and their maximum instances.
- `Dictionary<string, long> ConcurrentStereotypes`: Dictionary of browser names and their concurrent instances.
- `Dictionary<string, long> AvailableStereotypes`: Dictionary of browser names and their available instances.
- `DateTime LastSessionDetails`: Gets the timestamp of the last session details update.

### Methods

- `Task<string> EnqueueAction(Func<IWebDriver, string> action)`: Enqueues an action for execution on an available browser instance.
- `Task<string> EnqueueAction(Func<IWebDriver, string> action, string browserName)`: Enqueues an action for execution on a specific browser instance.
- `void TryExecuteNext()`: Tries to execute the next action in the queue.
- `Task<int> GetAvailableInstances()`: Gets the number of available browser instances.
- `Task<dynamic?> GetHeartBeat()`: Gets the status data from the Selenium grid.
- `IWebDriver CreateDriverInstance(string? browserName = null)`: Creates an instance of the Selenium WebDriver for a specified browser.
- `string GetAvailableDriverName(string? browserName)`: Gets an available browser name for execution.

## ConfigManager

### Properties

- `ConfigurationSettings configSettings`: Gets the configuration settings for Selenium Manager.

### Methods

- `ConfigManager(string? configFilePath = null)`: Initializes the ConfigManager with the specified configuration file path.
- `ConfigurationSettings LoadConfigSettingsFromResource(string resourceName)`: Loads configuration settings from a resource.
- `ConfigurationSettings LoadConfigSettingsFromFile(string configFilePath)`: Loads configuration settings from a file.

## ISeleniumManager

### Properties

- `int MaxSessions`: Gets the maximum number of sessions available.
- ... (similar to the properties listed under Selenium Manager)

### Methods

- ... (similar to the methods listed under Selenium Manager)

## WebDriverType

An enumeration representing different types of web drivers.

## AdjustType

An enumeration representing actions to adjust browser instance counts (Create or Destroy).

## RatioDictionary

A utility class for calculating a dictionary of browser instance ratios.

## WebDriverTypeExtensions

Extensions for the WebDriverType enumeration, including custom descriptions.

