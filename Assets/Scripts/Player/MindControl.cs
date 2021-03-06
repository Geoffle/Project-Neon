﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class MindControl : MonoBehaviour
{
	// Components
	private GameObject player;
	private Transform cameraPos;
	private Collider attackTrigger;
	private NavMeshAgent navMeshAgent;
	private Animator anim;
	private GameObject UI;

	// Scripts
	private EnemyAI GetEnemyAI;
	private ThirdPersonCharacter GetCharacter;
	private ThirdPersonUserControl GetThirdPersonUserControl;
	private PlayerCameraController GetPlayerCameraController;
	private Dialogs GetDialogs;

	// Assets
	public RuntimeAnimatorController animatorController;
	public RuntimeAnimatorController animatorControllerMindControl;

	// Variables
	private Coroutine coroutineMind;
	public bool mindControl;
	private bool abilityCD;

	void Start()
	{
		UI = GameObject.Find("UI");

		player = GameObject.Find("Player");
		cameraPos = transform.Find("CameraPosMindControl");
		attackTrigger = transform.Find("AttackTrigger").GetComponent<Collider>();
		navMeshAgent = transform.GetComponent<NavMeshAgent>();
		anim = transform.GetComponent<Animator>();

		anim.runtimeAnimatorController = animatorController;

		GetEnemyAI = transform.GetComponent<EnemyAI>();
		GetCharacter = transform.GetComponent<ThirdPersonCharacter>();
		GetThirdPersonUserControl = transform.GetComponent<ThirdPersonUserControl>();
		GetPlayerCameraController = Camera.main.GetComponent<PlayerCameraController>();
		GetDialogs = GameObject.Find("Audio/Dialogs").GetComponent<Dialogs>();
	}

	void Update()
	{
		if (mindControl)
		{
			Mindcontrol(true);

			if (Dialogs.mindControl) // Tut
			{
				Dialogs.mindControl = false;
				GetDialogs.PlayDialog("T-5-9");
			}

			if (Input.GetButtonDown("Interact") && !abilityCD) // Interact
			{
				Interact();
			}

			if (Input.GetButtonDown("Mind-Control") && !abilityCD) // Interact
			{
				Mindcontrol(false, true);
			}
		}
	}

	void Interact()
	{
		anim.SetTrigger("Interact");

		StartCoroutine(InteractTimer(0.2f));
		StartCoroutine(AbilityCooldown(1f));
	}

	public void Mindcontrol(bool yes, bool stunned = false, bool combat = false)
	{
		if (yes)
		{
			anim.runtimeAnimatorController = animatorControllerMindControl; // Change animations
			GetPlayerCameraController.lookAt = cameraPos; // Change camera position         
			GetEnemyAI.WeaponDraw(true);

			player.GetComponent<PlayerAbility>().enabled = false;
			player.GetComponent<ThirdPersonCharacter>().enabled = false;
			player.GetComponent<ThirdPersonUserControl>().enabled = false;

			navMeshAgent.enabled = false;
			GetCharacter.enabled = true;
			GetThirdPersonUserControl.enabled = true;

			UI.transform.Find("Overlay/Timer").gameObject.SetActive(true);
			Timer.timerStart = true;

			if (coroutineMind == null)
			{
				coroutineMind = StartCoroutine(MindControlTimer(10f));
			}
		}
		else if (!yes)
		{
			mindControl = false;
			AttackTrigger.mindControl = false;

			anim.runtimeAnimatorController = animatorController;
			GetPlayerCameraController.lookAt = player.transform.Find("CameraPos");

			player.GetComponent<PlayerAbility>().enabled = true;
			player.GetComponent<ThirdPersonCharacter>().enabled = true;
			player.GetComponent<ThirdPersonUserControl>().enabled = true;

			navMeshAgent.enabled = true;
			GetCharacter.enabled = false;
			GetThirdPersonUserControl.enabled = false;

			if (stunned)
			{
				StartCoroutine(GetEnemyAI.PatrolTimer(5f, true));
			}
			else if (combat)
			{
				GetEnemyAI.combatStart = true;
			}
			else if (!GetEnemyAI.distracted)
			{
				GetEnemyAI.BacktoPatrol();
			}

			Timer.timerStart = false;
			UI.transform.Find("Overlay/Timer").gameObject.SetActive(false);

			coroutineMind = null;
		}
	}

	IEnumerator InteractTimer(float time)
	{
		attackTrigger.enabled = true;

		yield return new WaitForSeconds(time);

		attackTrigger.enabled = false;
	}

	IEnumerator AbilityCooldown(float time)
	{
		abilityCD = true;

		yield return new WaitForSeconds(time);

		abilityCD = false;
	}

	public IEnumerator MindControlTimer(float time) // Time Controlled by attackTrigger
	{
		yield return new WaitForSeconds(time);

		mindControl = false;

		Mindcontrol(false);
	}
}
