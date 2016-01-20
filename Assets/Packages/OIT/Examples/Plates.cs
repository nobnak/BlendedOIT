using UnityEngine;
using System.Collections;

namespace OIT {

	public class Plates : MonoBehaviour {
		public const string PROP_MAIN_TEX = "_MainTex";
		public const string PROP_COLOR = "_TintColor";
		public const float ROUND_RAD = 2f * Mathf.PI;

		public GameObject platefab;
		public PlateData[] plateDataset;
		public float speed = 1f;
		public float space = 1f;

		Transform[] _plates;
		Renderer[] _renderers;
		MaterialPropertyBlock[] _blocks;

		void Start () {
			_plates = new Transform[plateDataset.Length];
			_blocks = new MaterialPropertyBlock[_plates.Length];
			_renderers = new Renderer[_plates.Length];

			var offset = -0.5f * space * _plates.Length;

			for (var i = 0; i < _plates.Length; i++) {
				var inst = (GameObject)Instantiate(platefab, new Vector3(i * space + offset, 0f, 0f), Quaternion.identity);
				_plates[i] = inst.transform;
				_plates[i].SetParent(transform, false);

				_renderers[i] = inst.GetComponent<Renderer>();
				_blocks[i] = new MaterialPropertyBlock();
				_renderers[i].GetPropertyBlock(_blocks[i]);
				plateDataset[i].SetProperties(_blocks[i]);
				_renderers[i].SetPropertyBlock(_blocks[i]);
			}
		}
		
		// Update is called once per frame
		void Update () {
			var dr = Time.deltaTime * speed * ROUND_RAD;

			for (var i = 0; i < plateDataset.Length; i++) {
				plateDataset[i].SetProperties(_blocks[i]);
				_renderers[i].SetPropertyBlock(_blocks[i]);
				_plates[i].localRotation *= Quaternion.Euler(0f, dr, 0f);
			}
		}

		[System.Serializable]
		public class PlateData {
			public Color color;
			public Texture texture;

			public void SetProperties(MaterialPropertyBlock b) {
				if (texture != null)
					b.SetTexture(PROP_MAIN_TEX, texture);
				b.SetColor(PROP_COLOR, color);
			}
		}
	}
}
