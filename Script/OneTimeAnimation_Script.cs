﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeAnimation_Script : MonoBehaviour {
	private Animator animator;
	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			Destroy(gameObject);
		}
	}
}
