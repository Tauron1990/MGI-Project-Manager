using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Calculator.Shared.Commands;
using Calculator.Shared.Dto;
using Calculator.Shared.Querys;
using CQRSlite.Commands;
using CQRSlite.Queries;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.CQRS.Services.Extensions;

namespace CalculatorUI
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        private string _evaluationField;
        private ExpressionEntry _selectedEntry;
        public event PropertyChangedEventHandler PropertyChanged;

        private IServiceProvider ServiceProvider => App.ServiceProvider;

        public string EvaluationField
        {
            get => _evaluationField;
            set
            {
                if (value == _evaluationField) return;
                _evaluationField = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExpressionEntry> ExpressionEntries { get; } = new ObservableCollection<ExpressionEntry>();

        public ExpressionEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (Equals(value, _selectedEntry)) return;
                _selectedEntry = value;

                if(_selectedEntry != null)
                    EvaluationField = $"{_selectedEntry.Expression} = {_selectedEntry.Result}";

                OnPropertyChanged();
            }
        }

        public async Task Load()
        {
            ResultReciver.Result += s =>
            {
                EvaluationField = s.Error ? s.Result : $"{s.Expression} = {s.Result}"; ;

                ExpressionEntries.Add(new ExpressionEntry(s.Expression, s.Result));
            };

            await ServiceProvider.StartCQRS();

            foreach (var expressionEntry in await ServiceProvider.GetRequiredService<IQueryProcessor>().Query(new ExpressionsQuery())) 
                ExpressionEntries.Add(expressionEntry);
        }

        public async Task Eval()
        {
            if(string.IsNullOrWhiteSpace(EvaluationField)) return;

            var old = EvaluationField;
            EvaluationField = null;

            try
            {
                var sender = ServiceProvider.GetRequiredService<ICommandSender>();
                await sender.Send(new RunExpressionCommand {Input = old});
            }
            catch (Exception e)
            {
                EvaluationField = old;
                MessageBox.Show(Application.Current.MainWindow, e.Message, "Fehler", MessageBoxButton.OK);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}