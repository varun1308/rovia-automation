namespace Rovia.UI.Automation.Exceptions
{
    using System;

    public class PageLoadFailed:BaseException
    {
        public PageLoadFailed(string page)
            : base(page +" failed to load ")
        {

        }

        public PageLoadFailed(string page, Exception innerException)
            : base(page + " failed to load ", innerException)
        {

        }
    }
}
