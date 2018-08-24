using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Choice
{
	public class ChoiceController
	{
		private List<ChoiceButton> options = new List<ChoiceButton>();

		public void Destroy()
		{
			foreach (ChoiceButton option in options)
			{
				UnityEngine.Object.Destroy(option.gameObject);
			}
		}

		private void FinishChoice()
		{
			foreach (ChoiceButton option in options)
			{
				option.DisableButton();
			}
			GameSystem.Instance.LeaveChoices();
		}

		public void Create(List<string> optstrings, int count)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("PrimaryUIPanel");
			Mathf.RoundToInt(120f / (float)(count - 1));
			int num = 0;
			while (true)
			{
				if (num >= count)
				{
					return;
				}
				int id = num;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(Resources.Load("ChoiceButton")) as GameObject;
				if (gameObject2 == null)
				{
					break;
				}
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localScale = Vector3.one;
				if (count > 8)
				{
					float x = (num == count - 1 && count % 2 == 1) ? (-150f) : ((num % 2 != 0) ? 150f : (-450f));
					gameObject2.transform.localPosition = new Vector3(x, (float)(-75 * (num / 2) + 27 * count - 50), 0f);
				}
				else
				{
					gameObject2.transform.localPosition = new Vector3(-150f, (float)(-75 * num + 27 * count + 50), 0f);
				}
				ChoiceButton component = gameObject2.GetComponent<ChoiceButton>();
				component.ChangeText(optstrings[num]);
				component.SetCallback(this, delegate
				{
					GameSystem.Instance.ScriptSystem.SetFlag("SelectResult", id);
					Debug.Log("ID: " + id);
					FinishChoice();
				});
				options.Add(gameObject2.GetComponent<ChoiceButton>());
				num++;
			}
			throw new Exception("Failed to instantiate ChoiceButton!");
		}
	}
}
