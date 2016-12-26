using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipConnector : MonoBehaviour
{
	public InputField AddressField = null;


	public static string SelectedHost = string.Empty;

	// Use this for initialization
	void Start ()
	{
		if(AddressField != null)
			AddressField.text = "127.0.0.1";
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void ConnectClicked()
	{
		if(AddressField == null)
			return;

		SelectedHost = AddressField.text;

		// connect shit and load basic sceene

		Debug.Log("Connecting to address " + SelectedHost);
	}
}
