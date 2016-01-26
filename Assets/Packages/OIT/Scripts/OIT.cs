using UnityEngine;
using System.Collections;

namespace OIT {

	[ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class OIT : MonoBehaviour {
		public const string KW_PREMULT_ON = "PREMULTIPLIED_ALPHA_ON";
		public const string KW_PREMULT_OFF = "PREMULTIPLIED_ALPHA_OFF";

		public enum TransparentModeEnum { PremultAlpha = 0, NotPremultAlpha }
		public enum UIModeEnum { Hidden = 0, Visible }

		public TransparentModeEnum transparentMode;
		public UIModeEnum uiMode;
		public KeyCode uikey = KeyCode.T;
    	public Shader accumShader;
    	public Shader revealageShader;
    	public Shader postEffectShader;
    	public bool	oitEnabled;
    	public bool weightEnabled;
		public float weight = -5f;
		public LayerMask _layerOpaque = 1 << 0;
		public LayerMask _layerTransparent = 1 << 1;

        Camera _attachedCam;
    	Camera _renderCam;
    	GameObject _renderGO;

    	RenderTexture _opaqueTex;
    	RenderTexture _accumTex;
    	RenderTexture _revealageTex;

    	Material _postEffectMat;

    	void Awake() {
            _attachedCam = GetComponent<Camera>();
			if (_renderGO == null) {
	    		_renderGO = new GameObject();
				_renderGO.hideFlags = HideFlags.DontSave;
				_renderGO.transform.SetParent(transform, false);
	    		_renderCam = _renderGO.AddComponent<Camera>();
	            _renderCam.CopyFrom(_attachedCam);
	    		_renderCam.enabled = false;
	    		_renderCam.clearFlags = CameraClearFlags.Nothing;
			}
			if (_postEffectMat == null) {
    			_postEffectMat = new Material(postEffectShader);
				_postEffectMat.hideFlags = HideFlags.DontSave;
			}
			weight = Mathf.Min(0f, weight);
			Shader.SetGlobalFloat("_Weight", (weightEnabled ? weight : 0f));
    	}
		void OnDestroy() {
			DestroyImmediate(_renderGO);
			DestroyImmediate(_postEffectMat);
		}

    	void OnPreRender() {
            var width = Screen.width;
            var height = Screen.height;
    		_opaqueTex = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
    		_accumTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            _revealageTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

			switch (transparentMode) {
			case TransparentModeEnum.NotPremultAlpha:
				Shader.DisableKeyword(KW_PREMULT_ON);
				Shader.EnableKeyword(KW_PREMULT_OFF);
				break;
			case TransparentModeEnum.PremultAlpha:
				Shader.DisableKeyword(KW_PREMULT_OFF);
				Shader.EnableKeyword(KW_PREMULT_ON);
				break;
			}

    		_renderCam.targetTexture = _opaqueTex;
            _renderCam.backgroundColor =_attachedCam.backgroundColor;
            _renderCam.clearFlags = _attachedCam.clearFlags;
    		_renderCam.cullingMask = _layerOpaque;
    		_renderCam.Render();

    		_renderCam.targetTexture = _accumTex;
    		_renderCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
    		_renderCam.clearFlags = CameraClearFlags.SolidColor;
    		_renderCam.cullingMask = 0;
    		_renderCam.Render();
    		_renderCam.SetTargetBuffers(_accumTex.colorBuffer, _opaqueTex.depthBuffer);
    		_renderCam.clearFlags = CameraClearFlags.Nothing;
    		_renderCam.cullingMask = _layerTransparent;
    		_renderCam.RenderWithShader(accumShader, null);

    		_renderCam.targetTexture = _revealageTex;
    		_renderCam.backgroundColor = new Color(1f, 1f, 1f, 1f);
    		_renderCam.clearFlags = CameraClearFlags.SolidColor;
    		_renderCam.cullingMask = 0;
    		_renderCam.Render();
    		_renderCam.SetTargetBuffers(_revealageTex.colorBuffer, _opaqueTex.depthBuffer);
    		_renderCam.clearFlags = CameraClearFlags.Nothing;
    		_renderCam.cullingMask = _layerTransparent;
    		_renderCam.RenderWithShader(revealageShader, null);
    	}
    	
    	void OnRenderImage(RenderTexture src, RenderTexture dst) {
    		if (!oitEnabled) {
    			Graphics.Blit(src, dst);
    			return;
    		}
    		_postEffectMat.SetTexture("_AccumTex", _accumTex);
    		_postEffectMat.SetTexture("_RevealageTex", _revealageTex);
    		Graphics.Blit(_opaqueTex, dst, _postEffectMat);
    	}

    	void OnPostRender() {
    		RenderTexture.ReleaseTemporary(_opaqueTex);
    		RenderTexture.ReleaseTemporary(_accumTex);
    		RenderTexture.ReleaseTemporary(_revealageTex);
    	}
		void Update() {
			if (Input.GetKeyDown(uikey)) {
				uiMode = (UIModeEnum)(((int)uiMode + 1) % 2);
			}
		}

    	void OnGUI() {
			if (uiMode == UIModeEnum.Hidden)
				return;
			
    		GUILayout.BeginVertical();
    		oitEnabled = GUILayout.Button(string.Format("OIT {0}", (oitEnabled ? "Enabled" : "Disabled"))) == true ? !oitEnabled : oitEnabled;
    		weightEnabled = GUILayout.Button(string.Format("Weight {0}", (weightEnabled ? "Enabled" : "Disabled"))) == true ? !weightEnabled : weightEnabled;
    		weight = Mathf.Min(0f, weight);
			Shader.SetGlobalFloat("_Weight", (weightEnabled ? weight : 0f));
    		GUILayout.EndVertical();
    	}
    }
}