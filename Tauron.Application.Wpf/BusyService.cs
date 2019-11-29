namespace Tauron.Application.Wpf
{
    //[ServiceDescriptor(typeof(IPleaseWaitService), ServiceLifetime.Singleton)]
    //public sealed class BusyService : ObservableObject, IPleaseWaitService
    //{
    //    public string Status { get; private set; }

    //    public bool Run { get; set; }

    //    public void Show(string status = "") 
    //        => Push(status);

    //    public void Show(PleaseWaitWorkDelegate workDelegate, string status = "")
    //    {
    //        try
    //        {
    //            Push(status);
    //            workDelegate();
    //        }
    //        finally
    //        {
    //            Pop();
    //        }
    //    }

    //    public void UpdateStatus(string status) 
    //        => Status = status;

    //    public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "") 
    //        => Status = string.Format(statusFormat, currentItem, totalItems);

    //    public void Hide() => Pop();

    //    public void Push(string status = "")
    //    {
    //        Run = true;
    //        Status = status;
    //        ShowCounter++;
    //    }

    //    public void Pop()
    //    {
    //        ShowCounter++;
    //        if (ShowCounter > 0) return;

    //        Run = false;
    //        ShowCounter = 0;
    //    }

    //    public int ShowCounter { get; private set; }
    //}
}