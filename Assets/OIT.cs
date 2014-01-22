using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class OIT : MonoBehaviour {
	public Shader accumShader;
	public Shader revealageShader;
	public Shader postEffectShader;

	private Camera _renderCam;
	private GameObject _renderGO;

	private int _layerOpaque;
	private int _layerTransparent;

	private RenderTexture _opaqueTex;
	private RenderTexture _accumTex;
	private RenderTexture _revealageTex;

	void OnEnable() {
		_renderGO = new GameObject();
		_renderGO.transform.parent = transform;
		_renderCam = _renderGO.AddComponent<Camera>();
		_renderCam.CopyFrom(camera);
		_renderCam.enabled = false;
		_renderCam.clearFlags = CameraClearFlags.Nothing;
		_layerOpaque = LayerMask.NameToLayer("Default");
		_layerTransparent = LayerMask.NameToLayer("TransparentFX");
	}

	void OnPreRender() {
		var width = Screen.width;
		var height = Screen.height;
		_opaqueTex = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
		_accumTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);
		_revealageTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf);

		Graphics.SetRenderTarget(_opaqueTex);
		GL.Clear(true, true, Color.black);
		_renderCam.cullingMask = 1 << _layerOpaque;
		_renderCam.Render();

		Graphics.SetRenderTarget(_accumTex.colorBuffer, _opaqueTex.depthBuffer);
		GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
		_renderCam.cullingMask = 1 << _layerTransparent;
		_renderCam.RenderWithShader(accumShader, null);

		Graphics.SetRenderTarget(_revealageTex.colorBuffer, _opaqueTex.depthBuffer);
		GL.Clear(false, true, new Color(1f, 1f, 1f, 1f));
		_renderCam.cullingMask = 1 << _layerTransparent;
		_renderCam.RenderWithShader(revealageShader, null);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		
	}

	void OnDisable() {
		Destroy(_renderGO);
	}
}
