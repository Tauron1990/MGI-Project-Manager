using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.Data;
using Tauron.Application.MgiProjectManager.Resources;
using Tauron.Application.MgiProjectManager.ServiceLayer;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.TimeCalculationModel)]
    public class TimeCalculationModel : ModelBase
    {
        private static void PropertyChangedCommon(ObservableProperty prop, ModelBase model, object value) => ((TimeCalculationModel)model).SetReultForCalculation();
        private static void PropertyChangedProblems(ObservableProperty prop, ModelBase model, object value)
        {
            var obj = ((TimeCalculationModel) model);
            if ((bool) value)
                obj.BigProblems = false;
           obj.SetReultForCalculation();
        }
        private static void PropertyChangedBigProblems(ObservableProperty prop, ModelBase model, object value)
        {
            var obj = ((TimeCalculationModel) model);
            if ((bool) value)
                obj.Problems = false;
            obj.SetReultForCalculation();
        }

        public static readonly ObservableProperty SpeedProperty = RegisterProperty(nameof(Speed), typeof(TimeCalculationModel), typeof(double?), new ObservablePropertyMetadata(PropertyChangedCommon));
        public static readonly ObservableProperty AmountProperty = RegisterProperty(nameof(Amount), typeof(TimeCalculationModel), typeof(long?), new ObservablePropertyMetadata(PropertyChangedCommon));
        public static readonly ObservableProperty IterationsProperty = RegisterProperty(nameof(Iterations), typeof(TimeCalculationModel), typeof(long?), new ObservablePropertyMetadata(PropertyChangedCommon));
        public static readonly ObservableProperty ProblemsProperty = RegisterProperty(nameof(Problems), typeof(TimeCalculationModel), typeof(bool), new ObservablePropertyMetadata(PropertyChangedProblems));
        public static readonly ObservableProperty BigProblemsProperty = RegisterProperty(nameof(BigProblems), typeof(TimeCalculationModel), typeof(bool), new ObservablePropertyMetadata(PropertyChangedBigProblems));
        public static readonly ObservableProperty PaperFormatProperty = RegisterProperty(nameof(PaperFormat), typeof(TimeCalculationModel), typeof(string), new ObservablePropertyMetadata(PropertyChangedCommon));
        public static readonly ObservableProperty StartDateTimeProperty = RegisterProperty(nameof(StartDateTime), typeof(TimeCalculationModel), typeof(DateTime), new ObservablePropertyMetadata(PropertyChangedCommon));
        public static readonly ObservableProperty RunTimeProperty = RegisterProperty(nameof(RunTime), typeof(TimeCalculationModel), typeof(TimeSpan), new ObservablePropertyMetadata(PropertyChangedCommon));

        [InjectModel(AppConststands.TaskManager)]
        public TaskManagerModel TaskManagerModel { get; set; }

        [Inject]
        public ITimeCalculator TimeCalculator { get; set; }

        public double? Speed
        {
            get => GetValue<double?>(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }
        public long? Amount
        {
            get => GetValue<long?>(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
        public long? Iterations
        {
            get => GetValue<long?>(IterationsProperty);
            set => SetValue(IterationsProperty, value);
        }
        public bool Problems
        {
            get => GetValue<bool>(ProblemsProperty);
            set => SetValue(ProblemsProperty, value);
        }
        public bool BigProblems
        {
            get => GetValue<bool>(BigProblemsProperty);
            set => SetValue(BigProblemsProperty, value);
        }
        public string PaperFormat
        {
            get => GetValue<string>(PaperFormatProperty);
            set => SetValue(PaperFormatProperty, value);
        }
        public DateTime StartDateTime
        {
            get => GetValue<DateTime>(StartDateTimeProperty);
            set => SetValue(StartDateTimeProperty, value);
        }
        public TimeSpan RunTime
        {
            get => GetValue<TimeSpan>(RunTimeProperty);
            set => SetValue(RunTimeProperty, value);
        }

        private void SetReultForCalculation()
        {
            var cresult = TimeCalculator.InsertValidation(new ValidationInput(Amount, Iterations, RunTime, new PaperFormat(PaperFormat), Speed));

            _insertOk = cresult.NormalizedTime != null;

            if (_insertOk)
            {
                Status = cresult.FormatedResult;
                StatusOk = true;
            }
            else
            {
                Status = UIResources.ResourceManager.GetString(cresult.FormatedResult);
                StatusOk = false;
            }

            CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        private bool _insertOk;
        private bool _saveOperationCompled;

        private string _status;
        private bool _statusOk;

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public bool StatusOk
        {
            get => _statusOk;
            set => SetProperty(ref _statusOk, value);
        }



        //private const string RuntimeCalculatorProperty = nameof(RuntimeCalculatorProperty);
        //private const string InProgressProperty        = nameof(InProgressProperty);

        //#region Common

        //private readonly Dictionary<string, object> _propertys = new Dictionary<string, object>();

        //private TType GetProperty<TType>(Expression<Func<TType>> exp)
        //{
        //    var name = PropertyHelper.ExtractPropertyName(exp);

        //    if (_propertys.TryGetValue(name, out var value)) return (TType) value;

        //    return default(TType);
        //}

        //private void SetProperty<TType>(Expression<Func<TType>> exp, TType value, Action changed = null)
        //{
        //    var comparer = EqualityComparer<TType>.Default;

        //    var name = PropertyHelper.ExtractPropertyName(exp);
        //    if (_propertys.TryGetValue(name, out var propValue) && comparer.Equals((TType) propValue, value)) return;

        //    _propertys[name] = value;
        //    OnPropertyChangedExplicit(name);

        //    changed?.Invoke();
        //}

        //private JobItem _currentItem;

        //public TimeCalculationModel()
        //{
        //    //PropertyChanged += OnPropertyChanged;
        //    _speedNotes     =  new SpeedNotesHolder();
        //}

        //public override void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        //{
        //    base.OnPropertyChanged(sender, propertyChangedEventArgs);

        //    switch (propertyChangedEventArgs.PropertyName)
        //    {
        //        case nameof(CalcAmount):
        //        case nameof(CalcIterations):
        //        case nameof(CalcPaperFormat):
        //        case nameof(CalcMikron):
        //        case nameof(CalcSpeed):
        //            SetResultCalculation();
        //            break;
        //        case nameof(PaperFormat):
        //        case nameof(RunTime):
        //        case nameof(Amount):
        //        case nameof(Iterations):
        //        case nameof(Speed):
        //            SetReultOfInsert();
        //            break;
        //    }
        //}

        //public void ChangeJob(JobItem item)
        //{
        //    _currentItem = item;
        //    Rows = 2;

        //    var info = TimeCalculator.FetchJobInfo(_currentItem.CreateDto());

        //    Reset();
        //    SetResultCalculation();

        //    if (!info.IsValid) return;

        //    CalcAmount              = info.Amount;
        //    CalcPaperFormat         = new PaperFormat(info.Width, info.Length).ToString();
        //    _fullRunTime            = info.EffectiveTime;
        //    CalcSpeed               = info.Speed;
        //    CalcIterations          = info.Iterations;
        //    _calcTime               = DateTime.Now;
        //    _calculationSuccessFull = true;

        //    if (item.Status == JobStatus.InProgress)
        //        Exchange();
        //}

        //#endregion

        //#region Operations



        //#endregion

        //#region Insert

        //private int? _setupTime;
        //private int? _iterationTime;


        //public void Reset()
        //{
        //    ResetCalc();
        //    Speed                 = null;
        //    Amount                = null;
        //    Iterations            = null;
        //    StartDateTime         = DateTime.Now;
        //    RunTime               = TimeSpan.Zero;
        //    Problems              = false;
        //    BigProblems           = false;
        //    PaperFormat           = null;
        //    Status                = string.Empty;
        //    _saveOperationCompled = false;
        //    SetReultOfInsert();
        //}

        //public double? Speed
        //{
        //    get => GetProperty(() => Speed);
        //    set => SetProperty(() => Speed, value);
        //}

        //public long? Amount
        //{
        //    get => GetProperty(() => Amount);
        //    set => SetProperty(() => Amount, value);
        //}

        //public long? Iterations
        //{
        //    get => GetProperty(() => Iterations);
        //    set => SetProperty(() => Iterations, value);
        //}

        //public bool Problems
        //{
        //    get => GetProperty(() => Problems);
        //    set
        //    {
        //        SetProperty(() => Problems, value, () =>
        //                                           {
        //                                               if (value) BigProblems = false;
        //                                           });
        //    }
        //}

        //public bool BigProblems
        //{
        //    get => GetProperty(() => BigProblems);
        //    set => SetProperty(() => BigProblems, value, () =>
        //                                                 {
        //                                                     if (value) Problems = false;
        //                                                 });
        //}

        //public string PaperFormat
        //{
        //    get => GetProperty(() => PaperFormat);
        //    set => SetProperty(() => PaperFormat, value);
        //}

        //public DateTime StartDateTime
        //{
        //    get => GetProperty(() => StartDateTime);
        //    set => SetProperty(() => StartDateTime, value);
        //}


        //public TimeSpan RunTime
        //{
        //    get => GetProperty(() => RunTime);
        //    set => SetProperty(() => RunTime, value);
        //}

        //public string Result
        //{
        //    get => GetProperty(() => Result);
        //    set => SetProperty(() => Result, value);
        //}

        //private void SetReultOfInsert()
        //{
        //    if (_saveOperationCompled)
        //        CurrentDispatcher.BeginInvoke(Reset);

        //    var cresult = TimeCalculator.InsertValidation(new ValidationInput(Amount, Iterations, RunTime, new PaperFormat(PaperFormat), Speed));

        //    _insertOk = cresult.NormalizedTime != null;

        //    if (_insertOk)
        //    {
        //        Result = cresult.FormatedResult;
        //        Status = null;
        //    }
        //    else
        //    {
        //        Result = null;
        //        Status = UIResources.ResourceManager.GetString(cresult.FormatedResult);
        //    }

        //    CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        //}

        //private bool _insertOk;
        //private bool _saveOperationCompled;

        //public string Status
        //{
        //    get => GetProperty(() => Status);
        //    set => SetProperty(() => Status, value);
        //}

        //public bool StatusOk
        //{
        //    get => GetProperty(() => StatusOk);
        //    set => SetProperty(() => StatusOk, value);
        //}

        //public void Save()
        //{
        //    TaskManagerModel.RunTask(() =>
        //                             {
        //                                 try
        //                                 {
        //                                     var result = TimeCalculator.Save(new SaveInput(Amount, Iterations, Problems, BigProblems, new PaperFormat(PaperFormat), Speed,
        //                                                                                    StartDateTime, RunTime, _setupTime, _iterationTime, _currentItem.CreateDto()));

        //                                     if (result.Succsess)
        //                                     {
        //                                         Status         = UIResources.LabelTimeSaveJobTimeSuccess;
        //                                         StatusOk       = true;
        //                                         _iterationTime = null;
        //                                         _setupTime     = null;
        //                                     }
        //                                     else
        //                                     {
        //                                         Status   = result.Message;
        //                                         StatusOk = false;
        //                                     }
        //                                 }
        //                                 catch (Exception e)
        //                                 {
        //                                     if (e is OutOfMemoryException || e is Win32Exception || e is StackOverflowException)
        //                                         throw;

        //                                     Log.Error(e, "Error on Time Calc Validation");
        //                                     StatusOk = false;

        //                                     Status = $"{UIResources.LabelCommonError}: {e.GetType()}-{e.Message}";
        //                                 }

        //                                 _saveOperationCompled = true;
        //                             }, UIResources.LabelTimeSaveJobTime);
        //}

        //public bool CanSave()
        //{
        //    return !_saveOperationCompled && _insertOk;
        //}

        //public void RecalulateSetup(RuntimeCalculatorResult effectiveTime)
        //{
        //    RecalulateSetupImpl(effectiveTime);
        //}

        //private void RecalulateSetupImpl(RuntimeCalculatorResult effectiveTime)
        //{
        //    TaskManagerModel.RunTask(() =>
        //                             {
        //                                 RunTime        = TimeSpan.Zero;
        //                                 _setupTime     = null;
        //                                 _iterationTime = null;
        //                                 if (effectiveTime == null) return;

        //                                 TimeCalculator.AddSetupItems(new AddSetupInput(effectiveTime.Iterations.Concat(new[] {effectiveTime.Setup}).Select(i => i.CreateDto())));
        //                                 TimeCalculator.RecalculateSetup();

        //                                 if (effectiveTime.Runtime == null) return;

        //                                 _setupTime     = effectiveTime.Setup.CalculateDiffernce()?.Minutes;
        //                                 _iterationTime = effectiveTime.Iterations.Sum(i => i.CalculateDiffernce()?.Minutes);

        //                                 RunTime = effectiveTime.Runtime.Value;
        //                                 CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        //                             }, UIResources.TimeCalcRuntimeCalculation);
        //}

        //#endregion

        //#region Calculation

        //private SpeedNotesHolder _speedNotes;
        //private bool             _canCalculate;
        //private TimeSpan?        _fullRunTime;
        //private bool             _calculationSuccessFull;
        //private DateTime         _calcTime;

        //public string CalculatetRunTime
        //{
        //    get => GetProperty(() => CalculatetRunTime);
        //    set => SetProperty(() => CalculatetRunTime, value);
        //}

        //public string CalculatetSetupTime
        //{
        //    get => GetProperty(() => CalculatetSetupTime);
        //    set => SetProperty(() => CalculatetSetupTime, value);
        //}

        //public string CalculatetFullTime
        //{
        //    get => GetProperty(() => CalculatetFullTime);
        //    set => SetProperty(() => CalculatetFullTime, value);
        //}

        ////public string CalculationStatus
        ////{
        ////    get => GetProperty(() => CalculationStatus);
        ////    set => SetProperty(() => CalculationStatus, value);
        ////}


        //public long? CalcAmount
        //{
        //    get => GetProperty(() => CalcAmount);
        //    set => SetProperty(() => CalcAmount, value);
        //}

        //public long? CalcIterations
        //{
        //    get => GetProperty(() => CalcIterations);
        //    set => SetProperty(() => CalcIterations, value);
        //}

        //public string CalcPaperFormat
        //{
        //    get => GetProperty(() => CalcPaperFormat);
        //    set => SetProperty(() => CalcPaperFormat, value);
        //}

        //public double? CalcSpeed
        //{
        //    get => GetProperty(() => CalcSpeed);
        //    set => SetProperty(() => CalcSpeed, value);
        //}

        //public int? Rows
        //{
        //    get => GetProperty(() => Rows);
        //    set => SetProperty(() => Rows, value, CalcMikronChanged);
        //}

        //public long? CalcMikron
        //{
        //    get => GetProperty(() => CalcMikron);
        //    set => SetProperty(() => CalcMikron, value, CalcMikronChanged);
        //}

        //private void CalcMikronChanged()
        //{
        //    if (CalcMikron == null || Rows == null) return;

        //    CalcSpeed = _speedNotes.CalculateSpeed(Rows.Value, (int) CalcMikron.Value);
        //}

        //private void SetResultCalculation()
        //{
        //    var result = TimeCalculator.CalculateValidation(new CalculateTimeInput(CalcIterations, new PaperFormat(CalcPaperFormat), CalcAmount, CalcSpeed));

        //    Status = !result.Valid ? UIResources.ResourceManager.GetString(result.Message) : "Start Bereit";

        //    _canCalculate           = result.Valid;
        //    _calculationSuccessFull = false;

        //    CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        //}

        //public void Calculate()
        //{
        //    TaskManagerModel.RunTask(() =>
        //                             {
        //                                 _calcTime = DateTime.Now;

        //                                 var output = TimeCalculator.CalculateTime(new CalculateTimeInput(CalcIterations, new PaperFormat(CalcPaperFormat), CalcAmount, CalcSpeed));
        //                                 if (output.PrecisionMode == PrecisionMode.InValid || output.PrecisionMode == PrecisionMode.NoData)
        //                                 {
        //                                     CalculatetFullTime      = "0";
        //                                     CalculatetRunTime       = "0";
        //                                     CalculatetSetupTime     = "0";
        //                                     Status       = UIResources.ResourceManager.GetString(output.Error);
        //                                     _calculationSuccessFull = output.PrecisionMode != PrecisionMode.InValid;
        //                                     _fullRunTime            = null;
        //                                 }
        //                                 else
        //                                 {
        //                                     _fullRunTime        = output.IterationTime + output.RunTime + output.SetupTime;
        //                                     Status   = $"{UIResources.CommonLabelOk} ({FormatPrecision(output.PrecisionMode)})";
        //                                     CalculatetRunTime   = FormatTime(output.RunTime);
        //                                     CalculatetFullTime  = FormatTime(_fullRunTime);
        //                                     CalculatetSetupTime = $"{FormatTime(output.IterationTime + output.SetupTime)} ({UIResources.LabelTimeCalcSetupTime}: {FormatTime(output.SetupTime)}" +
        //                                                           $"- {UIResources.LabelTimeCalcIterationTime}: {FormatTime(output.IterationTime)})";
        //                                     _calculationSuccessFull = true;
        //                                 }

        //                                 CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        //                             }, UIResources.TitleTimeCalcCalculation);
        //}

        //private string FormatTime(TimeSpan? value)
        //{
        //    return value?.ToString("hh:mm");
        //}

        //private string FormatPrecision(PrecisionMode precisionMode)
        //{
        //    switch (precisionMode)
        //    {
        //        case PrecisionMode.Perfect:
        //            return UIResources.PrecisionModePerfect;
        //        case PrecisionMode.NearCorner:
        //            return UIResources.PrecisionModeNearCorner;
        //        case PrecisionMode.AmountMismatchPerfect:
        //            return UIResources.PrecisionModeAmountMismatchPerfect;
        //        case PrecisionMode.AmountMismatchNearCorner:
        //            return UIResources.PrecisionModeAmountMismatchNearCorner;
        //        case PrecisionMode.NoData:
        //            return UIResources.PrecisionModeNoData;
        //        default:
        //            return string.Empty;
        //    }
        //}

        //[UsedImplicitly]
        //public bool CanCalculate()
        //{
        //    return _canCalculate;
        //}

        //private void ResetCalc()
        //{
        //    CalcAmount          = null;
        //    CalcMikron          = null;
        //    CalcIterations      = null;
        //    CalcSpeed           = null;
        //    CalcPaperFormat     = null;
        //    CalculatetFullTime  = string.Empty;
        //    CalculatetRunTime   = string.Empty;
        //    CalculatetSetupTime = string.Empty;
        //}

        //public void Exchange()
        //{
        //    PaperFormat   = CalcPaperFormat;
        //    RunTime       = _fullRunTime ?? TimeSpan.Zero;
        //    Speed         = CalcSpeed;
        //    Amount        = CalcAmount;
        //    Iterations    = CalcIterations;
        //    StartDateTime = _calcTime;

        //    RecalulateSetupImpl(null);
        //}

        //[UsedImplicitly]
        //public bool CanExchange()
        //{
        //    return _calculationSuccessFull;
        //}

        //#endregion
    }
}