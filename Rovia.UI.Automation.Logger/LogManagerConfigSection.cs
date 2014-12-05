
using System.Configuration;

namespace Rovia.UI.Automation.Logger
{
    class LogManagerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("SummaryLoggers", IsRequired = false, IsDefaultCollection = true)]
        public LoggerCollection SummaryLoggers
        {
            get { return (LoggerCollection)this["SummaryLoggers"]; }
            set { this["SummaryLoggers"] = value; }
        }

        [ConfigurationProperty("DetailLoggers", IsRequired = false, IsDefaultCollection = true)]
        public LoggerCollection DetailLoggers
        {
            get { return (LoggerCollection)this["DetailLoggers"]; }
            set { this["DetailLoggers"] = value; }
        }

        [ConfigurationProperty("Severity", IsRequired = true)]
        public SeverityValue Severity
        {
            get { return (SeverityValue)this["Severity"]; }
            set { this["Severity"] = value; }
        }
    }


    internal class LoggerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //set to whatever Element Property you want to use for a key
            return ((LoggerElement)element).Alias;
        }
    }

    internal class LoggerElement : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("assembly", IsKey = true, IsRequired = true)]
        public string Assembly
        {
            get { return (string)base["assembly"]; }
            set { base["assembly"] = value; }
        }

        [ConfigurationProperty("namespace", IsRequired = true)]
        public string Namespace
        {
            get { return (string)base["namespace"]; }
            set { base["namespace"] = value; }
        }
        [ConfigurationProperty("alias", IsRequired = true)]
        public string Alias
        {
            get { return (string)base["alias"]; }
            set { base["alias"] = value; }
        }

        [ConfigurationProperty("target", IsRequired = true)]
        public string Target
        {
            get { return (string)base["target"]; }
            set { base["target"] = value; }
        }
    }

    internal class SeverityValue : ConfigurationElement
    {
        //Make sure to set IsKey=true for property exposed as the GetElementKey above
        [ConfigurationProperty("value", IsKey = true, IsRequired = true)]
        public Severity Value
        {
            get { return (Severity)base["value"]; }
            set { base["value"] = value; }
        }
    }
}
