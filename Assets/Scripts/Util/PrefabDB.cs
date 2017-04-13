using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject {
	[SerializeField]
	private GameObject player;
	public GameObject Player { get { return player; } }

	[SerializeField]
	private GameObject basicAttack;
	public GameObject BasicAttack { get { return basicAttack; } }

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

    [SerializeField]
    private GameObject blink;
    public GameObject Blink { get { return blink; } }

    [SerializeField]
    private GameObject[] scenes;
    public GameObject[] Scenes { get { return scenes; } }

    [SerializeField]
    private GameObject genericImage;
    public GameObject GenericImage { get { return genericImage; } }

	public GameObject GetPrefabFromAbilityType(Ability.Type type){
		switch (type) {
		case Ability.Type.BasicAttack:
			return basicAttack;
		case Ability.Type.Fireball:
			return fireball;
		case Ability.Type.Lunge:
			return lunge;
		case Ability.Type.Pull:
			return pull;
		case Ability.Type.Shield:
			return shield;
		case Ability.Type.Sing:
			return sing;
		case Ability.Type.Wallop:
			return wallop;
        case Ability.Type.Blink:
            return blink;
		default:
			return null;
		}
	}

}
