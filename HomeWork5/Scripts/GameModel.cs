﻿using UnityEngine;
using System.Collections;
using Com.Factory;
using System.Collections.Generic;
public class GameModel : MonoBehaviour
{
	public float countDown = 2f; // 飞碟发射倒计时总时间
	public float timeToEmit;     // 飞碟发射倒计时剩余时间
	private bool counting;		 // 正在倒计时
	private bool shooting;		 // 正在射击
	public bool isCounting(){    // 倒计时是否结束
		return counting;
	}
	public bool isShooting(){	 // 是否正在射击
		return shooting;
	}
	private List<GameObject> disks = new List<GameObject> ();  // 飞碟对象列表
	private List<int> idList = new List<int>();                // 飞碟id列表
	private int diskScale;                  				   // 飞碟大小  
	private Color diskColor;                				   // 飞碟颜色  
	private Vector3 emitPosition;         			   		   // 发射位置  
	private Vector3 emitDirection;          				   // 发射方向  
	private float emitSpeed;                				   // 发射速度  
	private int emitNumber;                 				   // 发射数量  
	private bool emitEnable;                				   // 允许新的发射事件 
	private bool state = false;
	private SceneController scene;

	void Awake(){
		scene = SceneController.getInstance ();
		scene.setGameModel (this);

	}
	private float acceleration;  //acceleration是重力加速度，为9.8 
	private float horizontalSpeed;  //horizontalSpeed是飞碟水平方向的速度
	private Vector3 direction;  //direction是飞碟的初始飞行方向  
	private float time;    // time是飞碟已经飞行的时间 
	private float verticalSpeed = 20f;
	private float[] array = new float[1000];
	//改变运动模式

	public void changeState(bool state){
		this.state = state;
	}
	public void setting(int scale, Color color, Vector3 emitPos, Vector3 emitDir, float speed, int num){
		// 对飞碟的各种参数进行设置
		diskScale = scale;
		diskColor = color;
		emitPosition = emitPos;
		emitDirection = emitDir;
		emitSpeed = speed;
		emitNumber = num;
		acceleration = 9.8f;  
		time = 0;  
		horizontalSpeed = emitSpeed;
		direction = emitDirection;
	}
	// prepare next emiting
	public void prepareToEmitDisk(){
		if (!counting && !shooting) {
			timeToEmit = countDown;
			emitEnable = true;
		}
	}
	void start(){
		for (int i = 0; i < array.Length; ++i) {
			array [i] = 20f;
		}
	}
	public void firstModelMove (){
		time += Time.deltaTime; //计算飞碟的累计飞行时间  

		for (int i = 0; i < disks.Count; ++i) {
			disks [i].GetComponent<Rigidbody> ().Sleep ();
			float deltaZ = 2 * horizontalSpeed * Time.deltaTime;
			float deltaX = 3f * Time.deltaTime;
			float deltaY = horizontalSpeed * Time.deltaTime * 3;
			disks [i].transform.position = disks [i].transform.position + new Vector3 (deltaX, deltaY, deltaZ);
		}
	}
	// 发射飞碟, 这里要根据模式的不同作出不同的响应
	void emitDisks(){
		if (state == true) {
			for (int i = 0; i < emitNumber; ++i) {  
				idList.Add (DiskFactory.getInstance ().getDisk ());  
				disks.Add (DiskFactory.getInstance ().getDiskObject (idList [i]));  
				disks [i].transform.localScale *= diskScale;  
				disks [i].GetComponent<Renderer> ().material.color = diskColor;
				disks [i].transform.position = new Vector3 (emitPosition.x, emitPosition.y + i, emitPosition.z);  
				disks [i].SetActive (true); 
				disks [i].GetComponent<Rigidbody> ().AddForce (emitDirection * Random.Range (emitSpeed * 5, emitSpeed * 10 / 10), ForceMode.Impulse);
			} 
		}
		if (state == false) {
			for (int i = 0; i < emitNumber; ++i) {
				idList.Add (DiskFactory.getInstance ().getDisk ());
				disks.Add (DiskFactory.getInstance ().getDiskObject (idList [i]));
				disks [i].transform.localScale *= diskScale;
				disks [i].GetComponent<Renderer> ().material.color = diskColor;
				disks [i].transform.position = new Vector3 (emitPosition.x, emitPosition.y + i, emitPosition.z);
				disks [i].SetActive (true);
				//disks[i].GetComponent<Rigidbody>().
			}
		}
	}
	// 回收飞碟
	void freeADisk(int i) {  
		DiskFactory.getInstance().free(idList[i]);  
		disks.RemoveAt(i);  
		idList.RemoveAt(i);  
	} 
	void FixedUpdate() {  
		if (timeToEmit > 0) {  
			counting = true;  
			timeToEmit -= Time.deltaTime;  
		} else {  
			counting = false;  
			if (emitEnable) {  
				emitDisks(); // 发射飞碟  
				emitEnable = false;  
				shooting = true;  
			}  
		}  
	}  
	
	// Update is called once per frame
	void Update () {  
		/*
		if (state == false) {
			time += Time.deltaTime; //计算飞碟的累计飞行时间  
			for(int i=0;i<disks.Count;++i){
				disks [i].transform.Translate (Vector3.down * acceleration * time * Time.deltaTime); //飞碟在竖直方向的运动 
				disks[i].transform.Translate(direction * horizontalSpeed * Time.deltaTime);  //飞碟在水平方向的运动 
			}
		}
		*/
		for (int i = 0; i < disks.Count; ++i) {  
			if (!disks[i].activeInHierarchy) {  // 飞碟不在场景中  
				scene.getJudge().scoreADisk();  // 得分  
				freeADisk(i);  
			}  
			else if (disks[i].transform.position.y < 0) {    // 飞碟在场景中但落地  
				scene.getJudge().failADisk();   // 失分  
				freeADisk(i);  
			}  
		}  
		if (disks.Count == 0) {  
			shooting = false;  
		}  
	}  
}  