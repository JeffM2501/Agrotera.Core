using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Core.Types;

namespace Entities.Classes.Components
{
    public class NavigationComputer
    {
        protected Ship Host = null;

        public bool ReadOnly = false;

        public enum NavigationModes
        {
            Drift,
            Direct,
            Heading,
            Course,
        }
        public NavigationModes Mode = NavigationModes.Drift;

		public double DesiredTurnSpeed = 0;
		public double DesiredSpeed = 0;

		public Rotation DesiredHeading = Rotation.Invalid;

        public bool AtHeading = false;

        public class CourseWaypoint : EventArgs
        {
            public Location TargetPosition = Location.Zero;
            public double AcceptableDistance = 0;
            public object Tag = null;

            public CourseWaypoint() { }
            public CourseWaypoint(Location l) { TargetPosition = l; }
        }

        public List<CourseWaypoint> Waypoints = new List<CourseWaypoint>();

        protected double CourseSpeed = 0;
        protected bool SteerToCourse = true;

        public event EventHandler<CourseWaypoint> ArrivedAtWaypoint;
        public event EventHandler<CourseWaypoint> CourseComplete;

		public event EventHandler HeadingReached = null;

        public NavigationComputer(Ship host)
        {
            Host = host;
        }

        public void SetDrift()
        {
            Mode = NavigationModes.Drift;
            SteerToCourse = false;
        }

        public void SetDirectNavigation(double turnSpeed, double speed)
        {
            Mode = NavigationModes.Direct;
            DesiredTurnSpeed = turnSpeed;
            DesiredSpeed = speed;

            SteerToCourse = false;
            DesiredHeading = Rotation.Invalid;
        }

        public void SteerTo(double heading, double speed)
        {
            Mode = NavigationModes.Heading;
            DesiredHeading = new Rotation(heading);
            DesiredSpeed = speed;

            SteerToCourse = false;

			AtHeading = false;
		}

        public void PlotCourse(List<CourseWaypoint> waypoints, double speed, bool startNow)
        {

            if (waypoints.Count == 0 || speed == 0)
            {
                Mode = NavigationModes.Direct;
                return;
            }

            DesiredSpeed = CourseSpeed = Math.Abs(speed);

            Mode = NavigationModes.Course;

            Waypoints.Clear();
            Waypoints.AddRange(waypoints.ToArray());
            SteerToCourse = startNow;

			if (SteerToCourse)
                SteerTo(Waypoints[0]);
        }

        public void ClearCourse()
        {
            Waypoints.Clear();
            Mode = NavigationModes.Heading;
        }

        public void SteerToWaypoint(CourseWaypoint waypoint, double speed)
        {
            Mode = NavigationModes.Heading;
            DesiredSpeed = Math.Abs(speed);
			AtHeading = false;
            SteerTo(waypoint);
        }

		public void SteerToLocation(Location waypoint, double speed)
		{
			Mode = NavigationModes.Heading;
			DesiredSpeed = Math.Abs(speed);
			AtHeading = false;
			SteerTo(waypoint);
		}

		public void PauseCoursePlot()
        {
            SteerToCourse = false;
            DesiredTurnSpeed = 0;
        }

        public void ResumeCoursePlot()
        {
            if (Waypoints.Count > 0)
            {
				Mode = NavigationModes.Course;
				SteerToCourse = true;
                DesiredSpeed = CourseSpeed;
            }
        }

        public void ResumeCoursePlot(double newSpeed)
        {
            if (Mode == NavigationModes.Course)
            {
                SteerToCourse = true;
                DesiredSpeed = CourseSpeed = newSpeed;
            }
        }

        public void AllStop()
        {
            if (Mode == NavigationModes.Course)
                PauseCoursePlot();

            DesiredTurnSpeed = 0;
            DesiredSpeed = 0;
        }

		protected void SteerTo(Location position)
		{
			Vector3D vecToTarget = Location.VectorTo(Host.Position, position);
			DesiredHeading = Rotation.FromVector3D(vecToTarget);
		}

        protected void SteerTo(CourseWaypoint waypoint)
        {
			SteerTo(waypoint.TargetPosition);
        }

        protected void UpdateCourse()
        {
            if (Mode != NavigationModes.Course || !SteerToCourse || DesiredSpeed == 0 || Waypoints.Count == 0)
                return;

            var waypoint = Waypoints[0];

            double dist = Location.Distance(Host.Position, waypoint.TargetPosition);
            if (dist > waypoint.AcceptableDistance)
                SteerTo(waypoint);
            else
            {
                CallAtWaypoint(waypoint);

                Waypoints.RemoveAt(0);

				if (Waypoints.Count == 0)
                {
                    Mode = NavigationModes.Heading;
                    if (CourseComplete != null)
                        CourseComplete.Invoke(Host, waypoint);
                }
                else
                    SteerTo(Waypoints[0]);
            }
        }

        private bool UseDirectNavValues()
        {
            return Mode == NavigationModes.Direct || (Mode == NavigationModes.Course && !SteerToCourse);
        }

        public void CallAtWaypoint(CourseWaypoint wp)
        {
            if (ArrivedAtWaypoint != null)
                ArrivedAtWaypoint.Invoke(Host, wp);
        }

        public void CallAtTargetHeading()
        {
            if (HeadingReached != null)
                HeadingReached.Invoke(Host, EventArgs.Empty);
        }

        public void Update()
        {
            if (Mode == NavigationModes.Drift || ReadOnly)
                return;

            UpdateCourse();

            if (Math.Abs(DesiredTurnSpeed) > Host.MaxTurnSpeed)
                DesiredTurnSpeed = Math.Sign(DesiredTurnSpeed) * Host.MaxTurnSpeed;

            if (Math.Abs(DesiredSpeed) > Host.MoveMaxSpeed)
                DesiredSpeed = Math.Sign(DesiredSpeed) * Host.MoveMaxSpeed;

            if (UseDirectNavValues())
            {
                Host.AngularVelocity = new Rotation(DesiredTurnSpeed);
            }
            else
            {
                Rotation targetHeading = DesiredHeading;

                var delta = Rotation.ShortRotationTo(Host.Orientation, DesiredHeading);

                if (Math.Abs(delta.Angle) <= (Host.MaxTurnSpeed * Timer.Delta * 2))
                {
					Host.Orientation = targetHeading;
					Host.AngularVelocity = Rotation.Zero;

                    if (!AtHeading)
                        CallAtTargetHeading();

                    AtHeading = true;
				}
                else
                    Host.AngularVelocity = new Rotation(Host.MaxTurnSpeed * Math.Sign(delta.Angle));
            }

            double currentSpeed = Host.Velocity.Length();
            if (currentSpeed < DesiredSpeed)
            {
                currentSpeed += Host.MoveAcceleration * Timer.Delta;
                if (currentSpeed > DesiredSpeed)
                    currentSpeed = DesiredSpeed;
            }
            else if (currentSpeed > DesiredSpeed)
            {
                currentSpeed -= Host.MoveAcceleration * Timer.Delta;
                if (currentSpeed < DesiredSpeed)
                    currentSpeed = DesiredSpeed;
            }

            Host.Velocity = Host.Orientation.ToVector3D() * currentSpeed;
        }
    }
}
