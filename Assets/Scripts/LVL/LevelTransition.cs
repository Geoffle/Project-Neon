using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
	public void LoadLevel(int saveSlot = 1)
	{
		switch (saveSlot)
		{
			case 1:
				EnterLVL(1);
				break;
			case 2:
				DataSaved();
				EnterLVL(1);
				break;
		}
	}

	public void DataSaved()
	{
        DataSave.nextLevel = 5;
        DataSave.collected.Add("AudioMessage (T1)");
		DataSave.levelBeaten.Add(2);
		DataSave.levelBeaten.Add(3);
		DataSave.levelBeaten.Add(4);
		DataSave.levelBeaten.Add(5);
	}

	public static void EnterLVL(int enter)
	{
		DataSave.help = false;
		DataSave.lastWaypoint = 0;
		DataSave.lastCheckpoint = new Vector3(0, 0, 0);

		SceneManager.LoadScene(enter, LoadSceneMode.Single);
	}

	public static void ExitLVL(int exit)
	{
		if (exit == 6)
		{
			DataSave.levelBeaten.Add(exit);
			DataSave.nextLevel = 0;
			DataSave.help = false;
			DataSave.lastCheckpoint = new Vector3(0, 0, 0);

			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
		else
		{
			DataSave.levelBeaten.Add(exit);
			DataSave.nextLevel = exit;
			DataSave.lastWaypoint = 0;
			DataSave.help = false;
			DataSave.lastCheckpoint = new Vector3(0, 0, 0);

			SceneManager.LoadScene("Hub", LoadSceneMode.Single);
		}
	}
}
