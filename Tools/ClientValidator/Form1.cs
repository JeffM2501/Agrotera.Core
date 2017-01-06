using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ShipClient;

namespace ClientValidator
{
    public partial class Form1 : Form
    {
        ShipConnection Connection = null;

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
                Connection.Update();
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
            if (Connection != null)
                Connection.Update();
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (Connection == null)
                return;

            if (Connection.PlayerShip == null)
                return;

            e.Graphics.TranslateTransform(Map.Width * 0.5f, Map.Height * 0.5f);

            double scale = 0.25;

            e.Graphics.DrawLine(Pens.DarkGreen, 0, Map.Height * 0.5f, 0, -Map.Height * 0.5f);
            e.Graphics.DrawLine(Pens.DarkGreen, Map.Width * 0.5f,0, -Map.Width * 0.5f, 0);

            foreach (var i in Connection.PlayerShip.KnownEntities.Values)
            {
                RectangleF rect = new RectangleF();


                float size = 5;
                Brush b = Brushes.LightYellow;
                Image pic = null;

                if (MapElementColors.ContainsKey(i.BaseEntity.VisualGraphics))
                {
                    var d = MapElementColors[i.BaseEntity.VisualGraphics];
                    if (d.Pic == null)
                        b = d.Color;
                    else
                        pic = d.Pic;

                    size = d.Size;
                }

                if (ViewType.SelectedIndex == 2)
                {
                    UserShip.ShipCentricSensorEntity scs = i as UserShip.ShipCentricSensorEntity;
                    rect.X = (float)(scs.ShipRelativePosition.X * scale);
                    rect.Y = (float)(scs.ShipRelativePosition.Y * scale);

//                     rect.X = (float)((i.LastPosition.X - Connection.PlayerShip.Position.X) * scale);
//                     rect.Y = (float)((i.LastPosition.Y - Connection.PlayerShip.Position.Y) * scale);
                }
                else if (ViewType.SelectedIndex == 1)
                {
                    rect.X = (float)(i.BaseEntity.Position.X * scale);
                    rect.Y = (float)(i.BaseEntity.Position.Y * scale);
                }
                else if (ViewType.SelectedIndex == 0)
                {
                    rect.X = (float)(i.LastPosition.X * scale);
                    rect.Y = (float)(i.LastPosition.Y * scale);
                }


                rect.X -= size;
                rect.Y -= size;

                rect.Width = size;
                rect.Height = size;

                if (pic != null)
                    e.Graphics.DrawImage(pic, rect);
                else
                    e.Graphics.FillRectangle(b, rect);
            }
        }

        private void ViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Map.Invalidate();
        }
    }
}
