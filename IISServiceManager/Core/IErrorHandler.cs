using System;

namespace IISServiceManager.Core
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}