namespace Tauron.MgiProjectManager.Dispatcher
{
    public static class OperationMeta
    {
        public static class Linker
        {
            public const string FilePath = "FilePath";
            public const string FileName = "FileName";
            public const string UserName = "UserName";
            public const string StartId = "StartId";
            public const string RequestedName = "RequestedName";
        }

        public static class MultiFile
        {
            public const string UserName = "UserName";
        }

        public const string OperationType = nameof(OperationType);
        public const string OperationName = nameof(OperationName);
    }
}