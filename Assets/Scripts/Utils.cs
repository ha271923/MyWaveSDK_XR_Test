using UnityEngine;

namespace Hubble.Launcher.Utils
{
	public class GameObjectUtils
	{
		static public string getParentGOname(GameObject currentGO) {
			if (currentGO.gameObject.transform.parent != null)
			{
				var parentGameObject = currentGO.gameObject.transform.parent.gameObject;
				if (parentGameObject != null)
				{
					return parentGameObject.transform.name;
				}
			}
			return null;
		}
	}
}