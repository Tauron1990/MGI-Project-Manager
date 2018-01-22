namespace Tauron.Application
{
	public class ActiveProgress
	{
		public string Message { get; private set; }
		public double Percent { get; private set; }
		public double OverAllProgress { get; set; }

		public ActiveProgress(string message, double percent, double overAllProgress)
		{
			if (percent > 100)
				Percent = 100;
			if (overAllProgress > 100)
				OverAllProgress = 100;

			Message = message;
			Percent = percent;
			OverAllProgress = overAllProgress;
		}
	}
}