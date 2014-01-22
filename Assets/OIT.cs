using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class OIT : MonoBehaviour {
	private Camera _opaqueCamera;
	private Camera _accumCamera;
	private Camera _revealageCamera;
	
	private GameObject _opaqueCameraGO;
	private GameObject _accumCameraGO;
	private GameObject _revealageCameraGO;
	
	void OnEnable() {
		_opaqueCameraGO = new GameObject();
		_opaqueCameraGO.transform.parent = transform;
		_opaqueCamera = _opaqueCameraGO.AddComponent<Camera>();
		_opaqueCamera.CopyFrom(camera);
		_opaqueCamera.enabled = true;
		_opaqueCamera.depth = camera.depth - 3;
		_opaqueCamera.backgroundColor = new Color(0f, 0f, 0f, 1f);
		_opaqueCamera.clearFlags = CameraClearFlags.SolidColor;
		_opaqueCamera.cullingMask = 1 << LayerMask.NameToLayer("Default");

		_accumCameraGO = new GameObject();
		_accumCameraGO.transform.parent = transform;
		_accumCamera = _accumCameraGO.AddComponent<Camera>();
		_accumCamera.CopyFrom(camera);
		_accumCamera.enabled = true;
		_accumCamera.depth = camera.depth - 2;
		_accumCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		_accumCamera.clearFlags = CameraClearFlags.Color;
		_accumCamera.cullingMask = 1 << LayerMask.NameToLayer("TransparentFX");

		_revealageCameraGO = new GameObject();
		_revealageCameraGO.transform.parent = transform;
		_revealageCamera = _revealageCameraGO.AddComponent<Camera>();
		_revealageCamera.CopyFrom(camera);
		_revealageCamera.enabled = true;
		_revealageCamera.depth = camera.depth - 1;
		_revealageCamera.backgroundColor = new Color(1f, 1f, 1f, 1f);
		_revealageCamera.clearFlags = CameraClearFlags.Color;
		_revealageCamera.cullingMask = 1 << LayerMask.NameToLayer("TransparentFX");
	}

	void OnDisable() {
		Destroy(_opaqueCameraGO);
		Destroy(_accumCameraGO);
		Destroy(_revealageCameraGO);
	}
}
