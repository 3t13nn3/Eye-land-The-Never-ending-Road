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

				Vector2 roundedSampleInput =
					new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
				Debug.Log("x (in px): " + roundedSampleInput.x);
				Debug.Log("y (in px): " + roundedSampleInput.y);
				GetComponent<Image>().transform.position = new Vector3(gazePosition.x, gazePosition.y, 0);
			}
		}
	}
}
