using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ShipClient;
using Core.Types;
using ScenarioServer;
using ScenarioServer.Interfaces;
using System.IO;
using System.Reflection;

namespace ClientValidator
{
    public partial class Form1 : Form
    {
        ShipConnection Connection = null;

		ScenarioState State = null;

		public class GraphicInfo
        {
            public Brush Color = Brushes.Magenta;
            public Image Pic = null;
            public float Size = 5;

            public GraphicInfo(Brush c, float s)
            {
                Color = c;
                Size = s;
            }

            public GraphicInfo(Image p, float s)
            {
                Color = Brushes.White;
                Pic = p;
                Size = s;
            }
        }

        public Dictionary<string, GraphicInfo> MapElementColors = new Dictionary<string, GraphicInfo>();

        public Form1()
        {
            InitializeComponent();
            ViewType.SelectedIndex = 0;

            timer1.Start();


            //    Application.Idle += Application_Idle;

            MapElementColors.Add("Station", new GraphicInfo(imageList1.Images[0],32));
            MapElementColors.Add("CargoStack", new GraphicInfo(Brushes.Green,10));
            MapElementColors.Add("Shuttle", new GraphicInfo(Brushes.Blue,5));
            MapElementColors.Add("PlayerShip", new GraphicInfo(Brushes.White,8));
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (Connection != null)
            {
                Connection.Update(); 
            }
        }


        private void ConnectButton_Click(object sender, EventArgs e)
        {
            Connection = new ShipConnection("localhost", 2501);
            Connection.Connected += Connection_Connected;
            Connection.Disconnected += Connection_Disconnected;
            Connection.ShipAssigned += Connection_ShipAssigned;

        }

        private void Connection_ShipAssigned(object sender, EventArgs e)
        {
            StatusText.Text = "Ship Assigned";
            Map.Invalidate();
            Connection.PlayerShip.SensorEntityAppeared += PlayerShip_SensorEntityAppeared;
            Connection.PlayerShip.SensorEntityRemoved += PlayerShip_SensorEntityRemoved;
            Connection.PlayerShip.SensorEntityUpdated += PlayerShip_SensorEntityUpdated;
        }

        private void PlayerShip_SensorEntityUpdated(object sender, Entities.Classes.Ship.KnownEntity e)
        {
            Map.Invalidate();
          //  throw new NotImplementedException();
        }

        private void PlayerShip_SensorEntityRemoved(object sender, Entities.Classes.Ship.KnownEntity e)
        {
            Map.Invalidate();
            // throw new NotImplementedException();
        }

        private void PlayerShip_SensorEntityAppeared(object sender, Entities.Classes.Ship.KnownEntity e)
        {
            Map.Invalidate();
            //  throw new NotImplementedException();
        }

        private void Connection_Disconnected(object sender, EventArgs e)
        {
            StatusText.Text = "Disconnected";
        }

        private void Connection_Connected(object sender, ShipConnection.ConnectedEventArgs e)
        {
            StatusText.Text = "Connected";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
			if(State != null)
			{
				State.Update();
				Map.Invalidate();
			}

			if (Connection != null)
            {
                Connection.Update();

                if (Connection.PlayerShip != null)
                {
                    ShipSpeed = Connection.PlayerShip.Velocity.Length();
                    var dir = Vector3D.Normalize(Connection.PlayerShip.Velocity);

                    ShipDirection = Math.Atan2(dir.Y, dir.X) * Rotation.DegCon;
                }
            }
   
        }

		private void AddEntityDetails(Entities.Entity ent)
		{
			if(UpdateCount != 10)
				return;

			var item = ElementList.Items.Add(ent.ID.ToString());
			item.SubItems.Add(ent.Name);
            if (Connection != null && State != null)
            {
                var serverEnt = State.MapItems.GetByID(ent.ID);
                double error = Core.Types.Location.Distance(ent.Position, serverEnt.Position);
                item.SubItems.Add(error.ToString());
                item.SubItems.Add(ent.Velocity.ToString());
            }
            else
            {
                item.SubItems.Add(ent.Orientation.Angle.ToString());
                item.SubItems.Add(ent.AngularVelocity.Angle.ToString());
            }
		}

		private void DrawEntityShape(Entities.Entity ent, Graphics g, float x, float y)
		{
			AddEntityDetails(ent);

			float size = 5;
			Brush b = Brushes.LightYellow;
			Image pic = null;

			if(MapElementColors.ContainsKey(ent.VisualGraphics))
			{
				var d = MapElementColors[ent.VisualGraphics];
				if(d.Pic == null)
					b = d.Color;
				else
					pic = d.Pic;

				size = d.Size;
			}

			float halfSize = size * 0.5f;

			RectangleF rect = new RectangleF(x - halfSize, y - halfSize, size, size);

			if(pic != null)
				g.DrawImage(pic, rect);
			else
				g.FillRectangle(b, rect);

			double lineScale = 5.0 * (1.0 / ViewScale);
			Vector3D orient = ent.Orientation.TransformVec(new Vector3D(lineScale, 0, 0));

			g.DrawLine(Pens.Aqua, x, y, x + (float)orient.X, y + (float)orient.Y);
		}

