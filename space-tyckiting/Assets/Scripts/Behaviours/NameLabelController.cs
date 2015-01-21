using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SpaceTyckiting
{
	[RequireComponent(typeof(Joint))]
	[RequireComponent(typeof(LineRenderer))]
	public class NameLabelController : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.UI.Text nameLabel;

		private UnitController target;

		private Transform textTarget;

		private LineRenderer line;

		private Transform tr;

		private Vector3 targetPos;

		private Collider col;

		private Transform targetModel;

		public void Init(UnitController target)
		{
			this.target = target;

			nameLabel.text = target.Data.name;
			
			var go = new GameObject("Label target for " + target.Data.name);
			textTarget = go.GetComponent<Transform>();
			var rb = go.AddComponent<Rigidbody>();
			rb.useGravity = false;
			rb.isKinematic = true;
			GetComponent<Joint>().connectedBody = rb;			
			
			var posAway = Settings.GetWorldCoordinate(target.PositionX, target.PositionY).normalized * 500;
			rb.position = new Vector3(
				posAway.x,
				100,
				posAway.z);

			var pos = Settings.GetWorldCoordinate(target.PositionX, target.PositionY);
			textTarget.position = new Vector3(
				pos.x,
				100,
				pos.z + 10);

			line = GetComponent<LineRenderer>();
			tr = GetComponent<Transform>();
			col = GetComponent<Collider>();
			col.enabled = false;
			Invoke("EnableCollider", 1);
		}

		void EnableCollider()
		{
			col.enabled = true;
		}

		void Update()
		{
			if (target != null)
			{
				SetTargetPosition();

				line.SetPosition(0, tr.position);
				line.SetPosition(1, targetPos);

				tr.position = new Vector3(
				tr.position.x,
				3,
				tr.position.z);
			}
			else
			{
				Destroy(gameObject);
				Destroy(textTarget.gameObject);
			}
		}

		void SetTargetPosition()
		{
			targetPos = Settings.GetWorldCoordinate(target.PositionX, target.PositionY);
			textTarget.position = Vector3.MoveTowards(
				textTarget.position,
				new Vector3(
					targetPos.x,
					100,
					targetPos.z + 10),
				Time.deltaTime * 10);
		}
	}
}