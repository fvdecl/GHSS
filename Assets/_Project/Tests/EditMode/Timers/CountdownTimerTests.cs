using NUnit.Framework;
using GHSS.Core.Timers;

namespace GHSS.Tests.EditMode.Timers
{
    /// <summary>
    /// CountdownTimer advances only via an explicitly-passed deltaTime (Tick),
    /// never via its own Update() - so every state transition is testable with
    /// plain method calls, no coroutines, no Time.deltaTime, no frame waits.
    /// </summary>
    public class CountdownTimerTests
    {
        [Test]
        public void BeforeStart_IsIdle_RemainingEqualsFullDuration()
        {
            var timer = new CountdownTimer(10f);

            Assert.IsFalse(timer.IsRunning);
            Assert.AreEqual(10f, timer.Remaining);
            Assert.AreEqual(10f, timer.Duration);
        }

        [Test]
        public void Tick_BeforeStart_IsIgnored()
        {
            var timer = new CountdownTimer(10f);

            timer.Tick(4f);

            Assert.IsFalse(timer.IsRunning);
            Assert.AreEqual(10f, timer.Remaining);
        }

        [Test]
        public void Start_EntersRunningState()
        {
            var timer = new CountdownTimer(10f);

            timer.Start();

            Assert.IsTrue(timer.IsRunning);
            Assert.AreEqual(10f, timer.Remaining);
        }

        [Test]
        public void WhileRunning_TickDecreasesRemaining()
        {
            var timer = new CountdownTimer(10f);
            timer.Start();

            timer.Tick(4f);

            Assert.IsTrue(timer.IsRunning);
            Assert.AreEqual(6f, timer.Remaining);
        }

        [Test]
        public void ReachingZero_CompletesAndLeavesRunningState()
        {
            var timer = new CountdownTimer(5f);
            timer.Start();
            var completed = false;
            timer.Completed += () => completed = true;

            timer.Tick(5f);

            Assert.IsTrue(completed);
            Assert.IsFalse(timer.IsRunning);
            Assert.AreEqual(0f, timer.Remaining);
        }

        [Test]
        public void OvershootingDuration_ClampsAtZero_DoesNotGoNegative()
        {
            var timer = new CountdownTimer(5f);
            timer.Start();

            timer.Tick(999f);

            Assert.AreEqual(0f, timer.Remaining);
            Assert.IsFalse(timer.IsRunning);
        }

        [Test]
        public void Start_WhileAlreadyRunning_DoesNotResetProgress()
        {
            var timer = new CountdownTimer(10f);
            timer.Start();
            timer.Tick(4f);

            timer.Start();

            Assert.AreEqual(6f, timer.Remaining, "Start() must not restart an in-progress countdown.");
        }

        [Test]
        public void Start_AfterCompletion_RunsAgainFromFullDuration()
        {
            var timer = new CountdownTimer(5f);
            timer.Start();
            timer.Tick(5f);
            Assert.IsFalse(timer.IsRunning, "Precondition: timer must have completed.");

            timer.Start();

            Assert.IsTrue(timer.IsRunning);
            Assert.AreEqual(5f, timer.Remaining);
        }

        [Test]
        public void Ticked_ReportsRemainingValue()
        {
            var timer = new CountdownTimer(10f);
            timer.Start();
            float? reported = null;
            timer.Ticked += remaining => reported = remaining;

            timer.Tick(3f);

            Assert.AreEqual(7f, reported);
        }
    }
}
