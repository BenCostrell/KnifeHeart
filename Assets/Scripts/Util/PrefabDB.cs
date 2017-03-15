using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject {
	[SerializeField]
	private GameObject player;
	public GameObject Player { get { return player; } }

	[SerializeField]
	private GameObject fireball;
	public GameObject Fireball { get { return fireball; } }

	[SerializeField]
	private GameObject sing;
	public GameObject Sing { get { return sing; } }

	[SerializeField]
	private GameObject shield;
	public GameObject Shield { get { return shield; } }

	[SerializeField]
	private GameObject lunge;
	public GameObject Lunge { get { return lunge; } }

	[SerializeField]
	private GameObject wallop;
	public GameObject Wallop { get { return wallop; } }

	[SerializeField]
	private GameObject pull;
	public GameObject Pull { get { return pull; } }

}
