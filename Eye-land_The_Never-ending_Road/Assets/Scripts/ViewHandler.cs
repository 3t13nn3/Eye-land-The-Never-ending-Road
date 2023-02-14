//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Tobii.Gaming
{
	public class ViewHandler : MonoBehaviour
	{	
		private GazePoint gazePoint;

		private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

		void Start()
		{
			
		}

		void Update()
		{
			gazePoint = TobiiAPI.GetGazePoint();
			if (gazePoint.IsValid)
			{
				Vector3 gazePosition = new Vector3(gazePoint.Screen.x, gazePoint.Screen.y, 0f);
				lastPosition = Vector3.Lerp(lastPosition, gazePosition, 1f);
				GetComponent<Image>().transform.position = lastPosition;
			}
		}

		public Vector2 GetViewCoord() {
			return lastPosition;
		}
	}
}
