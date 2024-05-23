using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fabius
{
	[System.Serializable]
	public class Screen
	{
		public string name;
		//public List<GameObject> screenElements;
		public GameObject UIParent;
		public enum PauseStateTransition { None, Pause, Unpause};
		public PauseStateTransition pauseStateTransition = PauseStateTransition.None;
		public enum BuildPlatform { None, Editor, Browser, Computer, Mobile}
		public List<GameObjectPlatformPair> hideObjectsOnPlatform = new List<GameObjectPlatformPair>();
		public List<Screen> subscreens;

		Screen activeSubscreen;

		public void SetActive(bool state, BuildPlatform simulatePlatform = BuildPlatform.None)
		{
			if (Application.isPlaying && state && pauseStateTransition == PauseStateTransition.Pause)
            {
				PauseManager.instance.Pause();
            }
			else if (Application.isPlaying && state && pauseStateTransition == PauseStateTransition.Unpause)
            {
				PauseManager.instance.Unpause();
            }

			UIParent.SetActive(state);

            if (Application.isPlaying)
            {
				foreach (GameObjectPlatformPair pair in hideObjectsOnPlatform)
				{
					if (isCurrentBuildPlatform(pair.platform, simulatePlatform))
					{
						pair.gameObject.SetActive(false);
					}
					else
					{
						pair.gameObject.SetActive(true);
					}
				}
			}

			if (activeSubscreen == null && subscreens.Count > 0)
			{
				SwitchSubscreen(subscreens[0]);
			}
		}

		private bool isCurrentBuildPlatform (BuildPlatform platform, BuildPlatform simulatePlatform)
        {
			return (platform == BuildPlatform.Editor && Application.isEditor && (simulatePlatform == BuildPlatform.None || simulatePlatform == BuildPlatform.Editor))
				|| (platform == BuildPlatform.Browser && ((Application.isEditor && simulatePlatform == BuildPlatform.Browser) || Application.platform == RuntimePlatform.WebGLPlayer))
				|| (platform == BuildPlatform.Computer && ((Application.isEditor && simulatePlatform == BuildPlatform.Computer) || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer))
				|| (platform == BuildPlatform.Mobile && ((Application.isEditor && simulatePlatform == BuildPlatform.Mobile) || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android));
        }

		void SwitchSubscreen(Screen subscreen)
		{
			if (activeSubscreen != null)
				activeSubscreen.SetActive(false);

			if (subscreen != null)
				subscreen.SetActive(true);

			activeSubscreen = subscreen;
		}

		public void SwitchSubscreen(int screenNumber)
		{
			if (screenNumber < subscreens.Count && screenNumber >= 0)
			{
				SwitchSubscreen(subscreens[screenNumber]);
			}
			else
			{
				Debug.LogError("No screen exists with index: " + screenNumber);
			}
		}

		public void SwapSubscreen()
		{
			if (activeSubscreen == subscreens[0])
			{
				if (subscreens.Count > 1)
				{
					SwitchSubscreen(subscreens[1]);
				}
			}
			else
			{
				if (subscreens.Count > 0)
				{
					SwitchSubscreen(subscreens[0]);
				}
			}
		}

		[System.Serializable]
		public class GameObjectPlatformPair
        {
			public GameObject gameObject;
			public BuildPlatform platform;
        }
	}
}
