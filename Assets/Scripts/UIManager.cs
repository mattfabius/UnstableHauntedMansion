using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Fabius
{
	[ExecuteInEditMode]
	public class UIManager : MonoBehaviour
	{
		[HideInInspector]
		public int startScreen = 0;
		[HideInInspector]
		public int currentScreenIndex;

		public Screen.BuildPlatform simulatePlatform = Screen.BuildPlatform.Editor;
		public List<Screen> screens;
		
		Screen activeScreen;

		void Start()
		{
			SetStartScreen();
		}

        #region Screen Management Methods
        public void SetStartScreen()
		{
			if (screens.Count <= 0)
			{
				Debug.LogError("Error: No screens found!");
				return;
			}
			else if (screens.Count < startScreen + 1)
			{
				Debug.LogWarning("Error: Start screen not found! Defaulting to screen 0.");
				startScreen = 0;
			}

			foreach (Screen screen in screens)
			{
				screen.SetActive(false, simulatePlatform);
			}
			screens[startScreen].SetActive(true, simulatePlatform);
			activeScreen = screens[startScreen];
			currentScreenIndex = startScreen;
		}

		void SwitchScreen(Screen nextScreen)
		{
			if (nextScreen != null)
			{
				nextScreen.SetActive(true, simulatePlatform);
			}
            else
            {
				Debug.LogError("Given Screen does not exist");
				return;
			}

			if (activeScreen != null)
			{
				activeScreen.SetActive(false, simulatePlatform);
			}

			activeScreen = nextScreen;

#if UNITY_EDITOR
			currentScreenIndex = screens.IndexOf(activeScreen);
#endif
		}

		public void SwitchScreen(string screenName)
		{
			foreach (Screen screen in screens)
			{
				if (screen.name == screenName)
				{
					SwitchScreen(screen);
					return;
				}
			}
			Debug.LogError("No screen exists with name: " + screenName);
		}

		public void SwitchScreen(int screenNumber)
		{
			if (screenNumber < screens.Count && screenNumber >= 0)
			{
				SwitchScreen(screens[screenNumber]);
			}
			else
			{
				Debug.LogError("No screen exists with index: " + screenNumber);
			}
		}

		public void SwitchSubscreen(int subscreenNumber)
		{
			activeScreen.SwitchSubscreen(subscreenNumber);
		}

		public void SwapSubscreen()
		{
			activeScreen.SwapSubscreen();
		}
        #endregion

        #region Menu Functions
        public void Quit()
        {
			Application.Quit();
        }

		public void LoadScene(string sceneName)
        {
			SceneManager.LoadScene(sceneName);
        }
        #endregion
    }

}