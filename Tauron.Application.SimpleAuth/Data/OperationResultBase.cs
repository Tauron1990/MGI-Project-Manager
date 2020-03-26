using System;

namespace Tauron.Application.SimpleAuth.Data
{
    public abstract class OperationResultBase<TImpl>
        where TImpl : OperationResultBase<TImpl>, new()

    {
        /// <summary>
        /// Gibt an ob das Password erfolgreich gesetz wurde
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Nachricht was Schiefgelaufen ist,
        /// </summary>
        public string FailMessage { get; set; } = string.Empty;


        public static TImpl Success(Action<TImpl> data)
        {
            var result = new TImpl { Successful = true };
            data(result);
            return result;
        }

        public static TImpl Fail(Exception e)
            => Fail(e.Message);

        public static TImpl Fail(string message)
        {
            return new TImpl
            {
                Successful = false,
                FailMessage = message
            };
        }
    }
}