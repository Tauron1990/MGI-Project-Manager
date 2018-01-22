using System.ComponentModel;

namespace Tauron.Application.Models
{
    public interface IModel : INotifyPropertyChanged, IEditableObject, INotifyDataErrorInfo
    {
    }
}