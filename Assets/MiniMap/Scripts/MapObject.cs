// ISTA 425 / INFO 525 Algorithms for Games
//
// Sample code file

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObject : MonoBehaviour 
{
	MiniMapEntity linkedMiniMapEntity;
	MiniMapController mmc;
	GameObject owner;
	Camera mapCamera;
	Image spr;
	GameObject panelGO;

	Vector2 screenPos;
	RectTransform sprRect;
	RectTransform rt;

	Transform miniMapTarget;

	void FixedUpdate () 
	{
		if (owner == null)
			Destroy (this.gameObject);
		else
			SetTransform ();
	}

	public void SetMiniMapEntityValues(MiniMapController controller, MiniMapEntity mme, 
									   GameObject attachedGO, Camera renderCamera, GameObject parentPanelGO)
	{
		linkedMiniMapEntity = mme;
		owner = attachedGO;
		mapCamera = renderCamera;
		panelGO = parentPanelGO;
		spr = gameObject.GetComponent<Image> ();
		spr.sprite = mme.icon;
		sprRect = spr.gameObject.GetComponent<RectTransform> ();
		sprRect.sizeDelta = mme.size;
		rt = panelGO.GetComponent<RectTransform> ();
		mmc = controller;
		miniMapTarget = mmc.target;
		SetTransform ();
	}

	// TODO: Implement transformation of registered map icons in MiniMap space
	void SetTransform()
	{
		if (owner == null || sprRect == null || rt == null || linkedMiniMapEntity == null) return;
    
		transform.SetParent(panelGO.transform, false);
    
		Vector3 worldPosition = owner.transform.position;
		Vector3 screenPosition = mapCamera.WorldToScreenPoint(worldPosition);
    
		Vector2 canvasPosition;
		bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rt, screenPosition, null, out canvasPosition
		);
    
		if (success)
		{
			sprRect.anchoredPosition = canvasPosition;
			bool isNPC = owner.name.Contains("NPC") || owner.name.Contains("npc") || 
			             owner.name.Contains("Npc") || owner.name.Contains("Red");
		
			if (isNPC && linkedMiniMapEntity.rotateWithObject)
			{
			
				Vector3 forward = owner.transform.forward;
				float rawAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
				float iconAngle = rawAngle + 90f;
				if (iconAngle < 0) iconAngle += 360f;
				if (iconAngle >= 360f) iconAngle -= 360f;
			
				sprRect.localEulerAngles = new Vector3(0, 0, iconAngle);
			}
			else if (linkedMiniMapEntity.rotateWithObject)
			{
				Vector3 forward = owner.transform.forward;
				float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
				sprRect.localEulerAngles = new Vector3(0, 0, angle);
			}
			else
			{
				sprRect.localEulerAngles = Vector3.zero;
			}
		}
	}
}
