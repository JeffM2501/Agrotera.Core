using System;
using System.Collections.Generic;
using UnityEngine;

using ShipClient;

public class HostConnection : MonoBehaviour
{
	ShipClient.ShipConnection Connection = null;

	[System.Serializable]
	public class ObjectGracphicsDef
	{
		public string ClassName = string.Empty;
		public GameObject GraphicObject = null;
	}
	public List<ObjectGracphicsDef> ObjectGraphics = new List<ObjectGracphicsDef>();

	// Use this for initialization
	void Start ()
	{
		Connection = new ShipConnection(ShipConnector.SelectedHost, 2501);
		Connection.Connected += Connection_Connected;
		Connection.Disconnected += Connection_Disconnected;
		Connection.ShipAssigned += Connection_ShipAssigned;
	}

	private void Connection_ShipAssigned(object sender, System.EventArgs e)
	{
		StartupSceene();
	}

	private void Connection_Disconnected(object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException();
	}

	private void Connection_Connected(object sender, ShipConnection.ConnectedEventArgs e)
	{
		e.DesiredShipArguments.Add("Default");
	}

	// Update is called once per frame
	void Update ()
	{
		Connection.Update();
	}

	void StartupSceene()
	{
		Connection.PlayerShip.SensorEntityAppeared += PlayerShip_SensorEntityAppeared;
		Connection.PlayerShip.SensorEntityUpdated += PlayerShip_SensorEntityUpdated;
		Connection.PlayerShip.SensorEntityRemoved += PlayerShip_SensorEntityRemoved;
	}

	private void PlayerShip_SensorEntityRemoved(object sender, Entities.Classes.Ship.KnownEntity e)
	{
	
	}

	private void PlayerShip_SensorEntityUpdated(object sender, Entities.Classes.Ship.KnownEntity e)
	{
		
	}

	private void PlayerShip_SensorEntityAppeared(object sender, Entities.Classes.Ship.KnownEntity e)
	{
		e.Tag = null;
		e.BaseEntity.Tag = null;
	}
}
