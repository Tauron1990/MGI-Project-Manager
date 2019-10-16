using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using Calculator.Shared.Dto;

namespace CalculatorUI
{
    public class ExpressionCollection : ObservableCollection<ExpressionEntry>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => base.OnCollectionChanged(e));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => base.OnPropertyChanged(e));
        }
    }
}