		public double ViewScale = 0.5;

		int UpdateCount = 0;

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            UpdateStatusMarker();

            e.Graphics.Clear(Color.Black);

            if (Connection == null && State == null)
                return;

			UpdateCount++;
			if (UpdateCount == 10)
				ElementList.Items.Clear();

			e.Graphics.TranslateTransform(Map.Width * 0.5f, Map.Height * 0.5f);

            e.Graphics.DrawLine(Pens.DarkGreen, 0, Map.Height * 0.5f, 0, -Map.Height * 0.5f);
            e.Graphics.DrawLine(Pens.DarkGreen, Map.Width * 0.5f,0, -Map.Width * 0.5f, 0);

			if (Connection != null && Connection.PlayerShip != null)
			{
				foreach(var i in Connection.PlayerShip.KnownEntities.Values)
				{

					if(ViewType.SelectedIndex == 2)
					{
						UserShip.ShipCentricSensorEntity scs = i as UserShip.ShipCentricSensorEntity;

						DrawEntityShape(i.BaseEntity, e.Graphics, (float)(scs.ShipRelativePosition.X * ViewScale), (float)(scs.ShipRelativePosition.Y * ViewScale));

						//                     rect.X = (float)((i.LastPosition.X - Connection.PlayerShip.Position.X) * scale);
						//                     rect.Y = (float)((i.LastPosition.Y - Connection.PlayerShip.Position.Y) * scale);
					}
					else if(ViewType.SelectedIndex == 1)
					{
						DrawEntityShape(i.BaseEntity, e.Graphics, (float)(i.BaseEntity.Position.X * ViewScale), (float)(i.BaseEntity.Position.Y * ViewScale));
					}
					else if(ViewType.SelectedIndex == 0)
					{
						DrawEntityShape(i.BaseEntity, e.Graphics, (float)(i.LastPosition.X * ViewScale), (float)(i.LastPosition.Y * ViewScale));
					}
				}
			}
			else if (State != null)
			{
				foreach(var ent in State.MapItems.Ents.Values)
				{
					DrawEntityShape(ent, e.Graphics, (float)(ent.Position.X * ViewScale), (float)(ent.Position.Y * ViewScale));
				}
			}

			if(UpdateCount == 10)
				UpdateCount = 0;

		}

        private void ViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Map.Invalidate();
        }

		private void StartHost_Click(object sender, EventArgs e)
		{
			ScenarioControllerLoader loader = new ScenarioControllerLoader();
			loader.Scan(Assembly.GetExecutingAssembly());   // aways add ourselves

			var dir = ScenariosDir();
			if(dir.Exists)
				LoadScenarios(dir, loader);

			State = new ScenarioState(loader.GetDefaultScenario());
			State.Startup();
		}

		static void LoadScenarios(DirectoryInfo dir, ScenarioControllerLoader loader)
		{
			foreach(var f in dir.GetFiles("*.dll"))
				loader.Scan(Assembly.LoadFile(f.FullName));

			foreach(var d in dir.GetDirectories())
				LoadScenarios(d, loader);
		}

		static DirectoryInfo ScenariosDir()
		{
			return new DirectoryInfo(Path.Combine(GetExeDir().FullName, "scenarios"));
		}

		static DirectoryInfo GetExeDir()
		{
			return new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
		}

		private void ZoomIn_Click(object sender, EventArgs e)
		{
			ViewScale += 0.01;
			Map.Invalidate();
		}

		private void ZoomOut_Click(object sender, EventArgs e)
		{
			ViewScale -= 0.01;
			if(ViewScale < 0.001)
				ViewScale = 0.001;

			Map.Invalidate();
		}

        protected void SetCourse()
        {
            if (Connection == null || Connection.PlayerShip == null)
                return;

            Connection.PlayerShip.SetCourseManual(ShipEngineSpeed, ShipNavDirection);
        }

        protected double ShipSpeed = 0;
        protected double ShipDirection = 0;

        protected double ShipEngineSpeed = 0;
        protected double ShipNavDirection = 0;

        private void UpdateStatusMarker()
        {
            this.StatusLabel.Text = String.Format("Speed {0} Heading {1}", ShipSpeed, ShipDirection);
        }

        private void Forward_Click(object sender, EventArgs e)
        {
            ShipEngineSpeed += 1;
            SetCourse();
        }

        private void Backwards_Click(object sender, EventArgs e)
        {
            ShipEngineSpeed -= 1;
            SetCourse();
        }

        private void Left_Click(object sender, EventArgs e)
        {
            ShipNavDirection -= 3;
            SetCourse();
        }

        private void Right_Click(object sender, EventArgs e)
        {
            ShipNavDirection += 3;
            SetCourse();
        }


    }
}
