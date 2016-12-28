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


    public GameObject DefaultObject = null;

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

        foreach (UserShip.ShipCentricSensorEntity sre in Connection.PlayerShip.KnownEntities.Values)
            UpdateRealtiveEntities(sre);

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
        GameObject obj = e.Tag as GameObject;

        if (obj == null)
            PlayerShip_SensorEntityAppeared(sender, e);
        else
        {
            if (e.Tag != e.BaseEntity.Tag)
            {
                GameObject.Destroy(obj);
                PlayerShip_SensorEntityAppeared(sender, e);
            }
            else
            {
                UpdateRealtiveEntities(e as UserShip.ShipCentricSensorEntity);
            }
        }
    }

    private void UpdateRealtiveEntities(UserShip.ShipCentricSensorEntity sre)
    {
        GameObject obj = sre.Tag as GameObject;
        if (obj == null)
            return;

        obj.transform.position = new Vector3(sre.ShipRelativePosition.X, sre.ShipRelativePosition.Y, sre.ShipRelativePosition.Z);
        obj.transform.rotation = new Quaternion((float)sre.BaseEntity.Orientation.XYZ.X, (float)sre.BaseEntity.Orientation.XYZ.Y, (float)sre.BaseEntity.Orientation.XYZ.Z, (float)sre.BaseEntity.Orientation.W);
    }

    private void PlayerShip_SensorEntityAppeared(object sender, Entities.Classes.Ship.KnownEntity e)
	{
		e.Tag = null;
		e.BaseEntity.Tag = null;

        GameObject obj = CreateObjectForEntity(e.BaseEntity.VisualGraphics);
        if (obj == null)
        {
            Debug.Log("Unable to create " + e.BaseEntity.VisualGraphics + ", no linked object");
            return;
        }

        UserShip.ShipCentricSensorEntity sre = e as UserShip.ShipCentricSensorEntity;

        e.Tag = obj;
        e.BaseEntity.Tag = obj;
        UpdateRealtiveEntities(sre);
    }

    GameObject CreateObjectForEntity(string graphicType)
    {
        foreach(var t in ObjectGraphics)
        {
            if (t.ClassName == graphicType)
            {
                return GameObject.Instantiate(t.GraphicObject);
            }
        }
        return DefaultObject;
    }
}
