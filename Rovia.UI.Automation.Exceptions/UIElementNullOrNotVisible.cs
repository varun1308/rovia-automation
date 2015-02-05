namespace Rovia.UI.Automation.Exceptions
{
    using System;

    public class UIElementNullOrNotVisible:BaseException
    {
        public UIElementNullOrNotVisible(string param)
            : base(param+" is Null or Not Visible")
        {

        }
        public UIElementNullOrNotVisible(string param, Exception innerException)
            : base(param + " is Null or Not Visible", innerException)
        {

        }
    }
}
