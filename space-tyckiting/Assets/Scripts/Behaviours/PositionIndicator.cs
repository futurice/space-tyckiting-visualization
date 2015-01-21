using UnityEngine;
using System.Collections;

namespace SpaceTyckiting
{
	[RequireComponent(typeof(LineRenderer))]
	public class PositionIndicator : MonoBehaviour
	{
		private Transform tr;
		private LineRenderer line;

		void Awake()
		{
			tr = GetComponent<Transform>();
			line = GetComponent<LineRenderer>();
		}

		void Update()
		{
			line.SetPosition(0, tr.position);
			line.SetPosition(1, new Vector3(tr.position.x, 0, tr.position.z));
		}
	}
}