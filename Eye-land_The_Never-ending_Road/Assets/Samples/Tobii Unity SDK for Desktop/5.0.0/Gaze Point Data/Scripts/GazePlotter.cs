//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Tobii.Gaming.Examples.GazePointData
{
	public class GazePlotter : MonoBehaviour
	{
		void Start()
		{

		}

		void Update()
		{
			GazePoint gazePoint = TobiiAPI.GetGazePoint();
			if (gazePoint.IsValid)
			{
				Vector2 gazePosition = gazePoint.Screen;
				GetComponent<Image>().transform.position = new Vector3(gazePosition.x, gazePosition.y, 0);
			}
		}

		Vector2 GetViewPoint() {
			GazePoint gazePoint = TobiiAPI.GetGazePoint();
			return gazePoint.Screen;
		}
	}
}
