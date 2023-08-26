﻿using Newtonsoft.Json;
using SeleniumManager.Core.DataContract;
using SeleniumManager.Core.Exception;
using System.Reflection;

namespace SeleniumManager.Core
{
    public class ConfigManager
    {
        #region Declaration

        public readonly ConfigurationSettings configSettings;

        #endregion

        #region Public Functions

        /// <summary>
        /// This function sets the config required for the Selenium Manager
        /// </summary>
        /// <param name="configFilePath"></param>
        public ConfigManager(string? configFilePath = null)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                // Use default configuration file included in the DLL
                configSettings = LoadConfigSettingsFromResource("Configuration.config.json");
            }
            else
            {
                // Use configuration file from the specified path
                configSettings = LoadConfigSettingsFromFile(configFilePath);
            }
        }

        #endregion

        #region Private Functions

        private ConfigurationSettings LoadConfigSettingsFromResource(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourcePath = $"{assembly.GetName().Name}.{resourceName}";

                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))

                using (StreamReader reader = new StreamReader(stream))
                {
                    string configJson = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<ConfigurationSettings>(configJson);
                }

            }
            catch (System.Exception ex)
            {

                throw new ConfigurationException("There was an error while loading settings from resource", ex);
            }
        }

        private ConfigurationSettings LoadConfigSettingsFromFile(string configFilePath)
        {
            try
            {
                string configJson = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<ConfigurationSettings>(configJson);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ConfigurationException($"An error occurred while loading the configuration file: {ex.Message}", ex);
            }
        }

        #endregion

    }
}
