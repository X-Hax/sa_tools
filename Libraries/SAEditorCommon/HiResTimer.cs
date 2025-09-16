#nullable enable

///////////////////////////////////////////////////////////////////////////////
//  File: HiResTimer.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Threading;

namespace SAModel.SAEditorCommon
{
	/// <summary>
	/// Represents a high resolution timer that allows precise timing even with sub-milliseconds intervals.
	/// The timer executes on a separated high priority thread.
	/// </summary>
	public class HiResTimer
	{
		#region Fields

		#region Static Fields

		/// <summary>
		/// The number of ticks per one millisecond.
		/// </summary>
		private static readonly float tickFrequency = 1000f / Stopwatch.Frequency;

		#endregion

		#region Instance Fields

		private volatile float interval;
		private volatile float ignoreElapsedThreshold = Single.PositiveInfinity;
		private volatile bool isRunning;

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the <see cref="Interval"/> elapses.
		/// </summary>
		public event EventHandler<HiResTimerElapsedEventArgs>? Elapsed;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the interval, in milliseconds, before <see cref="Elapsed"/> event is triggered.
		/// Fractional values are allowed, too. When zero, the <see cref="Elapsed"/> event is triggered as often as possible.
		/// <br/>Default value: <c>1.0</c>, if initialized by the default constructor; otherwise, as specified in the constructor.
		/// </summary>
		/// <value>
		/// The interval in milliseconds. For example, <c>1000</c> represents one second and <c>0.001</c> represents one microsecond.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative or <see cref="Single.NaN"/>.</exception>
		/// <remarks>
		/// <note>Please note that if <see cref="Interval"/> is smaller than <c>16</c>, then the timer may consume much CPU when running.</note>
		/// </remarks>
		public float Interval
		{
			get => interval;
			set
			{
				if (value < 0f || Single.IsNaN(value))
					throw new ArgumentOutOfRangeException();
				interval = value;
			}
		}

