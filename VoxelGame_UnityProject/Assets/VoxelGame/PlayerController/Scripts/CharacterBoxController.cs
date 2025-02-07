using System;
using UnityEngine;

namespace VoxelGame.Player
{
	public class CharacterBoxController : MonoBehaviour
	{
		public Vector3 Size = new Vector3(1, 1, 1);
		public Vector3 Center = new Vector3(0, 0, 0);

		[NonSerialized] private Vector3 lastPosition;
		[NonSerialized] private LayerMask collisionLayers;

		public Vector3 velocity { get; private set; }
		public bool isGrounded { get; private set; }
		public bool hitHead { get; private set; }

		private void Start()
		{
			lastPosition = transform.position;

			UpdateCollisionLayers();
		}

		/// <summary>
		/// Update the collision layers based on the configured layer collision matrix.
		/// </summary>
		public void UpdateCollisionLayers()
		{
			collisionLayers = 0;
			for (int j = 0; j < 32; j++)
			{
				if (!Physics.GetIgnoreLayerCollision(gameObject.layer, j))
				{
					collisionLayers |= 1 << j;
				}
			}
		}

		public void Move(Vector3 motion)
		{
			if (motion == Vector3.zero)
			{
				velocity = Vector3.zero;
				return;
			}

			Vector3 pos = transform.position + Center;
			Vector3 size = Size / 2;

			isGrounded = false;
			hitHead = false;

			// Check each axis separately
			if (motion.x != 0)
			{
				if (Physics.BoxCast(pos, new Vector3(0, size.y, size.z), Vector3.right * Mathf.Sign(motion.x), out RaycastHit hit, Quaternion.identity, Mathf.Abs(motion.x) + size.x, collisionLayers))
				{
					motion.x = Mathf.Sign(motion.x) * (hit.distance - size.x);
				}
			}

			if (motion.y != 0)
			{
				if (Physics.BoxCast(pos, new Vector3(size.x, 0, size.z), Vector3.up * Mathf.Sign(motion.y), out RaycastHit hit, Quaternion.identity, Mathf.Abs(motion.y) + size.y, collisionLayers))
				{
					isGrounded = motion.y < 0;
					hitHead = !isGrounded;
					motion.y = Mathf.Sign(motion.y) * (hit.distance - size.y);
				}
			}

			if (motion.z != 0)
			{
				if (Physics.BoxCast(pos, new Vector3(size.x, size.y, 0), Vector3.forward * Mathf.Sign(motion.z), out RaycastHit hit, Quaternion.identity, Mathf.Abs(motion.z) + size.z, collisionLayers))
				{
					motion.z = Mathf.Sign(motion.z) * (hit.distance - size.z);
				}
			}

			// Apply the adjusted movement
			transform.position += motion;

			// Calculate velocity
			velocity = (transform.position - lastPosition) / Time.deltaTime;

			// Store the new position for the next frame
			lastPosition = transform.position;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Draw the collider in the editor.
		/// </summary>
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(transform.position + Center, Size);
		}
#endif
	}
}