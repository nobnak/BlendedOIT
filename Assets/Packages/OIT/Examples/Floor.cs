using UnityEngine;
using System.Collections;

namespace OIT {

	public class Floor : MonoBehaviour {
		public GameObject fab;
		public Bounds field;
		public int count;

		void Start () {
			for (var i = 0; i < count; i++) {
				var pos = RandomPositionInField();
				var go = (GameObject)Instantiate(fab, pos, Random.rotationUniform);
				go.transform.SetParent(transform, true);
			}
		}
		void OnDrawGizmos() {
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(field.center, field.size);
		}

		Vector3 RandomPositionInField() {
			var min = field.min;
			var max = field.max;
			return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		}
	}
}