		/// <summary>
		/// Gets or sets a threshold value, in milliseconds, to ignore an <see cref="Elapsed"/> event (and thus trying to catch up the timer)
		/// if the next invoke is late by the given value. Value must not be zero but fractions are allowed.
		/// <br/>Default value: <c>+∞</c>.
		/// </summary>
		/// <remarks>
		/// <note>
		/// If the value of this property is too low (smaller than the execution time of the <see cref="Elapsed"/> event), it may
		/// cause that the <see cref="Elapsed"/> event is never triggered again.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is zero or negative or <see cref="Single.NaN"/>.</exception>
		public float IgnoreElapsedThreshold
		{
			get => ignoreElapsedThreshold;
			set
			{
				if (value <= 0f || Single.IsNaN(value))
					throw new ArgumentOutOfRangeException();
				ignoreElapsedThreshold = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the <see cref="Elapsed"/> event should be triggered.
		/// <br/>Default value: <see langword="false"/>.
		/// </summary>
		/// <value>
		/// <see langword="true"/>&#160;if enabled; otherwise, <see langword="false"/>.
		/// </value>
		public bool Enabled
		{
			get => isRunning;
			set
			{
				if (value)
					Start();
				else
					Stop();
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="HiResTimer"/> class with 1ms interval.
		/// </summary>
		public HiResTimer() : this(1f)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HiResTimer"/> class with a specified <paramref name="interval"/>.
		/// </summary>
		/// <param name="interval">The time, in milliseconds, between events. Value must be non-negative. Fractional values are allowed.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> is negative or <see cref="Single.NaN"/>.</exception>
		public HiResTimer(float interval)
		{
			if (interval < 0f || Single.IsNaN(interval))
				throw new ArgumentOutOfRangeException();
			this.interval = interval;
		}

		#endregion

		#region Methods

		#region Static Methods

		private static float ElapsedHiRes(Stopwatch stopwatch) => stopwatch.ElapsedTicks * tickFrequency;

		#endregion

		#region Instance Methods

		#region Public Methods

		/// <summary>
		/// Starts raising the <see cref="Elapsed"/> event by enabling the timer.
		/// </summary>
		public void Start()
		{
			if (isRunning)
				return;

			isRunning = true;
			Thread thread = new Thread(ExecuteTimer) { Priority = ThreadPriority.Highest };
			thread.Start();
		}

		/// <summary>
		/// Stops raising the <see cref="Elapsed"/> event by disabling the timer.
		/// </summary>
		public void Stop() => isRunning = false;

		#endregion

		#region Private Methods

		/// <summary>
		/// The timer loop on a dedicated thread.
		/// Works like an inverse SpinWait in terms of sleeping/spinning strategy: while SpinWait spins for short periods in the beginning and then starts to sleep,
		/// this timer sleeps more often in the beginning (if there is enough time), and starts to spin just before triggering the next event.
		/// </summary>
		private void ExecuteTimer()
		{
			int fallouts = 0;
			float nextTrigger = 0f;

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			while (isRunning)
			{
				float intervalLocal = interval;
				nextTrigger += intervalLocal;
				float elapsed;

				while (true)
				{
					elapsed = ElapsedHiRes(stopwatch);
					float diff = nextTrigger - elapsed;
					if (diff <= 0f)
						break;

					if (diff < 1f)
						Thread.SpinWait(10);
					else if (diff < 10f)
						Thread.SpinWait(100);
					else
					{
						// By default Sleep(1) lasts about 15.5 ms (if not configured otherwise for the application by WinMM, for example)
						// so not allowing sleeping under 16 ms. Not sleeping for more than 50 ms so interval changes/stopping can be detected.
						if (diff >= 16f)
							Thread.Sleep(diff >= 100f ? 50 : 1);
						else
						{
							Thread.SpinWait(1000);
							Thread.Sleep(0);
						}

						// if we have a larger time to wait, we check if the interval has been changed in the meantime
						float newInterval = interval;

						// ReSharper disable once CompareOfFloatsByEqualityOperator
						if (intervalLocal != newInterval)
						{
							nextTrigger += newInterval - intervalLocal;
							intervalLocal = newInterval;
						}
					}

					if (!isRunning)
						return;
				}


				float delay = elapsed - nextTrigger;
				if (delay >= ignoreElapsedThreshold)
				{
					fallouts += 1;
					continue;
				}

				Elapsed?.Invoke(this, new HiResTimerElapsedEventArgs(delay, fallouts));
				fallouts = 0;

				// restarting the timer in every hour to prevent precision problems
				if (stopwatch.Elapsed.TotalHours >= 1d)
				{
#if NET35
                    stopwatch.Reset();
                    stopwatch.Start();
#else
					stopwatch.Restart();
#endif
					nextTrigger = 0f;
				}
			}

			stopwatch.Stop();
		}

		#endregion

		#endregion

		#endregion
	}

	/// <summary>
	/// Provides data for the <see cref="HiResTimer.Elapsed">HiResTimer.Elapsed</see> event.
	/// </summary>
	public class HiResTimerElapsedEventArgs : EventArgs
	{
		#region Properties

		/// <summary>
		/// Gets the delay, in milliseconds, of the triggering of the <see cref="HiResTimer.Elapsed">HiResTimer.Elapsed</see> event
		/// compared to when it should have been called.
		/// </summary>
		public float Delay { get; }

		/// <summary>
		/// Gets the number of the fallen out <see cref="HiResTimer.Enabled">HiResTimer.Enabled</see> events since the last invoke.
		/// The value is nonzero if a larger delay occurred than the value of the <see cref="HiResTimer.IgnoreElapsedThreshold">HiResTimer.IgnoreElapsedThreshold</see> property
		/// and thus one or more <see cref="HiResTimer.Elapsed">HiResTimer.Elapsed</see> events were skipped to catch up the timer.
		/// </summary>
		public int Fallouts { get; }

		#endregion

		#region Constructors

		internal HiResTimerElapsedEventArgs(float delay, int fallouts)
		{
			Delay = delay;
			Fallouts = fallouts;
		}

		#endregion
	}
}