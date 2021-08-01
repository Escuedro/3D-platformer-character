using System;
using UnityEngine;

public class SimpleFollowToTarget : MonoBehaviour
{
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private Vector3 _offset;

	private void LateUpdate()
	{
		transform.position = _target.position + _offset;
	}
}