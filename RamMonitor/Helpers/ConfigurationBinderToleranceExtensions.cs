
using Microsoft.Extensions.Configuration;


namespace RamMonitor
{
    
    
    public static class ConfigurationBinderToleranceExtensions
    {
        
        
        /// <summary>
        /// Extracts the value with the specified key and converts it to type T.
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="key">The key of the configuration section's value to convert.</param>
        /// <returns>The converted value.</returns>
        public static T TryGetValue<T>(this Microsoft.Extensions.Configuration.IConfiguration configuration, string key, T defaultValue)
        {
            T value = defaultValue;
            
            try
            {
                value = configuration.GetValue(key, default(T));
            }
            catch (System.Exception){ }
            
            return value;
        }
        
    } // End Class ConfigurationBinderToleranceExtensions 
    
    
} 
